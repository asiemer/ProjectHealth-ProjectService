using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using Projects.Infrastructure;
using Projects.ReadModel.Views;

namespace Projects.ReadModel.Providers
{
    public interface IProjectsProvider : IProvider<ProjectView>
    {
        Task<IEnumerable<ProjectView>> Get(string name);
    }

    public class ProjectsProvider : MongoDbProvider<ProjectView>, IProjectsProvider
    {
        public ProjectsProvider(IApplicationSettings applicationSettings) : base(applicationSettings)
        {
        }

        public async Task<IEnumerable<ProjectView>> Get(string name)
        {
            var collection = GetCollection();
            var items = await collection.Find(x => x.Name.StartsWith(name)).ToListAsync();
            return items;
        }
    }
}