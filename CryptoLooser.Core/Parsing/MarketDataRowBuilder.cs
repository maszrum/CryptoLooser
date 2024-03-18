using System.Globalization;

namespace CryptoLooser.Core.Parsing;

internal class MarketDataRowBuilder
{
    private int _validationCount;
    private int _validationSum;

    private DateTime _openTime;
    private DateTime _closeTime;
    private double _openPrice;
    private double _closePrice;
    private double _highPrice;
    private double _lowPrice;
    private double _volume;
    private double _quoteVolume;
    private long _tradesCount;
    private double _takerBuyVolume;
    private double _takerBuyQuoteVolume;

    public MarketDataRow Build()
    {
        if (_validationCount != 11 || _validationSum != 55)
        {
            throw new InvalidOperationException("Incomplete data.");
        }

        var marketData = new MarketDataRow(
            _openTime,
            _closeTime,
            _openPrice,
            _closePrice,
            _highPrice,
            _lowPrice,
            _volume,
            _quoteVolume,
            _tradesCount,
            _takerBuyVolume,
            _takerBuyQuoteVolume);

        _validationSum = 0;
        _validationCount = 0;

        return marketData;
    }

    public void FeedData(int column, ReadOnlySpan<char> value)
    {
        var skipIndex = false;

        switch (column)
        {
            case 0:
                _openTime = ToDateTime(value);
                break;
            case 1:
                _openPrice = ToDouble(value);
                break;
            case 2:
                _highPrice = ToDouble(value);
                break;
            case 3:
                _lowPrice = ToDouble(value);
                break;
            case 4:
                _closePrice = ToDouble(value);
                break;
            case 5:
                _volume = ToDouble(value);
                break;
            case 6:
                _closeTime = ToDateTime(value);
                break;
            case 7:
                _quoteVolume = ToDouble(value);
                break;
            case 8:
                _tradesCount = ToLong(value);
                break;
            case 9:
                _takerBuyVolume = ToDouble(value);
                break;
            case 10:
                _takerBuyQuoteVolume = ToDouble(value);
                break;
            default:
                skipIndex = true;
                break;
        }

        if (!skipIndex)
        {
            _validationCount++;
            _validationSum += column;
        }
    }

    private static DateTime ToDateTime(ReadOnlySpan<char> input)
    {
        var unixMilliseconds = long.Parse(input, NumberStyles.None, CultureInfo.InvariantCulture);
        var dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(unixMilliseconds);
        return dateTimeOffset.UtcDateTime;
    }

    private static double ToDouble(ReadOnlySpan<char> input) =>
        double.Parse(
            input,
            NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign,
            CultureInfo.InvariantCulture);

    private static long ToLong(ReadOnlySpan<char> input) =>
        long.Parse(input, NumberStyles.None, CultureInfo.InvariantCulture);
}
