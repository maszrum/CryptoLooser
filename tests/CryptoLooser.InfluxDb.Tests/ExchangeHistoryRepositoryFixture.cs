using NUnit.Framework;
using CryptoLooser.Configuration;
using CryptoLooser.Core.Models;

namespace CryptoLooser.InfluxDb.Tests;

[TestFixture]
public class ExchangeHistoryRepositoryFixture
{
    [Test]
    public async Task check_if_written_data_is_equal_to_read_data()
    {
        var configuration = new ConfigurationProvider("appsettings.test.json")
            .GetConfiguration<InfluxDbConfiguration>(InfluxDbConfiguration.Section);

        /* generate sample data */
        var candlesticks = Enumerable
            .Range(0, 100)
            .Select(i =>
            {
                var candlestick = new CandlestickChartEntry(
                    Timestamp: new DateTime(2022, 3, 2, 13, 0, 0)
                        .AddSeconds(i * (int) ChartResolution.FifteenMinutes),
                    OpeningPrice: i + 1,
                    ClosingPrice: i + 2,
                    HighestPrice: i + 3,
                    LowestPrice: i + 4,
                    GeneratedVolume: i + 5);

                return candlestick;
            })
            .ToArray();

        var readCandlesticks = await DoOnTestBucket(
            configuration,
            async connectionFactory =>
            {
                var repository = new ExchangeHistoryRepository(connectionFactory);

                await repository.WriteCandlesticks(
                    entries: candlesticks,
                    marketCode: MarketCode.Parse("BTC-PLN"),
                    resolution: ChartResolution.FifteenMinutes);

                /* read sample data */
                return await repository.ReadCandlesticks(
                    candlesticks.Min(c => c.Timestamp),
                    candlesticks.Max(c => c.Timestamp).AddSeconds(1),
                    MarketCode.Parse("BTC-PLN"),
                    ChartResolution.FifteenMinutes);
            });

        /* check if correct */
        Assert.That(readCandlesticks.Entries, Has.Length.EqualTo(100));

        Assert.That(readCandlesticks.Resolution, Is.EqualTo(ChartResolution.FifteenMinutes));
        Assert.That(readCandlesticks.MarketCode.ToString(), Is.EqualTo("BTC-PLN"));

        Assert.That(readCandlesticks.Entries[0].Timestamp, Is.EqualTo(new DateTime(2022, 3, 2, 13, 0, 0)));
        Assert.That(readCandlesticks.Entries[99].Timestamp, Is.EqualTo(new DateTime(2022, 3, 3, 13, 45, 0)));
    }

    private static async Task<T> DoOnTestBucket<T>(
        InfluxDbConfiguration configuration,
        Func<ConnectionFactory, Task<T>> action)
    {
        var connectionFactory = await ConnectionFactory.Create(configuration);

        var bucketsApi = connectionFactory.OpenBucketsApi().Api;

        var existingBucket = await bucketsApi.FindBucketByNameAsync(configuration.Bucket);
        if (existingBucket is not null)
        {
            await bucketsApi.DeleteBucketAsync(existingBucket);
        }

        var testBucket = await bucketsApi.CreateBucketAsync(configuration.Bucket, connectionFactory.OrganizationId);

        try
        {
            return await action(connectionFactory);
        }
        finally
        {
            await bucketsApi.DeleteBucketAsync(testBucket);
        }
    }
}