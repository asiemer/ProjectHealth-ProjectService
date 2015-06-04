using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using Projects.Contracts.Enums;
using Projects.Contracts.Events;

namespace UnitTests.Domain
{
    public class when_suspending_a_project : project_aggregate_specs
    {
        protected override IEnumerable<object> GetEvents()
        {
            return new object[]
            {
                new ProjectCreated { Id = projectId, Name = projectName, DefaultMetrics = defaultMetrics },
                new CemSet{Id = projectId, StaffId = Guid.NewGuid()},
                new PmSet{Id = projectId, StaffId = Guid.NewGuid()},
                new TeamMembersAdded{Id = projectId, StaffIds = new[]{Guid.NewGuid()}}
            };
        }

        protected override void When()
        {
            sut.Suspend();
        }

        [Then]
        public void it_should_trigger_a_project_suspended_event()
        {
            Assert.That(GetUncommittedEvents().Count(), Is.EqualTo(1));
            Assert.That(GetUncommittedEvents().First(), Is.TypeOf<ProjectSuspended>());
        }

        [Then]
        public void it_should_set_state_to_suspended()
        {
            Assert.That(state.Status, Is.EqualTo(ProjectStatus.Suspended));
        }
    }

    public class when_trying_to_suspend_a_project_that_is_not_active : project_aggregate_specs
    {
        protected override IEnumerable<object> GetEvents()
        {
            return new object[]
            {
                new ProjectCreated { Id = projectId, Name = projectName, DefaultMetrics = defaultMetrics },
                new ProjectSuspended{Id = projectId}
            };
        }

        [Then]
        public void it_should_fail()
        {
            Assert.Throws<InvalidOperationException>(() => sut.Suspend());
        }
    }
}