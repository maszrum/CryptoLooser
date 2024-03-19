using System.Collections.Immutable;

namespace CryptoLooser.Core.NeuralNetwork;

internal class Layer(ImmutableArray<Neuron> neurons)
{
    private readonly ImmutableArray<Neuron> _neurons = neurons;
    private readonly double[] _result = new double[neurons.Length];
    private readonly bool _allNeuronsSingleInput = neurons.All(n => n.InputsCount == 1);

    public ReadOnlyMemory<double> CalculateOutputs(ReadOnlyMemory<double> inputs, bool chunkInput)
    {
        return chunkInput
            ? CalculateOutputsChunkInput(inputs)
            : CalculateOutputs(inputs);
    }

    private ReadOnlyMemory<double> CalculateOutputs(ReadOnlyMemory<double> inputs)
    {
        if (_neurons.Length == 1)
        {
            _result[0] = _neurons[0].CalculateOutput(inputs.Span);
        }
        else
        {
            for (var i = 0; i < _neurons.Length; i++)
            {
                _result[i] = _neurons[i].CalculateOutput(inputs.Span);
            }
        }

        return _result;
    }

    private ReadOnlyMemory<double> CalculateOutputsChunkInput(ReadOnlyMemory<double> inputs)
    {
        if (_allNeuronsSingleInput)
        {
            // fast path
            for (var i = 0; i < _neurons.Length; i++)
            {
                _result[i] = _neurons[i].CalculateOutput(inputs.Span.Slice(i, 1));
            }

            return _result;
        }

        var consumedInputs = 0;

        for (var i = 0; i < _neurons.Length; i++)
        {
            var neuron = _neurons[i];
            var neuronInputs = inputs.Slice(consumedInputs, neuron.InputsCount);
            _result[i] = _neurons[i].CalculateOutput(neuronInputs.Span);

            consumedInputs += neuron.InputsCount;
        }

        return _result;
    }
}
