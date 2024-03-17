namespace CryptoLooser.Core;

public record MarketDataRow(
    DateTime OpenTime,
    DateTime CloseTime,
    double OpenPrice,
    double ClosePrice,
    double HighPrice,
    double LowPrice,
    double Volume,
    double QuoteVolume,
    long TradesCount,
    double TakerBuyVolume,
    double TakerBuyQuoteVolume);
