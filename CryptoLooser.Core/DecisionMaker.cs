using CryptoLooser.Core.Units;

namespace CryptoLooser.Core;

internal class DecisionMaker
{
    private DecisionKind _previousDecision = DecisionKind.Sell;

    public TradeDecision? ProvidePredictionAndGetDecisionIfChanged(MarketPrediction prediction, Usdt currentPrice)
    {
        if (prediction == MarketPrediction.ToTheMoon && _previousDecision == DecisionKind.Sell)
        {
            _previousDecision = DecisionKind.Buy;
            return new TradeDecision(DecisionKind.Buy, currentPrice);
        }

        if (prediction == MarketPrediction.DumpIt && _previousDecision == DecisionKind.Buy)
        {
            _previousDecision = DecisionKind.Sell;
            return new TradeDecision(DecisionKind.Sell, currentPrice);
        }

        return null;
    }
}
