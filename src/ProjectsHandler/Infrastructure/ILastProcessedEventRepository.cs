using System.Threading.Tasks;
using EventStore.ClientAPI;

namespace Projects.Infrastructure
{
    public interface ILastProcessedEventRepository
    {
        Task<Position> Get();
        Task Save(Position lastPosition);
    }
}