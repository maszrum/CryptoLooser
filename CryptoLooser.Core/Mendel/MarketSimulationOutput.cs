using System.Collections.Immutable;
using CryptoLooser.Core.Units;

namespace CryptoLooser.Core.Mendel;

public readonly record struct MarketSimulationOutput(
    Usdt Profit,
    ImmutableArray<TradeDecision> Decisions);
