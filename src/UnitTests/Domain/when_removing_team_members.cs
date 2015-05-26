using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Projects.Contracts.Events;

// ReSharper disable InconsistentNaming
namespace UnitTests.Domain
{
    public class when_removing_team_members : project_aggregate_specs
    {
        private readonly Guid[] staffIds = { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
        private Guid[] staffIds2Remove;

        protected override IEnumerable<object> GetEvents()
        {
            staffIds2Remove = staffIds.Take(2).ToArray();
            return new object[]
            {
                new ProjectCreated {Id = projectId, Name = projectName, DefaultMetrics = defaultMetrics},
                new TeamMembersAdded{Id = projectId, StaffIds = staffIds }
            };
        }

        protected override void When()
        {
            sut.RemoveTeamMembers(staffIds2Remove);
        }

        [Then]
        public void it_should_trigger_team_members_removed_event()
        {
            Assert.That(GetUncommittedEvents().Count(), Is.EqualTo(1));
            Assert.That(GetUncommittedEvents().First(), Is.TypeOf<TeamMembersRemoved>());
        }

        [Then]
        public void it_should_set_properties_of_event_as_expected()
        {
            var e = (TeamMembersRemoved)GetUncommittedEvents().First();
            Assert.That(e.Id, Is.EqualTo(projectId));
            Assert.That(e.StaffIds, Is.EquivalentTo(staffIds2Remove));
        }

        [Then]
        public void it_should_update_team_members_on_state()
        {
            Assert.That(state.TeamMembers, Is.EquivalentTo(staffIds.Except(staffIds2Remove)));
        }
    }

    public class when_trying_to_remove_non_existing_team_members : project_aggregate_specs
    {
        private Guid[] staffIds;
        private Guid[] staffIds2Remove;

        protected override IEnumerable<object> GetEvents()
        {
            staffIds = new[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
            staffIds2Remove = staffIds.Take(2).Union(new[] { Guid.NewGuid() }).ToArray();
            return new object[]
            {
                new ProjectCreated {Id = projectId, Name = projectName},
                new TeamMembersAdded{Id = projectId, StaffIds = staffIds }
            };
        }

        protected override void When()
        {
            sut.RemoveTeamMembers(staffIds2Remove);
        }

        [Then]
        public void it_should_trigger_team_members_removed_event()
        {
            Assert.That(GetUncommittedEvents().Count(), Is.EqualTo(1));
            Assert.That(GetUncommittedEvents().First(), Is.TypeOf<TeamMembersRemoved>());
        }

        [Then]
        public void it_should_set_the_staffIds_property_of_the_event_to_only_include_truly_removed_team_members()
        {
            var e = (TeamMembersRemoved)GetUncommittedEvents().First();
            Assert.That(e.StaffIds, Is.EquivalentTo(staffIds2Remove.Intersect(staffIds)));
        }

        [Then]
        public void it_should_update_team_members_on_state()
        {
            Assert.That(state.TeamMembers, Is.EquivalentTo(staffIds.Except(staffIds2Remove)));
        }
    }

    public class when_trying_to_remove_null_or_empty_array_of_team_members : project_aggregate_specs
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
            Assert.Throws<InvalidOperationException>(() => sut.RemoveTeamMembers(new Guid[0]));
        }

        [Then]
        public void it_should_fail_for_null_array()
        {
            Assert.Throws<InvalidOperationException>(() => sut.RemoveTeamMembers(null));
        }
    }
}