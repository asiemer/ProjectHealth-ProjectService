using NServiceBus;
using Projects.Contracts.Commands;
using Projects.Domain;

namespace ProjectsHandler
{
    public class ProjectsHandler : 
        IHandleMessages<CreateProject>,
        IHandleMessages<SetCem>,
        IHandleMessages<SetPm>,
        IHandleMessages<AddTeamMembers>,
        IHandleMessages<RemoveTeamMembers>,
        IHandleMessages<AddMetrics>,
        IHandleMessages<RemoveMetrics>,
        IHandleMessages<SuspendProject>
    {
        private readonly IProjectApplicationService _projectApplicationService;

        public ProjectsHandler(IProjectApplicationService projectApplicationService)
        {
            _projectApplicationService = projectApplicationService;
        }

        public void Handle(CreateProject message)
        {
            _projectApplicationService.Execute(message);
        }

        public void Handle(SetCem message)
        {
            _projectApplicationService.Execute(message);
        }

        public void Handle(SetPm message)
        {
            _projectApplicationService.Execute(message);
        }

        public void Handle(AddTeamMembers message)
        {
            _projectApplicationService.Execute(message);
        }

        public void Handle(RemoveTeamMembers message)
        {
            _projectApplicationService.Execute(message);
        }

        public void Handle(AddMetrics message)
        {
            _projectApplicationService.Execute(message);
        }

        public void Handle(RemoveMetrics message)
        {
            _projectApplicationService.Execute(message);
        }

        public void Handle(SuspendProject message)
        {
            _projectApplicationService.Execute(message);
        }
    }
}
