using CryptoLooser.Core.Units;

namespace CryptoLooser.Core;

internal class ProfitLossCalculator
{
    public Usdt Calculate(Usdt startingBalance, ReadOnlySpan<TradeDecision> decisions, Usdt finalPrice)
    {
        if (decisions.Length == 0)
        {
            return Usdt.Zero;
        }

        if (decisions[0].Kind != DecisionKind.Buy)
        {
            throw new InvalidOperationException("Invalid buy/sell sequence.");
        }

        var currentBalanceUsdt = startingBalance;
        var currentBalanceCrypto = Crypto.Zero;

        for (var i = 0; i < decisions.Length; i++)
        {
            var expectedDecisionKind = i % 2 == 0 ? DecisionKind.Buy : DecisionKind.Sell;
            var actualDecision = decisions[i];

            if (expectedDecisionKind != actualDecision.Kind)
            {
                throw new InvalidOperationException("Invalid buy/sell sequence.");
            }

            if (actualDecision.Kind == DecisionKind.Buy)
            {
                currentBalanceCrypto = currentBalanceUsdt.ToCrypto(actualDecision.Price);
                currentBalanceUsdt = Usdt.Zero;
            }
            else
            {
                currentBalanceUsdt = currentBalanceCrypto.ToUsdt(actualDecision.Price);
                currentBalanceCrypto = Crypto.Zero;
            }
        }

        var lastDecisionKind = decisions[^1].Kind;

        var finalBalance = lastDecisionKind == DecisionKind.Buy
            ? currentBalanceCrypto.ToUsdt(finalPrice)
            : currentBalanceUsdt;

        return (finalBalance - startingBalance).AsUsdt();
    }
}
