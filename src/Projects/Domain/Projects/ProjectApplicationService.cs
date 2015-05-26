using System;
using System.Linq;
using Projects.Contracts.Commands;
using Projects.Services;

namespace Projects.Domain
{
    public interface IProjectApplicationService {
        Guid Create(CreateProject command);
        void Execute(object command);
    }

    public class ProjectApplicationService : IProjectApplicationService
    {
        private readonly IRepository _repository;
        private readonly IMetricsProvider _metricsProvider;

        public ProjectApplicationService(IRepository repository, IMetricsProvider metricsProvider)
        {
            _repository = repository;
            _metricsProvider = metricsProvider;
        }

        public void Execute(object command)
        {
            RedirectToWhen.InvokeCommand(this, command);
        }

        public Guid Create(CreateProject cmd)
        {
            var id = Guid.NewGuid();
            var metrics = _metricsProvider.GetCompanyMetrics()
                .Select(x => new MetricInfo
                {
                    MetricId = x.Id,
                    IsDefault = x.IsDefault
                })
                .ToArray();
            InternalAct(id, aggregate => aggregate.Create(id, cmd.Name, metrics));
            return id;
        }

// ReSharper disable UnusedMember.Local
        private void When(SetCem cmd)
        {
            InternalAct(cmd.Id, aggregate => aggregate.SetCem(cmd.StaffId));
        }

        private void When(SetPm cmd)
        {
            InternalAct(cmd.Id, aggregate => aggregate.SetPm(cmd.StaffId));
        }

        private void When(AddTeamMembers cmd)
        {
            InternalAct(cmd.Id, aggregate => aggregate.AddTeamMembers(cmd.StaffIds));
        }

        private void When(RemoveTeamMembers cmd)
        {
            InternalAct(cmd.Id, aggregate => aggregate.RemoveTeamMembers(cmd.StaffIds));
        }

        private void When(AddMetrics cmd)
        {
            InternalAct(cmd.Id, aggregate => aggregate.AddMetrics(cmd.MetricIds));
        }

        private void When(RemoveMetrics cmd)
        {
            InternalAct(cmd.Id, aggregate => aggregate.RemoveTeamMembers(cmd.MetricIds));
        }

        private void When(SuspendProject cmd)
        {
            InternalAct(cmd.Id, aggregate => aggregate.Suspend());
        }
// ReSharper restore UnusedMember.Local

        private void InternalAct(Guid id, Action<ProjectAggregate> action)
        {
            var aggregate = _repository.GetById<ProjectAggregate>(id);
            action(aggregate);
            _repository.Save(aggregate);
        }
    }
}