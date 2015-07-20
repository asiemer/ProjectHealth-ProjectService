using System;
using System.IO;
using System.Web.Http;
using log4net;
using log4net.Config;
using NServiceBus;
using StatsdClient;
using StructureMap;

namespace Projects
{
    public static class Bootstrap
    {
        public static void Init()
        {
            InitLogging();
            InitMetrics();
            var bus = InitBus();
            InitContainer(bus);
        }

        private static void InitMetrics()
        {
            var metricsConfig = new MetricsConfig
            {
                StatsdServerName = "statsd.hostedgraphite.com",
                Prefix = "9f05a9e6-ebc5-49bd-90fa-0c8689e7fbbf.CM.Heartbeat.DEV.IterationZero"
            };

            StatsdClient.Metrics.Configure(metricsConfig);
        }

        static void InitContainer(IBus bus)
        {
            var container = new Container(init =>
            {
                init.For<IBus>().Singleton().Use(c => bus);
                init.For<ILog>().Singleton().Use(c => LogManager.GetLogger("Projects"));
            });
            GlobalConfiguration.Configuration.DependencyResolver = new StructureMapResolver(container);
        }

        static IBus InitBus()
        {
            var config = new BusConfiguration();
            config.EndpointName("Heartbeat.Projects.API");
            config.UsePersistence<InMemoryPersistence>();
            config.UseTransport<SqlServerTransport>();
            config.EnableInstallers();
            config.Conventions().DefiningCommandsAs(t => t.Namespace != null && t.Namespace.StartsWith("Projects.Contracts.Commands"));
            IBus bus = Bus.Create(config).Start();
            //SetLoggingLibrary.Log4Net(() => XmlConfigurator.ConfigureAndWatch(new FileInfo("log4net.config")));
            return bus;
        }

        private static void InitLogging()
        {
            XmlConfigurator.Configure(new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.config")));
        }
    }
}