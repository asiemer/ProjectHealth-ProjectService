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
    }
}