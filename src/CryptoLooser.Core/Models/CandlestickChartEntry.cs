namespace CryptoLooser.Core.Models;

public record CandlestickChartEntry(
    DateTime Timestamp,
    decimal OpeningPrice,
    decimal ClosingPrice,
    decimal HighestPrice,
    decimal LowestPrice,
    decimal GeneratedVolume);