using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using log4net;
using Projects;
using Projects.Infrastructure;
using Projects.ReadModel.Observers;

namespace ReadModelTool
{
    public class Replayer
    {
        private Dictionary<Type, Info[]> _dictionary = new Dictionary<Type, Info[]>();
        private readonly IApplicationSettings _applicationSettings;
        private readonly ILog _log;

        public Replayer(IApplicationSettings applicationSettings, ILog log)
        {
            _applicationSettings = applicationSettings;
            _log = log;
        }

        public async Task ProcessEvents()
        {
            var factory = new MongoDbAtomicWriterFactory(_applicationSettings.MongoDbConnectionString, _applicationSettings.MongoDbName);
            var observers = new ObserverRegistry().GetObservers(factory);
            WireUpObservers(observers);

            var endpoint = GetEventStoreEndpoint();
            var connection = EventStoreConnection.Create(endpoint);
            await connection.ConnectAsync();
            const int maxCount = 500;
            var credentials = new UserCredentials(_applicationSettings.GesUserName, _applicationSettings.GesPassword);

            var position = Position.Start;
            do
            {
                var slice = await connection.ReadAllEventsForwardAsync(position, maxCount, false, credentials);
                foreach (var resolvedEvent in slice.Events)
                    await HandleEvent(resolvedEvent);
                position = slice.NextPosition;
                if(slice.IsEndOfStream) break;
            } while(true);
            var repo = new MongoDbLastProcessedEventRepository(_applicationSettings.MongoDbConnectionString, _applicationSettings.MongoDbName);
            await repo.Save(position);
        }

        private async Task HandleEvent(ResolvedEvent re)
        {
            if (re.OriginalEvent.EventType.StartsWith("$")) return; //skip internal events
            if (re.OriginalEvent.Metadata == null || re.OriginalEvent.Metadata.Any() == false) return;
            try
            {
                var e = re.DeserializeEvent();
                await Dispatch(e);
            }
            catch (Exception exception)
            {
                _log.Error(string.Format("Could not deserialize event {0}", re.OriginalEvent.EventType), exception);
            }
        }

        private async Task Dispatch(object e)
        {
            var eventType = e.GetType();
            if (_dictionary.ContainsKey(eventType) == false)
                return;

            foreach (var item in _dictionary[eventType])
            {
                try
                {
                    await ((Task)item.MethodInfo.Invoke(item.Observer, new[] { e }));
                }
                catch (Exception ex)
                {
                    _log.Error(string.Format("Could not dispatch event {0} to projection {1}",
                        eventType.Name, item.Observer.GetType().Name), ex);
                }
            }
        }

        private void WireUpObservers(IEnumerable<object> projections)
        {
            _dictionary = projections.Select(p => new { Projection = p, Type = p.GetType() })
                .Select(x => new
                {
                    x.Projection,
                    MethodInfos = x.Type
                        .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                        .Where(m => m.Name == "When" && m.GetParameters().Count() == 1)
                })
                .SelectMany(x => x.MethodInfos.Select(
                    y => new { x.Projection, MethodInfo = y, y.GetParameters().First().ParameterType }))
                .GroupBy(x => x.ParameterType)
                .ToDictionary(g => g.Key,
                    g => g.Select(y => new Info { Observer = y.Projection, MethodInfo = y.MethodInfo }).ToArray());
        }

        private IPEndPoint GetEventStoreEndpoint()
        {
            var ipAddress = IPAddress.Parse(_applicationSettings.GesIpAddress);
            var endpoint = new IPEndPoint(ipAddress, _applicationSettings.GesTcpIpPort);
            return endpoint;
        }

        public class Info
        {
            public MethodInfo MethodInfo;
            public object Observer;
        }
    }
}