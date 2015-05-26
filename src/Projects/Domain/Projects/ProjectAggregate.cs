using System;
using Projects.Contracts.Events;

namespace Projects.Domain
{
    public class ProjectAggregate : AggregateBase<ProjectState>
    {
        public ProjectAggregate(ProjectState state) : base(state)
        {
        }

        public void Create(Guid id, string name)
        {
            if(State.Version > 0)
                throw new InvalidOperationException("Cannot recreate an existing project");
            if(string.IsNullOrWhiteSpace(name))
                throw new InvalidOperationException("Cannot create a new project without valid name");

            Apply(new ProjectCreated
            {
                Id = id,
                Name = name
            });
        }

        public void SetCem(Guid staffId)
        {
            Apply(new CemSet{Id = State.Id});
        }

        public void SetPm(Guid staffId)
        {
            throw new NotImplementedException();
        }

        public void AddTeamMembers(Guid[] staffIds)
        {
            throw new NotImplementedException();
        }

        public void RemoveTeamMembers(Guid[] staffIds)
        {
            throw new NotImplementedException();
        }

        public void AddMetrics(Guid[] metricIds)
        {
            throw new NotImplementedException();
        }

        public void RemoveMetrics(Guid[] metricIds)
        {
            throw new NotImplementedException();
        }
    }
}