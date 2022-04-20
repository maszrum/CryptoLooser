using NUnit.Framework;
using MongoDB.Driver;
using CryptoLooser.Configuration;
using CryptoLooser.Core.Models;

namespace CryptoLooser.MongoDb.Tests;

[TestFixture]
public class AvailableDateRangesRepositoryFixture
{
    [Test]
    public async Task insert_three_different_but_query_should_return_one()
    {
        var configuration = new ConfigurationProvider("appsettings.test.json")
            .GetConfiguration<MongoDbConfiguration>(MongoDbConfiguration.Section);

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

        var readDateRanges = await DoOnTestDatabase(
            configuration,
            async connectionFactory =>
            {
                var repository = new AvailableDateRangesRepository(connectionFactory);

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

    private static async Task<T> DoOnTestDatabase<T>(
        MongoDbConfiguration configuration,
        Func<ConnectionFactory, Task<T>> action)
    {
        var connectionFactory = new ConnectionFactory(configuration);
        var databaseNamesCursor = await connectionFactory.Client.ListDatabaseNamesAsync();
        var databaseNames = await databaseNamesCursor.ToListAsync();

        if (databaseNames.Contains(configuration.Database))
        {
            await connectionFactory.Client.DropDatabaseAsync(configuration.Database);
        }

        connectionFactory = new ConnectionFactory(configuration);

        try
        {
            return await action(connectionFactory);
        }
        finally
        {
            await connectionFactory.Client.DropDatabaseAsync(configuration.Database);
        }
    }
}