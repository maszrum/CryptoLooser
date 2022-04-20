using NUnit.Framework;
using MongoDB.Bson;
using MongoDB.Driver;
using CryptoLooser.Core.Models;

namespace CryptoLooser.MongoDb.Tests;

[TestFixture]
public class AvailableDateRangesRepositoryFixture
{
    [Test]
    public async Task insert_three_different_but_query_should_return_one()
    {
        // TODO: move connection string to file
        var configuration = new MongoDbConfiguration(
            ConnectionString: "mongodb://crypto:YOUlooser@localhost:27017",
            Database: "cryptolooser");

        var connectionFactory = new ConnectionFactory(configuration);
        var repository = new AvailableDateRangesRepository(connectionFactory);

        var dateRangeOne = new DateRange(
            new DateTime(2022, 2, 12, 12, 0, 0),
            new DateTime(2022, 2, 26, 16, 0, 0),
            MarketCode.Parse("PLN-ETH"),
            ChartResolution.FiveMinutes);

        var dateRangeTwo = new DateRange(
            new DateTime(2022, 2, 12, 12, 0, 0),
            new DateTime(2022, 2, 26, 16, 0, 0),
            MarketCode.Parse("PLN-BTC"),
            ChartResolution.FiveMinutes);

        var dateRangeThree = new DateRange(
            new DateTime(2022, 2, 12, 12, 0, 0),
            new DateTime(2022, 2, 26, 16, 0, 0),
            MarketCode.Parse("PLN-ETH"),
            ChartResolution.OneDay);

        var readDateRanges = await ClearBeforeAndAfter(
            connectionFactory.GetDateRangesDocument(),
            async () =>
            {
                await repository.InsertAvailableDateRage(dateRangeOne);
                await repository.InsertAvailableDateRage(dateRangeTwo);
                await repository.InsertAvailableDateRage(dateRangeThree);

                var dateRanges = await repository.GetAvailableDateRanges(
                    MarketCode.Parse("PLN-ETH"),
                    ChartResolution.FiveMinutes);

                return dateRanges;
            });

        Assert.That(readDateRanges, Has.Length.EqualTo(1));
        Assert.That(readDateRanges[0], Is.EqualTo(dateRangeOne));
    }

    private static async Task<T> ClearBeforeAndAfter<T>(
        IMongoCollection<BsonDocument> collection,
        Func<Task<T>> action)
    {
        await RemoveAll(collection);

        try
        {
            return await action();
        }
        finally
        {
            await RemoveAll(collection);
        }
    }

    private static async Task RemoveAll(IMongoCollection<BsonDocument> collection)
    {
        var filter = Builders<BsonDocument>.Filter.Empty;
        await collection.DeleteManyAsync(filter);
    }
}