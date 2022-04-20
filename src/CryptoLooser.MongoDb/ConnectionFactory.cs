using MongoDB.Bson;
using MongoDB.Driver;

namespace CryptoLooser.MongoDb;

public class ConnectionFactory
{
    private static IMongoDatabase? _database;

    private readonly MongoDbConfiguration _configuration;

    public ConnectionFactory(MongoDbConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IMongoCollection<BsonDocument> GetDateRangesDocument()
    {
        var database = OpenConnectionOrGetExisting();
        return database.GetCollection<BsonDocument>("dateranges");
    }

    private IMongoDatabase OpenConnectionOrGetExisting()
    {
        if (_database is null)
        {
            var client = new MongoClient(_configuration.ConnectionString);
            _database = client.GetDatabase(_configuration.Database);
        }

        return _database;
    }
}