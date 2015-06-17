using System;
using System.Collections.Generic;
using System.Linq;
using Projects.Contracts.Enums;
using Projects.Contracts.Events;

namespace Projects.Domain
{
    public class MetricState
    {
        public Guid Id { get; set; }
        public bool IsDefault { get; set; }
        public int Weight { get; set; }
        public int Value { get; set; }
    }

    public class ProjectScore
    {
        public int Red { get; set; }
        public int Yellow { get; set; }
        public int Green { get; set; }
    }

    public class ProjectState : IState
    {
        public ProjectState(IEnumerable<object> events)
        {
            TeamMembers = new List<Guid>();
            Metrics = new List<MetricState>();
            foreach (var @event in events)
                Modify(@event);
        }

        public Guid Id { get; private set; }
        public int Version { get; private set; }
        public ProjectStatus Status { get; set; }
        public List<Guid> TeamMembers { get; set; }
        public List<MetricState> Metrics { get; set; }
        public ProjectScore Score { get; set; }

        public void Modify(object e)
        {
            Version++;
            RedirectToWhen.InvokeEventOptional(this, e);
        }

        // ReSharper disable UnusedMember.Local
        private void When(ProjectCreated e)
        {
            Id = e.Id;
            Status = ProjectStatus.Active;
            Metrics = (e.DefaultMetrics ?? new MetricInfo[0])
                .Select(x => new MetricState {Id = x.MetricId, IsDefault = x.IsDefault, Weight = x.Weight, Value = x.Value})
                .ToList();
        }

        private void When(TeamMembersAdded e)
        {
            TeamMembers.AddRange(e.StaffIds);
        }

        private void When(TeamMembersRemoved e)
        {
            foreach (var staffId in e.StaffIds)
                TeamMembers.Remove(staffId);
        }

        private void When(MetricsAdded e)
        {
            Metrics.AddRange(e.Metrics.Select(x => new MetricState{Id = x.MetricId, IsDefault = x.IsDefault}));
        }

        private void When(MetricsRemoved e)
        {
            foreach (var metricInfo in e.Metrics)
            {
                var m = Metrics.SingleOrDefault(x => x.Id == metricInfo.MetricId);
                Metrics.Remove(m);
            }
        }

        private void When(ProjectSuspended e)
        {
            Status = ProjectStatus.Suspended;
        }
        // ReSharper restore UnusedMember.Local
    }
}