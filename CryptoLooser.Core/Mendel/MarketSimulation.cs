using System.Collections.Immutable;
using System.Reflection;
using CryptoLooser.Core.NeuralNetwork;
using CryptoLooser.Core.Units;

namespace CryptoLooser.Core.Mendel;

public class MarketSimulation : IFitnessProvider<IndividualState>
{
    private readonly ImmutableArray<MarketDataRow> _marketData;
    private readonly ProfitLossCalculator _profitLossCalculator = new();
    private readonly int _neuralNetworkInputValuesCount;
    private readonly int _neuralNetworkHiddenLayerNeuronsCount;
    private readonly int _neuralNetworkSeriesSize;
    private readonly int _neuralNetworkWeightsCount;
    private readonly int _neuralNetworkBiasesCount;

    public MarketSimulation(
        ImmutableArray<MarketDataRow> marketData,
        int neuralNetworkHiddenLayerNeuronsCount,
        int neuralNetworkSeriesSize)
    {
        _marketData = marketData;
        _neuralNetworkHiddenLayerNeuronsCount = neuralNetworkHiddenLayerNeuronsCount;
        _neuralNetworkSeriesSize = neuralNetworkSeriesSize;

        _neuralNetworkInputValuesCount =
            GetPropertiesCount(typeof(NormalizedNeuralNetworkInput)) *
            _neuralNetworkSeriesSize;

        _neuralNetworkWeightsCount = MarketNeuralNetwork.GetWeightsCount(
            _neuralNetworkInputValuesCount,
            _neuralNetworkHiddenLayerNeuronsCount);

        _neuralNetworkBiasesCount = MarketNeuralNetwork.GetBiasesCount(
            _neuralNetworkInputValuesCount,
            _neuralNetworkHiddenLayerNeuronsCount);

        RequiredChromosomeLength = _neuralNetworkWeightsCount + _neuralNetworkBiasesCount + 2;
    }

    public int RequiredChromosomeLength { get; }

    public double GetFitness(ReadOnlySpan<double> chromosome, out IndividualState state)
    {
        var network = new MarketNeuralNetwork(
            inputsCount: _neuralNetworkInputValuesCount,
            hiddenLayerNeuronsCount: _neuralNetworkHiddenLayerNeuronsCount,
            weights: chromosome.Slice(0, _neuralNetworkWeightsCount),
            biases: chromosome.Slice(_neuralNetworkWeightsCount, _neuralNetworkBiasesCount));

        var collector = new MarketDataCollector(_neuralNetworkSeriesSize);

        var predictionMaker = new PredictionMaker(
            (Math.Min(chromosome[^1], chromosome[^2]) + 1.0d) / 2.0d,
            (Math.Max(chromosome[^1], chromosome[^2]) + 1.0d) / 2.0d);

        var decisionMaker = new DecisionMaker();

        var decisions = ImmutableArray.CreateBuilder<TradeDecision>();

        foreach (var marketDataRow in _marketData)
        {
            var collectedDataAvailable = collector.Feed(marketDataRow);

            if (collectedDataAvailable)
            {
                var outputValue = network.FeedForward(collector.GetNormalizedValues());
                var prediction = predictionMaker.Predict(outputValue);

                var decision = decisionMaker.ProvidePredictionAndGetDecisionIfChanged(
                    prediction,
                    marketDataRow.ClosePrice.AsUsdt());

                if (decision is not null)
                {
                    decisions.Add(decision);
                }
            }
        }

        var decisionsArray = decisions.ToImmutable();

        var profit = _profitLossCalculator.Calculate(
            startingBalance: 1000.0d.AsUsdt(),
            decisions: decisionsArray.AsSpan(),
            finalPrice: _marketData[^1].ClosePrice.AsUsdt());

        state = new IndividualState(profit, decisionsArray);

        return profit + 3 * Math.Sqrt(decisionsArray.Length);
    }

    private static int GetPropertiesCount(Type type)
    {
        return type
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Length;
    }
}
