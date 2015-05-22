using System;
using System.Collections.Generic;

namespace Projects.Domain
{
    public class AggregateFactory : IAggregateFactory
    {
        public T Create<T>(IEnumerable<object> events) where T : class, IAggregate
        {
            if (typeof (T) == typeof (SampleAggregate))
            {
                var state = new SampleState(events);
                return new SampleAggregate(state) as T;
            }

            if (typeof (T) == typeof (ProjectAggregate))
            {
                var state = new ProjectState(events);
                return new ProjectAggregate(state) as T;
            }

            throw new ArgumentException("Unknown aggregate type");
        }
    }
}