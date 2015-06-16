using System;
using System.IO;
using System.Web.Http;
using log4net;
using log4net.Config;
using StatsdClient;
using StructureMap;

namespace Projects
{
    public static class Bootstrap
    {
        public static void Init()
        {
            InitLogging();
            ObjectFactory.Initialize(init =>
            {
                init.For<ILog>().Singleton().Use(c => LogManager.GetLogger("Projects"));
            });
            GlobalConfiguration.Configuration.DependencyResolver =
                new StructureMapResolver(ObjectFactory.Container);

            var metricsConfig = new MetricsConfig
            {
                StatsdServerName = "statsd.hostedgraphite.com",
                Prefix = "9f05a9e6-ebc5-49bd-90fa-0c8689e7fbbf.CM.Heartbeat.DEV.IterationZero"
            };

            StatsdClient.Metrics.Configure(metricsConfig);
        }

        private static void InitLogging()
        {
            XmlConfigurator.Configure(new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.config")));
        }
    }
}