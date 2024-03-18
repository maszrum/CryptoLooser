namespace CryptoLooser.Core.Parsing;

public readonly record struct ChromosomeParsingOutput(
    int HiddenLayerNeuronsCount,
    int SeriesSize,
    ReadOnlyMemory<double> Genes);
