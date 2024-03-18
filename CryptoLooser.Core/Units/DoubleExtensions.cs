namespace CryptoLooser.Core.Units;

public static class DoubleExtensions
{
    public static Crypto AsCrypto(this double value) => new(value);

    public static Usdt AsUsdt(this double value) => new(value);
}
