using System.Collections.Immutable;
using CryptoLooser.Core.NeuralNetwork;
using CryptoLooser.Core.Units;

namespace CryptoLooser.Core.Mendel;

public class MarketSimulation : IFitnessProvider<MarketSimulationOutput>
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

        _neuralNetworkInputValuesCount = NormalizedNeuralNetworkInput.GetValuesCount(_neuralNetworkSeriesSize);

        _neuralNetworkWeightsCount = MarketNeuralNetwork.GetWeightsCount(
            _neuralNetworkInputValuesCount,
            _neuralNetworkHiddenLayerNeuronsCount);

        _neuralNetworkBiasesCount = MarketNeuralNetwork.GetBiasesCount(
            _neuralNetworkInputValuesCount,
            _neuralNetworkHiddenLayerNeuronsCount);

        RequiredChromosomeLength = _neuralNetworkWeightsCount + _neuralNetworkBiasesCount + 2;
    }

    public int RequiredChromosomeLength { get; }

    public MarketSimulationOutput Simulate(ReadOnlySpan<double> neuralNetworkParameters, Usdt startingBalance)
    {
        var network = new MarketNeuralNetwork(
            inputsCount: _neuralNetworkInputValuesCount,
            hiddenLayerNeuronsCount: _neuralNetworkHiddenLayerNeuronsCount,
            weights: neuralNetworkParameters.Slice(0, _neuralNetworkWeightsCount),
            biases: neuralNetworkParameters.Slice(_neuralNetworkWeightsCount, _neuralNetworkBiasesCount));

        var collector = new MarketDataCollector(_neuralNetworkSeriesSize);

        var predictionMaker = new PredictionMaker(
            (Math.Min(neuralNetworkParameters[^1], neuralNetworkParameters[^2]) + 1.0d) / 2.0d,
            (Math.Max(neuralNetworkParameters[^1], neuralNetworkParameters[^2]) + 1.0d) / 2.0d);

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
            startingBalance: startingBalance,
            decisions: decisionsArray.AsSpan(),
            finalPrice: _marketData[^1].ClosePrice.AsUsdt());

        return new MarketSimulationOutput(profit, decisionsArray);
    }

    public MarketSimulationOutput SimulateJustHold(Usdt startingBalance)
    {
        var decision = new TradeDecision(DecisionKind.Buy, _marketData[0].ClosePrice.AsUsdt());
        var decisions = ImmutableArray.Create(decision);

        var profit = _profitLossCalculator.Calculate(
            startingBalance: startingBalance,
            decisions: decisions.AsSpan(),
            finalPrice: _marketData[^1].ClosePrice.AsUsdt());

        return new MarketSimulationOutput(profit, decisions);
    }

    public MarketSimulationOutput SimulateBestPossible(Usdt startingBalance)
    {
        var decisions = ImmutableArray.CreateBuilder<TradeDecision>();
        var previousDecision = DecisionKind.Sell;

        for (var i = 1; i < _marketData.Length; i++)
        {
            var current = _marketData[i - 1];
            var next = _marketData[i];

            var decisionKind = current.ClosePrice < next.ClosePrice ? DecisionKind.Buy : DecisionKind.Sell;

            if (previousDecision != decisionKind)
            {
                decisions.Add(new TradeDecision(decisionKind, current.ClosePrice.AsUsdt()));
                previousDecision = decisionKind;
            }
        }

        var decisionsArray = decisions.ToImmutable();

        var profit = _profitLossCalculator.Calculate(
            startingBalance: startingBalance,
            decisions: decisionsArray.AsSpan(),
            finalPrice: _marketData[^1].ClosePrice.AsUsdt());

        return new MarketSimulationOutput(profit, decisionsArray);
    }

    public double GetFitness(ReadOnlySpan<double> chromosome, out MarketSimulationOutput state)
    {
        var simulationOutput = Simulate(chromosome, 1000.0d.AsUsdt());

        state = simulationOutput;

        return simulationOutput.Profit + (3 * Math.Sqrt(simulationOutput.Decisions.Length));
    }
}
