using CryptoLooser.Core.Parsing;
using NUnit.Framework;

namespace CryptoLooser.Core.UnitTests;

[TestFixture]
public class BinanceMarketDataFileParserTests
{
    [Test]
    public async Task todo_test_name()
    {
        var marketFilesLocation = Path.Combine(
            TestContext.CurrentContext.TestDirectory,
            "market-data",
            "ethusdt");

        var parser = BinanceMarketDataFileParser.Create("ETHUSDT", marketFilesLocation);

        var marketData = await parser.GetMarketData().ToArrayAsync();

        Assert.That(marketData, Has.Length.EqualTo(168));
    }
}
