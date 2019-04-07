#region Usings

using System.Threading.Tasks;
using CompositeApplicationFramework.Base;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Clusters;

#endregion

namespace CompositeApplicationFramework.DataAccess
{
    public class MongoDbConnector : DisposableBase
    {
        private MongoClient _client;

        public MongoDbConnector()
        {
        }

        public MongoDbConnector(string dbName)
        {
            DbName = dbName;
        }

        public MongoDbConnector(string dbName, string hostname, int port)
            : this(dbName)
        {
            Hostname = hostname;

            Port = port;
        }

        public MongoDbConnector(IMongoDatabase database)
        {
            Db = database;
        }

        public bool Connected => _client != null && _client.Cluster.Description.State == ClusterState.Connected;
        public int ProcessId { get; internal set; }
        public string DbName { get; } = "Default";
        public string Hostname { get; } = "localhost";
        public int Port { get; } = 27017;
        public IMongoDatabase Db { get; private set; }

        public void Connect()
        {
            if (Connected) return;

            _client = new MongoClient($"mongodb://{Hostname}:{Port}");

            Db = _client.GetDatabase(DbName);
        }

        public async Task ShutdownAsync()
        {
            var cmd = BsonDocument.Parse("{shutdown : 1, timeoutSecs : 5}");

            var adminDb = _client.GetDatabase("admin");

            try
            {
                await adminDb.RunCommandAsync<BsonDocument>(cmd);
            }
            catch (MongoConnectionException)
            {
            }
        }

        protected override async void DisposeHandler()
        {
            await ShutdownAsync();
        }
    }
}