using System.Diagnostics;

namespace CryptoLooser.Core.Units;

[DebuggerDisplay("{Value} USDT")]
public readonly struct Usdt(double value)
{
    public static readonly Usdt Zero = new(0.0d);

    public double Value { get; } = value;

    public override string ToString() => $"{Value:F2} USDT";

    public static implicit operator double(Usdt amount) => amount.Value;
}
