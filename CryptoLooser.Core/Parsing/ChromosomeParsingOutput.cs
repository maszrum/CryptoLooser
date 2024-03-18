using CryptoLooser.Core.NeuralNetwork;

namespace CryptoLooser.Core.Parsing;

public readonly record struct ChromosomeParsingOutput(
    int HiddenLayerNeuronsCount,
    NeuralNetworkInputLengths InputLengths,
    ReadOnlyMemory<double> Genes);
