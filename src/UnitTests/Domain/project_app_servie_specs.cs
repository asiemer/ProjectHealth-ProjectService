using System;
using System.Linq;
using NUnit.Framework;
using Projects.Contracts.Commands;
using Projects.Contracts.Events;
using Projects.Domain;
using Rhino.Mocks;

namespace UnitTests.Domain
{
    public abstract class project_app_servie_specs : SpecificationBase
    {
        protected ProjectApplicationService sut;
        protected object cmd;
        protected IRepository repository;
        protected bool getWasCalled;
        protected bool saveWasCalled;
        protected ProjectAggregate aggregate;

        protected override void Given()
        {
            aggregate = new ProjectAggregate(null);
            repository = MockRepository.GenerateMock<IRepository>();
            repository.Stub(x => x.GetById<ProjectAggregate>(Arg<Guid>.Is.Anything))
                .WhenCalled(mi => getWasCalled = true)
                .Return(aggregate);
            repository.Stub(x => x.Save(Arg<ProjectAggregate>.Is.Anything))
                .WhenCalled(mi => saveWasCalled = true);
            sut = new ProjectApplicationService(repository);
        }

        protected override void When()
        {
            sut.Execute(cmd);
        }
    }

    public class when_creating_new_project : project_app_servie_specs
    {
        protected override void Given()
        {
            base.Given();
            cmd = new CreateProject
            {
                Name = "foo"
            };
        }

        [Then]
        public void it_should_use_repository_to_rehydrate_aggregate()
        {
            Assert.That(getWasCalled, Is.True);
        }

        [Then]
        public void it_should_create_aggregate()
        {
            Assert.That(((IAggregate)aggregate).GetUncommittedEvents().First(), Is.InstanceOf<ProjectCreated>());
        }

        [Then]
        public void it_should_use_repository_to_save_changes_to_aggregate()
        {
            Assert.That(saveWasCalled, Is.True);
        }
    }
}