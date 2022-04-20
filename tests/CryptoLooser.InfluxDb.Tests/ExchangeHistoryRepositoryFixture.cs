using CryptoLooser.Core.Models;
using NUnit.Framework;

namespace CryptoLooser.InfluxDb.Tests;

[TestFixture]
public class ExchangeHistoryRepositoryFixture
{
    [Test]
    public async Task something()
    {
        // TODO: move configuration to file
        var configuration = new InfluxDbConfiguration(
            "http://localhost:8086",
            "cryptotoken",
            "bucket",
            "maszrum");
        
        var connectionFactory = new ConnectionFactory(configuration);
        var repository = new ExchangeHistoryRepository(connectionFactory);
        
        var candlesticks = Enumerable
            .Range(1, 100)
            .Select(i =>
            {
                var candlestick = new CandlestickChartEntry(
                    Timestamp: new DateTime(2022, 3, 1)
                        .AddSeconds(i * (int)ChartResolution.FifteenMinutes)
                        .ToUniversalTime(),
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
    }
}