namespace CryptoLooser.Cli;

internal class NeuralNetworkSettings
{
    public const string ConfigurationKey = "NeuralNetwork";

    public required int HiddenLayerNeuronsCount { get; init; }

    public required int SeriesSize { get; init; }
}
