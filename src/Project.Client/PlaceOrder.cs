using System;
using NServiceBus;

public class PlaceOrder : IEvent
{
    public Guid Id { get; set; }

    public string Product { get; set; }
}

