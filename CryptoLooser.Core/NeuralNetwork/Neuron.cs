using System.Collections.Immutable;
using System.Numerics;

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

        if (inputs.Length == 1)
        {
            var fastSum = (inputs[0] * _weights[0]) + _bias;
            return ActivationFunction(fastSum);
        }

        var vectorSize = Vector<double>.Count;
        var vectorCount = inputs.Length / vectorSize;
        var weightsSpan = _weights.AsSpan();

        var sum = _bias;

        for (var i = 0; i < vectorCount; i++)
        {
            var sliceFromIndex = i * vectorSize;

            var inputsSlice = inputs.Slice(sliceFromIndex, vectorSize);
            var weightsSlice = weightsSpan.Slice(sliceFromIndex, vectorSize);

            var inputsVector = new Vector<double>(inputsSlice);
            var weightsVector = new Vector<double>(weightsSlice);

            var sumVector = Vector.Multiply(inputsVector, weightsVector);
            sum += Vector.Sum(sumVector);
        }

        for (var i = vectorSize * vectorCount; i < inputs.Length; i++)
        {
            sum += _weights[i] * inputs[i];
        }

        return ActivationFunction(sum);
    }

    private static double ActivationFunction(double x) => Math.Tanh(x);
}
