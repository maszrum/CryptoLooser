namespace CryptoLooser.Core.NeuralNetwork;

public class MarketDataCollector(int seriesSize)
{
    private readonly CircularBuffer<double> _closePrices = new(seriesSize);
    private readonly CircularBuffer<double> _priceDifferences = new(seriesSize);
    private readonly CircularBuffer<double> _candlestickHeights = new(seriesSize);
    private readonly CircularBuffer<double> _volumes = new(seriesSize);
    private readonly CircularBuffer<double> _tradesCount = new(seriesSize);

    private readonly double[] _normalizedClosePrices = new double[seriesSize];
    private readonly double[] _normalizedPriceDifferences = new double[seriesSize];
    private readonly double[] _normalizedCandlestickHeights = new double[seriesSize];
    private readonly double[] _normalizedVolumes = new double[seriesSize];
    private readonly double[] _normalizedTradesCount = new double[seriesSize];

    public bool Feed(MarketDataRow marketData)
    {
        _closePrices.Append(marketData.ClosePrice);
        _priceDifferences.Append(marketData.ClosePrice - marketData.OpenPrice);
        _candlestickHeights.Append(marketData.HighPrice - marketData.LowPrice);
        _volumes.Append(marketData.Volume);
        _tradesCount.Append(marketData.TradesCount);

        if (_closePrices.IsFull)
        {
            NormalizeValues();
        }

        return _closePrices.IsFull;
    }

    public NormalizedNeuralNetworkInput GetNormalizedValues()
    {
        return new NormalizedNeuralNetworkInput(
            _normalizedClosePrices,
            _normalizedPriceDifferences,
            _normalizedCandlestickHeights,
            _normalizedVolumes,
            _normalizedTradesCount);
    }

    private void NormalizeValues()
    {
        NormalizeSingleSeries(_closePrices, _normalizedClosePrices);
        NormalizeDifference(_priceDifferences, _normalizedPriceDifferences);
        NormalizeSingleSeries(_candlestickHeights, _normalizedCandlestickHeights);
        NormalizeSingleSeries(_volumes, _normalizedVolumes);
        NormalizeSingleSeries(_tradesCount, _normalizedTradesCount);
    }

    private static void NormalizeSingleSeries(CircularBuffer<double> source, double[] destination)
    {
        source.CopyTo(destination);
        var maxValue = destination.Max();
        var minValue = destination.Min();
        var diff = maxValue - minValue;

        for (var i = 0; i < destination.Length; i++)
        {
            destination[i] -= minValue;
            destination[i] /= diff;
        }
    }

    // ReSharper disable once UnusedMember.Local
    private static void NormalizeSingleSeriesWithCommonMinMax(
        CircularBuffer<double> source,
        double[] destination,
        double minValue,
        double maxValue)
    {
        source.CopyTo(destination);
        var diff = maxValue - minValue;

        for (var i = 0; i < destination.Length; i++)
        {
            destination[i] -= minValue;
            destination[i] /= diff;
        }
    }

    private static void NormalizeDifference(CircularBuffer<double> source, double[] destination)
    {
        source.CopyTo(destination);
        var maxAbs = Math.Abs(destination.MaxBy(Math.Abs));

        for (var i = 0; i < destination.Length; i++)
        {
            destination[i] /= maxAbs;
        }
    }
}
