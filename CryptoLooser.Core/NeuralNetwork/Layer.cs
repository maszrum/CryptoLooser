using System.Buffers;
using System.Collections.Immutable;

namespace CryptoLooser.Core.NeuralNetwork;

internal class Layer(ImmutableArray<Neuron> neurons)
{
    private readonly ImmutableArray<Neuron> _neurons = neurons;
    private readonly double[] _result = new double[neurons.Length];

    public ReadOnlySpan<double> CalculateOutputs(ReadOnlySpan<double> inputs, bool chunkInput)
    {
        return chunkInput
            ? CalculateOutputsChunkInput(inputs)
            : CalculateOutputs(inputs);
    }

    private ReadOnlySpan<double> CalculateOutputs(ReadOnlySpan<double> inputs)
    {
        for (var i = 0; i < _neurons.Length; i++)
        {
            _result[i] = _neurons[i].CalculateOutput(inputs);
        }

        return _result;
    }

    private ReadOnlySpan<double> CalculateOutputsChunkInput(ReadOnlySpan<double> inputs)
    {
        var consumedInputs = 0;

        for (var i = 0; i < _neurons.Length; i++)
        {
            var neuron = _neurons[i];
            var neuronInputs = inputs.Slice(consumedInputs, neuron.InputsCount);
            _result[i] = _neurons[i].CalculateOutput(neuronInputs);

            consumedInputs += neuron.InputsCount;
        }

        return _result;
    }
}
