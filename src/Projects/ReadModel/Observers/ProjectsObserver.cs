using System.Linq;
using System.Threading.Tasks;
using Projects.Contracts.Enums;
using Projects.Contracts.Events;
using Projects.Infrastructure;
using Projects.ReadModel.Views;

namespace Projects.ReadModel.Observers
{
    public class ProjectsObserver
    {
        private readonly IProjectionWriter<ProjectView> _writer;

        public ProjectsObserver(IProjectionWriter<ProjectView> writer)
        {
            _writer = writer;
        }

        public async Task When(ProjectCreated e)
        {
            await _writer.Add(e.Id, new ProjectView
            {
                Id = e.Id,
                Name = e.Name,
                Status = ProjectStatus.Active
            });
        }

        public async Task When(CemSet e)
        {
            await _writer.Update(e.Id, x => x.CemId = e.StaffId);
        }

        public async Task When(PmSet e)
        {
            await _writer.Update(e.Id, x => x.PmId = e.StaffId);
        }

        public async Task When(TeamMembersAdded e)
        {
            await _writer.Update(e.Id, x => x.Team.AddRange(e.StaffIds));
        }

        public async Task When(TeamMembersRemoved e)
        {
            await _writer.Update(e.Id, x => x.Team.RemoveAll(id => e.StaffIds.Contains(id)));
        }
    }
}