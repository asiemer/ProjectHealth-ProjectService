using System;
using System.Collections.Generic;
using Projects.Contracts.Enums;
using Projects.Contracts.Events;

namespace Projects.Domain
{
    public class ProjectState : IState
    {
        public ProjectState(IEnumerable<object> events)
        {
            throw new NotImplementedException();
        }

        public Guid Id { get; private set; }
        public int Version { get; private set; }
        public ProjectStatus Status { get; set; }

        public void Modify(object e)
        {
            Version++;
            RedirectToWhen.InvokeEventOptional(this, e);
        }

        private void When(ProjectCreated e)
        {
            Id = e.Id;
            Status = ProjectStatus.Draft;
        }
    }
}