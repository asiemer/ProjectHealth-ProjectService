using System;
using System.Linq;
using Projects.Contracts.Enums;
using Projects.Contracts.Events;

namespace Projects.Domain
{
    public class ProjectAggregate : AggregateBase<ProjectState>
    {
        public ProjectAggregate(ProjectState state) : base(state)
        {
        }

        public void Create(Guid id, string name, MetricInfo[] defaultMetrics)
        {
            if(State.Version > 0)
                throw new InvalidOperationException("Cannot recreate an existing project");
            if(string.IsNullOrWhiteSpace(name))
                throw new InvalidOperationException("Cannot create a new project without valid name");

            Apply(new ProjectCreated
            {
                Id = id,
                Name = name,
                DefaultMetrics = defaultMetrics
            });
            UpdateScore();
        }

        private void UpdateScore()
        {
            var score = new ProjectScore
            {
                Red = State.Metrics.Where(x => x.Value == -1).Sum(m => m.Weight),
                Yellow = State.Metrics.Where(x => x.Value == 0).Sum(m => m.Weight),
                Green = State.Metrics.Where(x => x.Value == 1).Sum(m => m.Weight)
            };
            State.Score = score;
            PublishPublicEvent(new ProjectScoreChanged
            {
                Id = State.Id,
                Red = State.Score.Red,
                Yellow = State.Score.Yellow,
                Green = State.Score.Green
            });
        }

        public void SetCem(Guid staffId)
        {
            ThrowIfNotExists("set the CEM on");

            Apply(new CemSet{Id = State.Id, StaffId = staffId});
        }

        public void SetPm(Guid staffId)
        {
            ThrowIfNotExists("set the PM on");

            Apply(new PmSet { Id = State.Id, StaffId = staffId });
        }

        public void AddTeamMembers(Guid[] staffIds)
        {
            ThrowIfNotExists("add team members to");
            if (staffIds == null || staffIds.Any() == false)
                throw new InvalidOperationException("Cannot add null or empty set of team members");

            Apply(new TeamMembersAdded
            {
                Id = State.Id,
                StaffIds = staffIds.Except(State.TeamMembers).ToArray()
            });
        }

        public void RemoveTeamMembers(Guid[] staffIds)
        {
            ThrowIfNotExists("remove team members from");
            if (staffIds == null || staffIds.Any() == false)
                throw new InvalidOperationException("Cannot remove null or empty set of team members");

            Apply(new TeamMembersRemoved
            {
                Id = State.Id,
                StaffIds = staffIds.Intersect(State.TeamMembers).ToArray()
            });
        }

        public void AddMetrics(Guid[] metricIds)
        {
            ThrowIfNotExists("add metrics to");
            if (metricIds == null || metricIds.Any() == false)
                throw new InvalidOperationException("Cannot add null or empty set of metrics");

            var trulyNewMetricIds = metricIds.Except(State.Metrics.Select(x => x.Id));
            Apply(new MetricsAdded
            {
                Id = State.Id,
                Metrics = trulyNewMetricIds.Select(x => new MetricInfo{MetricId = x, IsDefault = false}).ToArray()
            });
            UpdateScore();
        }

        public void RemoveMetrics(Guid[] metricIds)
        {
            ThrowIfNotExists("remove metrics from");
            if (metricIds == null || metricIds.Any() == false)
                throw new InvalidOperationException("Cannot remove null or empty set of metrics");

            var metricIdsThatTrulyExist = metricIds.Intersect(State.Metrics.Select(x => x.Id));
            Apply(new MetricsRemoved
            {
                Id = State.Id,
                Metrics = metricIdsThatTrulyExist.Select(x => new MetricInfo{MetricId = x, IsDefault = false}).ToArray()
            });
        }

        public void Suspend()
        {
            ThrowIfNotExists("suspend");
            if (State.Status != ProjectStatus.Active)
                throw new InvalidOperationException("Cannot Suspend a project that is not is status Active");

            Apply(new ProjectSuspended { Id = State.Id });
        }

        private void ThrowIfNotExists(string message)
        {
            if (State.Version == 0)
                throw new InvalidOperationException(string.Format("Cannot {0} a project that has not been created", message));
        }
    }
}