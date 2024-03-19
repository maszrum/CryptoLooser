using CryptoLooser.Core;
using CryptoLooser.Core.Parsing;
using Serilog;

namespace CryptoLooser.Cli.Commands;

internal static class CommandHelpers
{
    public static async Task<MarketDataRow[]> LoadMarketData(
        string dataDirectory,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        var marketFilesLocation = Path.Combine("market-data", dataDirectory);

        logger.Information(
            "Reading and parsing market data files from location: {MarketFilesLocation}",
            marketFilesLocation);

        var parser = BinanceMarketDataFileParser.Create("ETHUSDT", marketFilesLocation);

        var marketData = await parser
            .GetMarketData(cancellationToken)
            .ToArrayAsync(cancellationToken);

        return marketData;
    }
}
