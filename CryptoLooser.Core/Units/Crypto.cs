using System.Diagnostics;

namespace CryptoLooser.Core.Units;

[DebuggerDisplay("{Value} Crypto")]
internal readonly struct Crypto(double value)
{
    public static readonly Crypto Zero = new(0.0d);

    public double Value { get; } = value;

    public static implicit operator double(Crypto amount) => amount.Value;
}
