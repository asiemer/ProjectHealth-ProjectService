using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Projects.Contracts.Events;

// ReSharper disable InconsistentNaming
namespace UnitTests.Domain
{
    public class when_adding_team_members : project_aggregate_specs
    {
        private readonly Guid[] staffIds = {Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()};

        protected override IEnumerable<object> GetEvents()
        {
            return new[]
            {
                new ProjectCreated {Id = projectId, Name = projectName}
            };
        }

        protected override void When()
        {
            sut.AddTeamMembers(staffIds);
        }

        [Then]
        public void it_should_trigger_team_members_added_event()
        {
            Assert.That(GetUncommittedEvents().Count(), Is.EqualTo(1));
            Assert.That(GetUncommittedEvents().First(), Is.TypeOf<TeamMembersAdded>());
        }

        [Then]
        public void it_should_set_properties_of_event_as_expected()
        {
            var e = (TeamMembersAdded)GetUncommittedEvents().First();
            Assert.That(e.Id, Is.EqualTo(projectId));
            Assert.That(e.StaffIds, Is.EquivalentTo(staffIds));
        }

        [Then]
        public void it_should_set_team_members_on_state()
        {
            Assert.That(state.TeamMembers, Is.EquivalentTo(staffIds));
        }
    }

    public class when_trying_to_add_an_already_existing_team_member : project_aggregate_specs
    {
        private Guid[] staffIds;
        private Guid[] staffIds2;

        protected override IEnumerable<object> GetEvents()
        {
            return new object[]
            {
                new ProjectCreated {Id = projectId, Name = projectName},
                new TeamMembersAdded{Id = projectId, StaffIds = staffIds}
            };
        }

        protected override void Given()
        {
            staffIds = new[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
            staffIds2 = new[] { Guid.NewGuid(), Guid.NewGuid(), staffIds[0] };
            base.Given();
        }

        protected override void When()
        {
            sut.AddTeamMembers(staffIds2);
        }

        [Then]
        public void it_should_trigger_team_members_added_event()
        {
            Assert.That(GetUncommittedEvents().Count(), Is.EqualTo(1));
            Assert.That(GetUncommittedEvents().First(), Is.TypeOf<TeamMembersAdded>());
        }

        [Then]
        public void it_should_set_the_staffIds_property_of_the_event_to_only_include_not_yet_added_team_members()
        {
            var e = (TeamMembersAdded)GetUncommittedEvents().First();
            Assert.That(e.StaffIds, Is.EquivalentTo(staffIds2.Take(2)));
        }

        [Then]
        public void it_should_only_add_the_not_yet_present_team_members_to_state()
        {
            Assert.That(state.TeamMembers, Is.EquivalentTo(staffIds.Union(staffIds2).Distinct()));
        }
    }

    public class when_trying_to_add_null_or_empty_array_of_team_members : project_aggregate_specs
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
            Assert.Throws<InvalidOperationException>(() => sut.AddTeamMembers(new Guid[0]));
        }

        [Then]
        public void it_should_fail_for_null_array()
        {
            Assert.Throws<InvalidOperationException>(() => sut.AddTeamMembers(null));
        }
    }
}