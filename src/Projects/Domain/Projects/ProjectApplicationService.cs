using System;
using Projects.Contracts.Commands;

namespace Projects.Domain
{
    public interface IProjectsApplicationService {
        void Execute(object command);
        Guid When(CreateProject command);
    }

    public class ProjectApplicationService : IProjectsApplicationService
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

        public Guid When(CreateProject cmd)
        {
            var id = Guid.NewGuid();
            InternalAct(id, aggregate => aggregate.Create(id, cmd.Name));
            return id;
        }

        public void When(SetCem cmd)
        {
            InternalAct(cmd.Id, aggregate => aggregate.SetCem(cmd.StaffId));
        }

        private void InternalAct(Guid id, Action<ProjectAggregate> action)
        {
            var aggregate = _repository.GetById<ProjectAggregate>(id);
            action(aggregate);
            _repository.Save(aggregate);
        }
    }
}