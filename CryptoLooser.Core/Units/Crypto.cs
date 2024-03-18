using System.Diagnostics;

namespace CryptoLooser.Core.Units;

[DebuggerDisplay("{Value} Crypto")]
public readonly struct Crypto(double value)
{
    public static readonly Crypto Zero = new(0.0d);

    public double Value { get; } = value;

    public override string ToString() => $"{Value} Crypto";

    public static implicit operator double(Crypto amount) => amount.Value;
}
