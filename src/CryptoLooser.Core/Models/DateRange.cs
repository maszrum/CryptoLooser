namespace CryptoLooser.Core.Models;

public record DateRange
{
    public DateRange(
        DateTime from,
        DateTime to,
        MarketCode marketCode,
        ChartResolution resolution)
    {
        if (from >= to)
        {
            throw new ArgumentException(
                "From date must be before to date.", nameof(from));
        }

        From = from;
        To = to;
        MarketCode = marketCode;
        Resolution = resolution;
    }

    public DateTime From { get; }

    public DateTime To { get; }

    public MarketCode MarketCode { get; }

    public ChartResolution Resolution { get; }
    
    public TimeSpan GetLength() => To - From;
}