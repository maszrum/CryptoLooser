namespace CryptoLooser.Core.Models;

public record CandlestickChartEntry(
    DateTime Timestamp,
    double OpeningPrice,
    double ClosingPrice,
    double HighestPrice,
    double LowestPrice,
    double GeneratedVolume);