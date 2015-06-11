using System;
using NServiceBus;

namespace Project.Client
{
    public class Program
    {
        static void Main()
        {
            var busConfiguration = new BusConfiguration();
            busConfiguration.EndpointName("Projects.Project.Client");
            busConfiguration.UseSerialization<JsonSerializer>();
            busConfiguration.EnableInstallers();
            busConfiguration.UsePersistence<InMemoryPersistence>();

            using (IBus bus = Bus.Create(busConfiguration).Start())
            {
                SendOrder(bus);
            }
        }
        static void SendOrder(IBus bus)
        {
            Console.WriteLine("Press 'Enter' to send a message. To exit press 'Ctrl + C'");

            while (Console.ReadLine() != null)
            {
                Guid id = Guid.NewGuid();

                var placeOrder = new PlaceOrder
                {
                    Product = "New shoes",
                    Id = id
                };
                bus.Send("Dashboard.Dashboard.Handler", placeOrder);

                Console.WriteLine("Sent a new PlaceOrder message with id: {0}", id.ToString("N"));
            }
        }
    }
}
