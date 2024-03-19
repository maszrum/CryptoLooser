using System.Collections.Immutable;

namespace CryptoLooser.Core.NeuralNetwork;

internal class MarketNeuralNetwork
{
    private const int OutputLayerNeuronsCount = 1;

    private readonly double[] _inputValues;

    public MarketNeuralNetwork(
        int inputsCount,
        int hiddenLayerNeuronsCount,
        ReadOnlySpan<double> weights,
        ReadOnlySpan<double> biases)
    {
        _inputValues = new double[inputsCount];

        var expectedWeightsCount = GetWeightsCount(inputsCount, hiddenLayerNeuronsCount);
        var expectedBiasesCount = GetBiasesCount(inputsCount, hiddenLayerNeuronsCount);

        if (weights.Length != expectedWeightsCount)
        {
            throw new ArgumentException(
                $"Provided invalid number of weights, expected count: {expectedWeightsCount}.",
                nameof(weights));
        }

        if (biases.Length != expectedBiasesCount)
        {
            throw new ArgumentException(
                $"Provided invalid number of biases, expected count: {expectedBiasesCount}.",
                nameof(biases));
        }

        var inputLayerNeurons = ImmutableArray.CreateBuilder<Neuron>();

        for (var i = 0; i < inputsCount; i++)
        {
            var neuron = new Neuron(ImmutableArray.Create(weights[i]), biases[i]);
            inputLayerNeurons.Add(neuron);
        }

        InputLayer = new Layer(inputLayerNeurons.ToImmutable());

        var hiddenLayerNeurons = ImmutableArray.CreateBuilder<Neuron>();
        var consumedWeights = inputsCount;

        for (var i = 0; i < hiddenLayerNeuronsCount; i++)
        {
            var neuronWeights = weights
                .Slice(consumedWeights, inputsCount)
                .ToImmutableArray();

            consumedWeights += inputsCount;

            var neuron = new Neuron(neuronWeights, biases[inputsCount + i]);

            hiddenLayerNeurons.Add(neuron);
        }

        HiddenLayer = new Layer(hiddenLayerNeurons.ToImmutable());

        var outputLayerNeuronWeights = weights
            .Slice(weights.Length - hiddenLayerNeuronsCount)
            .ToImmutableArray();

        var outputLayerNeuron = new Neuron(outputLayerNeuronWeights, biases[^1]);
        OutputLayer = new Layer(ImmutableArray.Create(outputLayerNeuron));
    }

    private Layer InputLayer { get; }

    private Layer HiddenLayer { get; }

    private Layer OutputLayer { get; }

    public double FeedForward(NormalizedNeuralNetworkInput input)
    {
        var index = 0;

        input.ClosePrice.CopyTo(_inputValues.AsSpan(index, input.ClosePrice.Length));
        index += input.ClosePrice.Length;

        input.PriceDifference.CopyTo(_inputValues.AsSpan(index, input.PriceDifference.Length));
        index += input.PriceDifference.Length;

        input.CandlestickHeight.CopyTo(_inputValues.AsSpan(index, input.CandlestickHeight.Length));
        index += input.CandlestickHeight.Length;

        input.Volume.CopyTo(_inputValues.AsSpan(index, input.Volume.Length));
        index += input.Volume.Length;

        input.TradesCount.CopyTo(_inputValues.AsSpan(index, input.TradesCount.Length));

        var inputLayerOutputs = InputLayer.CalculateOutputs(_inputValues, chunkInput: true);
        var hiddenLayerOutputs = HiddenLayer.CalculateOutputs(inputLayerOutputs, chunkInput: false);
        var outputLayerOutputs = OutputLayer.CalculateOutputs(hiddenLayerOutputs, chunkInput: false);

        return outputLayerOutputs.Span[0];
    }

    public static int GetWeightsCount(int inputsCount, int hiddenLayerNeuronsCount)
    {
        return
            inputsCount +
            inputsCount * hiddenLayerNeuronsCount +
            hiddenLayerNeuronsCount;
    }

    public static int GetBiasesCount(int inputsCount, int hiddenLayerNeuronsCount) =>
        inputsCount + hiddenLayerNeuronsCount + OutputLayerNeuronsCount;

    public static MarketNeuralNetwork CreateRandom(int inputsCount, int hiddenLayerNeuronsCount)
    {
        var weights = Enumerable
            .Repeat(0, GetWeightsCount(inputsCount, hiddenLayerNeuronsCount))
            .Select(_ => GetRandomWeight())
            .ToArray();

        var biases = Enumerable
            .Repeat(0, GetBiasesCount(inputsCount, hiddenLayerNeuronsCount))
            .Select(_ => GetRandomBias())
            .ToArray();

        return new MarketNeuralNetwork(inputsCount, hiddenLayerNeuronsCount, weights, biases);
    }

    private static double GetRandomWeight() => Random.Shared.NextDouble() * 2.0d - 1.0d;

    private static double GetRandomBias() => Random.Shared.NextDouble() * 2.0d - 1.0d;
}
