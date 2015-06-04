using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Projects.Contracts.Enums;
using Projects.Contracts.Events;
using Projects.Domain;

// ReSharper disable InconsistentNaming
namespace UnitTests.Domain
{
    public class when_creating_a_new_project : project_aggregate_specs
    {

        protected override void When()
        {
            sut.Create(projectId, projectName, defaultMetrics);
        }

        [Then]
        public void it_should_trigger_a_project_created_event()
        {
            Assert.That(GetUncommittedEvents().Count(), Is.EqualTo(1));
            Assert.That(GetUncommittedEvents().First(), Is.InstanceOf<ProjectCreated>());
        }

        private ProjectCreated GetEvent()
        {
            return (ProjectCreated) GetUncommittedEvents().First();
        }

        [Then]
        public void it_should_set_version_to_1()
        {
            Assert.That(((IAggregate)sut).Version, Is.EqualTo(1));
        }

        [Then]
        public void it_should_set_id()
        {
            Assert.That(((IAggregate)sut).Id, Is.EqualTo(projectId));
        }

        [Then]
        public void it_should_set_status_to_active()
        {
            Assert.That(state.Status, Is.EqualTo(ProjectStatus.Active));
        }

        [Then]
        public void it_should_set_id_on_event()
        {
            Assert.That(GetEvent().Id, Is.EqualTo(projectId));
        }

        [Then]
        public void it_should_set_name_on_event()
        {
            Assert.That(GetEvent().Name, Is.EqualTo(projectName));
        }

        [Then]
        public void it_should_add_the_default_metrics_to_the_project()
        {
            Assert.That(state.Metrics.ToMetricIds(), Is.EquivalentTo(defaultMetrics.ToMetricIds()));
        }
    }

    public class when_re_creating_an_existing_project : project_aggregate_specs
    {
        protected override IEnumerable<object> GetEvents()
        {
            return new[] { new ProjectCreated { Id = projectId, Name = projectName } };
        }

        [Then]
        public void it_should_throw()
        {
            Assert.Throws<InvalidOperationException>(() => sut.Create(Guid.NewGuid(), projectName + "...", defaultMetrics));
        }
    }

    public class when_re_creating_a_new_project_with_invalid_name : project_aggregate_specs
    {
        [Then]
        public void it_should_throw()
        {
            Assert.Throws<InvalidOperationException>(() => sut.Create(projectId, null, defaultMetrics));
        }
    }
}