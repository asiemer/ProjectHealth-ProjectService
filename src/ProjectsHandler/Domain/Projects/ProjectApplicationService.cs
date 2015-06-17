using System;
using System.Linq;
using NServiceBus;
using Projects.Contracts.Commands;
using Projects.Contracts.Events;
using Projects.Services;

namespace Projects.Domain
{
    public interface IProjectApplicationService {
        void Execute(object command);
    }

    public class ProjectApplicationService : IProjectApplicationService
    {
        private readonly IRepository _repository;
        private readonly IMetricsProvider _metricsProvider;
        private readonly IBus _bus;

        public ProjectApplicationService(IRepository repository, IMetricsProvider metricsProvider, IBus bus)
        {
            _repository = repository;
            _metricsProvider = metricsProvider;
            _bus = bus;
        }

        public void Execute(object command)
        {
            RedirectToWhen.InvokeCommand(this, command);
        }

// ReSharper disable UnusedMember.Local
        private void When(CreateProject cmd)
        {
            var metrics = _metricsProvider.GetCompanyMetrics()
                .Select(x => new MetricInfo
                {
                    MetricId = x.Id,
                    IsDefault = x.IsDefault,
                    Weight = x.Weight,
                    Value = x.Value
                })
                .ToArray();
            InternalAct(cmd.Id, aggregate => aggregate.Create(cmd.Id, cmd.Name, metrics));
        }

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
            PublishPublicEvents(aggregate);
        }

        private void PublishPublicEvents(IPublisher aggregate)
        {
            foreach (var e in aggregate.GetPublicEvents())
                _bus.Publish(e);
            aggregate.ClearPublicEvents();
        }
    }
}