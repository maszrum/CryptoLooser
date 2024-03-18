using System.Reflection;

namespace CryptoLooser.Core.NeuralNetwork;

public readonly ref struct NormalizedNeuralNetworkInput(
    ReadOnlySpan<double> closePrice,
    ReadOnlySpan<double> priceDifference,
    ReadOnlySpan<double> candlestickHeight,
    ReadOnlySpan<double> volume,
    ReadOnlySpan<double> tradesCount)
{
    public ReadOnlySpan<double> ClosePrice { get; } = closePrice;

    public ReadOnlySpan<double> PriceDifference { get; } = priceDifference;

    public ReadOnlySpan<double> CandlestickHeight { get; } = candlestickHeight;

    public ReadOnlySpan<double> Volume { get; } = volume;

    public ReadOnlySpan<double> TradesCount { get; } = tradesCount;

    public static int GetValuesCount(int seriesCount) =>
        seriesCount * GetPropertiesCount(typeof(NormalizedNeuralNetworkInput));

    private static int GetPropertiesCount(Type type)
    {
        return type
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Length;
    }
}
