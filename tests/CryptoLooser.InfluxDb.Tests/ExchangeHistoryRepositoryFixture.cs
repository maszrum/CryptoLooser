using NUnit.Framework;
using CryptoLooser.Core.Models;

namespace CryptoLooser.InfluxDb.Tests;

[TestFixture]
public class ExchangeHistoryRepositoryFixture
{
    [Test]
    public async Task check_if_written_data_is_equal_to_read_data()
    {
        // TODO: move configuration to file
        var configuration = new InfluxDbConfiguration(
            "http://localhost:8086",
            "cryptotoken",
            "bucket",
            "maszrum");

        var connectionFactory = new ConnectionFactory(configuration);
        var repository = new ExchangeHistoryRepository(connectionFactory);

        /* remove records if exist */
        using var deleteApi = connectionFactory.OpenDeleteApi();
        await deleteApi.Api.Delete(
            new DateTime(2000, 1, 1),
            new DateTime(2022, 12, 31),
            string.Empty,
            deleteApi.Bucket,
            deleteApi.Organization);

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

        await repository.WriteCandlesticks(
            entries: candlesticks,
            marketCode: MarketCode.Parse("BTC-PLN"),
            resolution: ChartResolution.FifteenMinutes);

        /* read sample data */
        var result = await repository.ReadCandlesticks(
            candlesticks.Min(c => c.Timestamp),
            candlesticks.Max(c => c.Timestamp).AddSeconds(1),
            MarketCode.Parse("BTC-PLN"),
            ChartResolution.FifteenMinutes);

        /* check if correct */
        Assert.That(result.Entries, Has.Length.EqualTo(100));

        Assert.That(result.Resolution, Is.EqualTo(ChartResolution.FifteenMinutes));
        Assert.That(result.MarketCode.ToString(), Is.EqualTo("BTC-PLN"));
        
        Assert.That(result.Entries[0].Timestamp, Is.EqualTo(new DateTime(2022, 3, 2, 13, 0, 0)));
        Assert.That(result.Entries[99].Timestamp, Is.EqualTo(new DateTime(2022, 3, 3, 13, 45, 0)));
    }
}