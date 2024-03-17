using System.Collections.Immutable;

namespace CryptoLooser.Core.Mendel;

public readonly record struct IndividualState(
    double Profit,
    ImmutableArray<TradeDecision> Decisions);
