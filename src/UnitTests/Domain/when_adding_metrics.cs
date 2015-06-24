using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Projects.Contracts.Events;
using Projects.Domain;

// ReSharper disable InconsistentNaming
namespace UnitTests.Domain
{
    public class when_adding_metrics : project_aggregate_specs
    {
        private readonly Guid[] metricIds = {Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()};

        protected override IEnumerable<object> GetEvents()
        {
            return new[]
            {
                new ProjectCreated {Id = projectId, Name = projectName, DefaultMetrics = defaultMetrics}
            };
        }

        protected override void When()
        {
            sut.AddMetrics(metricIds);
        }

        [Then]
        public void it_should_trigger_team_members_added_event()
        {
            Assert.That(GetUncommittedEvents().Count(), Is.EqualTo(1));
            Assert.That(GetUncommittedEvents().First(), Is.TypeOf<MetricsAdded>());
        }

        [Then]
        public void it_should_set_properties_of_event_as_expected()
        {
            var e = (MetricsAdded)GetUncommittedEvents().First();
            Assert.That(e.Id, Is.EqualTo(projectId));
            Assert.That(e.Metrics.ToMetricIds().Except(defaultMetrics.ToMetricIds()), Is.EquivalentTo(metricIds));
        }

        [Then]
        public void it_should_set_metrics_on_state()
        {
            Assert.That(state.Metrics.ToMetricIds().Except(defaultMetrics.ToMetricIds()), Is.EquivalentTo(metricIds));
        }
    }

    public static class Extensions
    {
        public static Guid[] ToMetricIds(this IEnumerable<MetricInfo> x)
        {
            return x.Select(y => y.MetricId).ToArray();
        }

        public static Guid[] ToMetricIds(this IEnumerable<MetricState> x)
        {
            return x.Select(y => y.Id).ToArray();
        }

        public static MetricInfo[] ToMetricInfos(this IEnumerable<Guid> ids)
        {
            return ids.Select(x => new MetricInfo {MetricId = x, IsDefault = false}).ToArray();
        }
    }

    public class when_trying_to_add_an_already_existing_metrics : project_aggregate_specs
    {
        private MetricInfo[] metrics;
        private Guid[] metricIds2;

        protected override void Given()
        {
            metrics = new[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() }.ToMetricInfos();
            metricIds2 = new[] { Guid.NewGuid(), Guid.NewGuid(), metrics[0].MetricId }; 
            base.Given();
        }

        protected override IEnumerable<object> GetEvents()
        {
            return new object[]
            {
                new ProjectCreated {Id = projectId, Name = projectName, DefaultMetrics = defaultMetrics},
                new MetricsAdded{Id = projectId, Metrics = metrics}
            };
        }

        protected override void When()
        {
            sut.AddMetrics(metricIds2);
        }

        [Then]
        public void it_should_trigger_metrics_added_event()
        {
            Assert.That(GetUncommittedEvents().Count(), Is.EqualTo(1));
            Assert.That(GetUncommittedEvents().First(), Is.TypeOf<MetricsAdded>());
        }

        [Then]
        public void it_should_set_the_staffIds_property_of_the_event_to_only_include_not_yet_added_metrics()
        {
            var e = (MetricsAdded)GetUncommittedEvents().First();
            Assert.That(e.Metrics.ToMetricIds(), Is.EquivalentTo(metricIds2.Take(2)));
        }

        [Then]
        public void it_should_only_add_the_not_yet_present_metrics_to_state()
        {
            Assert.That(state.Metrics.ToMetricIds(), Is.EquivalentTo(defaultMetrics.Union(metrics).ToMetricIds().Union(metricIds2).Distinct()));
        }
    }

    public class when_trying_to_add_null_or_empty_array_of_metrics : project_aggregate_specs
    {
        protected override IEnumerable<object> GetEvents()
        {
            return new[]
            {
                new ProjectCreated {Id = projectId, Name = projectName, DefaultMetrics = defaultMetrics}
            };
        }

        [Then]
        public void it_should_fail_for_empty_array()
        {
            Assert.Throws<InvalidOperationException>(() => sut.AddMetrics(new Guid[0]));
        }

        [Then]
        public void it_should_fail_for_null_array()
        {
            Assert.Throws<InvalidOperationException>(() => sut.AddMetrics(null));
        }
    }

    public class when_trying_to_update_metric_value : project_aggregate_specs
    {
        protected override IEnumerable<object> GetEvents()
        {
            return new[]
            {
                new ProjectCreated {Id = projectId, Name = projectName, DefaultMetrics = defaultMetrics}
            };
        }

        protected override void When()
        {
            sut.UpdateMetrics(defaultMetrics[0].MetricId, 1);
        }

        [Then]
        public void it_should_trigger_updated_metric_event()
        {
            Assert.That(GetUncommittedEvents().Count(), Is.EqualTo(1));
            Assert.That(GetUncommittedEvents().First(), Is.TypeOf<MetricUpdated>());
        }

        [Then]
        public void it_should_set_the_value_of_metric_to_one()
        {
            var e = (MetricUpdated)GetUncommittedEvents().First();
            Assert.That(e.Value, Is.EqualTo(1));
        }
    }
}