using MongoDB.Bson;
using MongoDB.Driver;

namespace CryptoLooser.MongoDb;

public class ConnectionFactory
{
    private IMongoDatabase? _database;
    private MongoClient? _client;

    private readonly MongoDbConfiguration _configuration;

    public ConnectionFactory(MongoDbConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public MongoClient Client => _client ?? OpenConnectionOrGetExising();

    public IMongoCollection<BsonDocument> GetDateRangesDocument()
    {
        var database = OpenDatabaseOrGetExistingConnection();
        return database.GetCollection<BsonDocument>("dateranges");
    }

    private IMongoDatabase OpenDatabaseOrGetExistingConnection()
    {
        if (_database is null)
        {
            var client = OpenConnectionOrGetExising();
            _database = client.GetDatabase(_configuration.Database);
        }

        return _database;
    }
    
    private MongoClient OpenConnectionOrGetExising()
    {
        return _client ??= new MongoClient(_configuration.ConnectionString);
    }
}