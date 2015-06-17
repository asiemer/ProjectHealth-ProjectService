using System;
using System.IO;
using System.Net;
using EventStore.ClientAPI;
using log4net;
using log4net.Config;
using Projects.Domain;
using Projects.Infrastructure;
using Projects.ReadModel.Observers;
using Projects.ReadModel.Providers;
using Projects.Services;
using StructureMap;

namespace ProjectsHandler
{
    public static class Bootstrap
    {
        private static IRepository _repository;
        private static EventsDispatcher _dispatcher;

        public static IContainer Init()
        {
            InitLogging();
            var container = new Container(init =>
            {
                init.For<IRepository>().Use(c => _repository);
                init.For<IApplicationSettings>().Use<ApplicationSettings>();
                init.For<IUniqueKeyGenerator>().Use<UniqueKeyGenerator>();
                init.For<IProjectApplicationService>().Use<ProjectApplicationService>();
                init.For<IProjectsProvider>().Use<ProjectsProvider>();
                init.For<ILog>().Singleton().Use(c => LogManager.GetLogger("Projects"));

                //***************** MOCKS ************************************
                init.For<IMetricsFileGetter>().Use<MetricsFileGetter>();
                init.For<IMetricsProvider>().Use<MetricsProviderMock>();
                //************************************************************
            });

            var applicationSettings = container.GetInstance<IApplicationSettings>();
            InitGetEventStore(applicationSettings);
            InitEventsDispatcher(applicationSettings, container.GetInstance<ILog>());
            return container;
        }

        private static void InitLogging()
        {
            XmlConfigurator.Configure(new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.config")));
        }

        private static void InitGetEventStore(IApplicationSettings applicationSettings)
        {
            var endpoint = GetEventStoreEndpoint(applicationSettings);
            var connection = EventStoreConnection.Create(endpoint);
            connection.ConnectAsync().Wait();
            var factory = new AggregateFactory();
            _repository = new GesRepository(connection, factory);
        }

        private async static void InitEventsDispatcher(IApplicationSettings applicationSettings, ILog logger)
        {
            var endpoint = GetEventStoreEndpoint(applicationSettings);
            var connection = EventStoreConnection.Create(endpoint);
            connection.ConnectAsync().Wait();
            _dispatcher = new EventsDispatcher(logger, applicationSettings);
            var factory = new MongoDbAtomicWriterFactory(applicationSettings.MongoDbConnectionString, applicationSettings.MongoDbName);
            var observers = new ObserverRegistry().GetObservers(factory);
            var repo = new MongoDbLastProcessedEventRepository(applicationSettings.MongoDbConnectionString, applicationSettings.MongoDbName);
            await _dispatcher.Start(connection, observers, repo);
        }

        private static IPEndPoint GetEventStoreEndpoint(IApplicationSettings applicationSettings)
        {
            var ipAddress = IPAddress.Parse(applicationSettings.GesIpAddress);
            var endpoint = new IPEndPoint(ipAddress, applicationSettings.GesTcpIpPort);
            return endpoint;
        }
    }
}