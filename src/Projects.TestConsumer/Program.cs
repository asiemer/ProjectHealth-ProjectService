using System;
using NServiceBus;
using NServiceBus.Features;
using NServiceBus.Persistence;
using Projects.Contracts.Events;

namespace Projects.TestConsumer
{
    class Program
    {
        static void Main()
        {
            var configuration = new BusConfiguration();
            configuration.EndpointName("Heartbeat.Projects.TestClient");
            configuration.EnableInstallers();
            configuration.UsePersistence<NHibernatePersistence>();
            configuration.DisableFeature<AutoSubscribe>();
            configuration.Conventions().DefiningEventsAs(t => t.Namespace != null && t.Namespace.StartsWith("Projects.Contracts.Events"));

            using (IBus bus = Bus.Create(configuration).Start())
            {
                bus.Subscribe<ProjectScoreChanged>();
                Console.WriteLine("To exit press 'Ctrl + C'");
                Console.ReadLine();
            }
        }
    }
}
