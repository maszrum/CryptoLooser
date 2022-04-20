using System.Collections.Immutable;

namespace CryptoLooser.Core.Models;

public class CandlestickChart
{
    public CandlestickChart(
        MarketCode marketCode,
        ChartResolution resolution,
        ImmutableArray<CandlestickChartEntry> entries)
    {
        MarketCode = marketCode;
        Resolution = resolution;
        Entries = entries;
    }

    public MarketCode MarketCode { get; }

    public ChartResolution Resolution { get; }

    public ImmutableArray<CandlestickChartEntry> Entries { get; }
}