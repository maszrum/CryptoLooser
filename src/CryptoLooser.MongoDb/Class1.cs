using CryptoLooser.Core.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CryptoLooser.MongoDb;

public class Class1
{
    public async Task test()
    {
        // TODO: move connection string to file
        var client = new MongoClient("mongodb://crypto:YOUlooser@localhost:27017");
        var database = client.GetDatabase("cryptolooser");
        var collection = database.GetCollection<BsonDocument>("dateranges");

        var dateRange = new DateRange(
            new DateTime(2022, 2, 12, 12, 0, 0),
            new DateTime(2022, 2, 26, 16, 0, 0),
            MarketCode.Parse("PLN-ETH"),
            ChartResolution.FiveMinutes);

        var dateRangeBson = new DateRangeToBsonConverter().Convert(dateRange);

        await collection.InsertOneAsync(dateRangeBson);
    }
}