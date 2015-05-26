using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Projects.Contracts.Events;
using Projects.Domain;

// ReSharper disable InconsistentNaming
namespace UnitTests.Domain
{
    public abstract class project_aggregate_specs : SpecificationBase
    {
        protected ProjectAggregate sut;
        protected ProjectState state;
        protected Guid projectId;
        protected string projectName;
        protected MetricInfo[] defaultMetrics;

        protected override void Given()
        {
            projectId = Guid.NewGuid();
            projectName = "project foo";
            defaultMetrics = new[] {Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()}.Select(x => new MetricInfo{MetricId = x}).ToArray();
            state = new ProjectState(GetEvents());
            sut = new ProjectAggregate(state);
        }

        protected virtual IEnumerable<object> GetEvents()
        {
            return new Object[0];
        } 

        protected IEnumerable<object> GetUncommittedEvents()
        {
            return ((IAggregate) sut).GetUncommittedEvents();
        }
    }

    public class when_setting_the_cem : project_aggregate_specs
    {
        private readonly Guid staffId = Guid.NewGuid();

        protected override IEnumerable<object> GetEvents()
        {
            return new[] { new ProjectCreated { Id = projectId, Name = projectName, DefaultMetrics = defaultMetrics } };
        }

        protected override void When()
        {
            sut.SetCem(staffId);
        }

        [Then]
        public void it_should_trigger_a_cem_set_event()
        {
            Assert.That(GetUncommittedEvents().Count(), Is.EqualTo(1));
            Assert.That(GetUncommittedEvents().First(), Is.TypeOf<CemSet>());
        }

        [Then]
        public void it_should_set_properties_of_event_as_expected()
        {
            var e = (CemSet)GetUncommittedEvents().First();
            Assert.That(e.Id, Is.EqualTo(projectId));
            Assert.That(e.StaffId, Is.EqualTo(staffId));
        }
    }

    public class when_setting_the_pm : project_aggregate_specs
    {
        private readonly Guid staffId = Guid.NewGuid();

        protected override IEnumerable<object> GetEvents()
        {
            return new[] { new ProjectCreated { Id = projectId, Name = projectName, DefaultMetrics = defaultMetrics } };
        }

        protected override void When()
        {
            sut.SetPm(staffId);
        }

        [Then]
        public void it_should_trigger_a_pm_set_event()
        {
            Assert.That(GetUncommittedEvents().Count(), Is.EqualTo(1));
            Assert.That(GetUncommittedEvents().First(), Is.TypeOf<PmSet>());
        }

        [Then]
        public void it_should_set_properties_of_event_as_expected()
        {
            var e = (PmSet) GetUncommittedEvents().First();
            Assert.That(e.Id, Is.EqualTo(projectId));
            Assert.That(e.StaffId, Is.EqualTo(staffId));
        }
    }
}