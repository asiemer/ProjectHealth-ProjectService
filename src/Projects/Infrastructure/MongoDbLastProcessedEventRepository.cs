using System.Threading.Tasks;
using EventStore.ClientAPI;
using MongoDB.Driver;

namespace Projects.Infrastructure
{
    public class MongoDbLastProcessedEventRepository : ILastProcessedEventRepository
    {
        private readonly string _connectionString;
        private readonly string _databaseName;
        private static IMongoCollection<LastProcessedPosition> _collection;

        public MongoDbLastProcessedEventRepository(string connectionString, string databaseName)
        {
            _connectionString = connectionString;
            _databaseName = databaseName;
        }

        public async Task<Position> Get()
        {
            var coll = GetCollection();
            var pos = await coll.Find(x => x.Id == 1).FirstOrDefaultAsync();
            if (pos == null)
                return Position.Start;
            return new Position(pos.CommitPosition, pos.PreparePosition);
        }

        public async Task Save(Position lastPosition)
        {
            var pos = new LastProcessedPosition
            {
                CommitPosition = lastPosition.CommitPosition,
                PreparePosition = lastPosition.PreparePosition
            };
            var coll = GetCollection();
            var lastPos = await coll.Find(x => x.Id == 1).FirstOrDefaultAsync();
            if (lastPos == null)
                await coll.InsertOneAsync(pos);
            else
                await coll.ReplaceOneAsync(x => x.Id == 1, pos);
        }

        private IMongoCollection<LastProcessedPosition> GetCollection()
        {
            if (_collection != null) return _collection;
            var client = new MongoClient(_connectionString);
            var database = client.GetDatabase(_databaseName);
            _collection = database.GetCollection<LastProcessedPosition>(typeof(LastProcessedPosition).Name);
            return _collection;
        }
    }

    public class LastProcessedPosition
    {
        public LastProcessedPosition()
        {
            Id = 1;
        }

        public int Id { get; set; }
        public long CommitPosition { get; set; }
        public long PreparePosition { get; set; }
    }
}