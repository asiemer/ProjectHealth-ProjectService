using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Projects.Contracts.Enums;
using Projects.Contracts.Events;
using Projects.Domain;

namespace UnitTests.Domain
{
    public abstract class project_aggregate_specs : SpecificationBase
    {
        protected ProjectAggregate sut;

        protected IEnumerable<object> GetUncommittedEvents()
        {
            return ((IAggregate) sut).GetUncommittedEvents();
        }
    }

    public class when_creating_a_new_project : project_aggregate_specs
    {
        private Guid projectId;
        private string projectName;
        private ProjectState state;

        protected override void Given()
        {
            base.Given();
            projectId = Guid.NewGuid();
            projectName = "Some project";
            state = new ProjectState(new object[0]);
            sut = new ProjectAggregate(state);
        }

        protected override void When()
        {
            sut.Create(projectId, projectName);
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
        public void it_should_set_status_to_draft()
        {
            Assert.That(state.Status, Is.EqualTo(ProjectStatus.Draft));
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
    }

    public class when_re_creating_an_existing_project : project_aggregate_specs
    {
        private Guid projectId;
        private string projectName;
        private ProjectState state;

        protected override void Given()
        {
            base.Given();
            projectId = Guid.NewGuid();
            projectName = "Some project";
            state = new ProjectState(new Object[0]);
            sut = new ProjectAggregate(state);
            sut.Create(Guid.NewGuid(), "Interesting project");
        }

        [Then]
        public void it_should_throw()
        {
            Assert.Throws<InvalidOperationException>(() => sut.Create(projectId, projectName));
        }
    }

    public class when_re_creating_a_new_project_with_invalid_name : project_aggregate_specs
    {
        private Guid projectId;
        private ProjectState state;

        protected override void Given()
        {
            base.Given();
            projectId = Guid.NewGuid();
            state = new ProjectState(new Object[0]);
            sut = new ProjectAggregate(state);
        }

        [Then]
        public void it_should_throw()
        {
            Assert.Throws<InvalidOperationException>(() => sut.Create(projectId, null));
        }
    }

    public class when_setting_the_cem : project_aggregate_specs
    {
        private Guid staffId;
        private string name ="Foo";
        private Guid projectId = Guid.NewGuid();

        protected override void Given()
        {
            base.Given();
            staffId = Guid.NewGuid();
            sut = new ProjectAggregate(new ProjectState(new Object[0]));
            sut.Create(projectId, name);
            ((IAggregate)sut).ClearUncommittedEvents();
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
    }
}