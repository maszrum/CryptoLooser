namespace CryptoLooser.ZondaExchange.ApiClient;

public record CandlestickChartEntry(
    DateTime Timestamp,
    decimal OpeningPrice,
    decimal ClosingPrice,
    decimal HighestPrice,
    decimal LowestPrice,
    decimal GeneratedVolume);