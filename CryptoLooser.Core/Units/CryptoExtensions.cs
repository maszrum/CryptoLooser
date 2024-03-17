namespace CryptoLooser.Core.Units;

internal static class CryptoExtensions
{
    public static Usdt ToUsdt(this Crypto value, Usdt price) => (value * price).AsUsdt();
}
