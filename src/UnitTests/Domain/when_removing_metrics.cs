using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Projects.Contracts.Events;
using Projects.Domain;

// ReSharper disable InconsistentNaming
namespace UnitTests.Domain
{
    public class when_removing_metrics : project_aggregate_specs
    {
        private readonly MetricInfo[] metrics = new[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() }.ToMetricInfos();
        private Guid[] metricIds2Remove;

        protected override IEnumerable<object> GetEvents()
        {
            metricIds2Remove = metrics.Select(x => x.MetricId).Take(2).ToArray();
            return new object[]
            {
                new ProjectCreated {Id = projectId, Name = projectName, DefaultMetrics = defaultMetrics},
                new MetricsAdded{Id = projectId, Metrics = metrics }
            };
        }

        protected override void When()
        {
            sut.RemoveMetrics(metricIds2Remove);
        }

        [Then]
        public void it_should_trigger_metrics_removed_event()
        {
            Assert.That(GetUncommittedEvents().Count(), Is.EqualTo(1));
            Assert.That(GetUncommittedEvents().First(), Is.TypeOf<MetricsRemoved>());
        }

        [Then]
        public void it_should_set_properties_of_event_as_expected()
        {
            var e = (MetricsRemoved)GetUncommittedEvents().First();
            Assert.That(e.Id, Is.EqualTo(projectId));
            Assert.That(e.Metrics.ToMetricIds(), Is.EquivalentTo(metricIds2Remove));
        }

        [Then]
        public void it_should_update_metrics_on_state()
        {
            Assert.That(state.Metrics.ToMetricIds(), Is.EquivalentTo(defaultMetrics.Union(metrics).ToMetricIds().Except(metricIds2Remove)));
        }
    }

    public class when_trying_to_remove_a_default_metrics : project_aggregate_specs
    {
    }

    public class when_trying_to_remove_non_existing_metrics : project_aggregate_specs
    {
        private MetricInfo[] metrics;
        private Guid[] metricIds2Remove;

        protected override IEnumerable<object> GetEvents()
        {
            metrics = new[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() }.ToMetricInfos();
            metricIds2Remove = metrics.Select(x => x.MetricId).Take(2).Union(new[] { Guid.NewGuid() }).ToArray();
            return new object[]
            {
                new ProjectCreated {Id = projectId, Name = projectName},
                new MetricsAdded{Id = projectId, Metrics = metrics }
            };
        }

        protected override void When()
        {
            sut.RemoveMetrics(metricIds2Remove);
        }

        [Then]
        public void it_should_trigger_metrics_removed_event()
        {
            Assert.That(GetUncommittedEvents().Count(), Is.EqualTo(1));
            Assert.That(GetUncommittedEvents().First(), Is.TypeOf<MetricsRemoved>());
        }

        [Then]
        public void it_should_set_the_metricIds_property_of_the_event_to_only_include_truly_removed_metrics()
        {
            var e = (MetricsRemoved)GetUncommittedEvents().First();
            Assert.That(e.Metrics.Select(x => x.MetricId), Is.EquivalentTo(metricIds2Remove.Intersect(metrics.Select(x => x.MetricId))));
        }

        [Then]
        public void it_should_update_metrics_on_state()
        {
            Assert.That(state.Metrics.Select(x => x.Id), Is.EquivalentTo(metrics.Select(x => x.MetricId).Except(metricIds2Remove)));
        }
    }

    public class when_trying_to_remove_null_or_empty_array_of_metrics : project_aggregate_specs
    {
        protected override IEnumerable<object> GetEvents()
        {
            return new[]
            {
                new ProjectCreated {Id = projectId, Name = projectName}
            };
        }

        [Then]
        public void it_should_fail_for_empty_array()
        {
            Assert.Throws<InvalidOperationException>(() => sut.RemoveMetrics(new Guid[0]));
        }

        [Then]
        public void it_should_fail_for_null_array()
        {
            Assert.Throws<InvalidOperationException>(() => sut.RemoveMetrics(null));
        }
    }
}