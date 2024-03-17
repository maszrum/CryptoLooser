using System.Collections.Immutable;

namespace CryptoLooser.Core.NeuralNetwork;

internal class Neuron(ImmutableArray<double> weights, double bias)
{
    private readonly ImmutableArray<double> _weights = weights;
    private readonly double _bias = bias;

    public int InputsCount => _weights.Length;

    public double CalculateOutput(ReadOnlySpan<double> inputs)
    {
        if (inputs.Length != _weights.Length)
        {
            throw new ArgumentException(
                "Number of inputs does not match number of weights.",
                nameof(inputs));
        }

        var sum = 0.0d;

        for (var i = 0; i < _weights.Length; i++)
        {
            sum += inputs[i] * _weights[i];
        }

        sum += _bias;

        return Sigmoid(sum);
    }

    private static double Sigmoid(double x) => 1.0d / (1.0d + Math.Exp(-x));
}
