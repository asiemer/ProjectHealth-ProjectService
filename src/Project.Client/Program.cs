using System;
using NServiceBus;

class Program
{
    private static void Main()
    {
        var busConfiguration = new BusConfiguration();
        busConfiguration.EndpointName("Projects.Project.Client");
        busConfiguration.UseSerialization<XmlSerializer>();
        busConfiguration.EnableInstallers();
        busConfiguration.UsePersistence<InMemoryPersistence>();

        using (var bus = Bus.Create(busConfiguration).Start())
        {
            SendOrder(bus);
        }
    }

    private static void SendOrder(IBus bus)
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