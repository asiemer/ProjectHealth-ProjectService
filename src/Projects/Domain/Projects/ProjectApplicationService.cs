using System;
using Projects.Contracts.Commands;

namespace Projects.Domain
{
    public class ProjectApplicationService
    {
        private readonly IRepository _repository;

        public ProjectApplicationService(IRepository repository)
        {
            _repository = repository;
        }

        public void Execute(object command)
        {
            RedirectToWhen.InvokeCommand(this, command);
        }

        private void When(CreateProject cmd)
        {
            var id = Guid.NewGuid();
            InternalAct(id, aggregate => aggregate.Create(id, cmd.Name));
        }

        private void InternalAct(Guid id, Action<ProjectAggregate> action)
        {
            var aggregate = _repository.GetById<ProjectAggregate>(id);
            action(aggregate);
            _repository.Save(aggregate);
        }
    }
}