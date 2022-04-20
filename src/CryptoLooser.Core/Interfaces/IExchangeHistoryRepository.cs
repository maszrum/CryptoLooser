using CryptoLooser.Core.Models;

namespace CryptoLooser.Core.Interfaces;

public interface IExchangeHistoryRepository
{
    Task WriteCandlestick(
        CandlestickChartEntry entry,
        MarketCode marketCode,
        ChartResolution resolution);

    Task WriteCandlesticks(
        IEnumerable<CandlestickChartEntry> entries,
        MarketCode marketCode,
        ChartResolution resolution);

    Task<CandlestickChart> ReadCandlesticks(
        DateTime from,
        DateTime to,
        MarketCode marketCode,
        ChartResolution resolution);
}