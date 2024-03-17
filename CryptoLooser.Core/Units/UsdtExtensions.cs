namespace CryptoLooser.Core.Units;

internal static class UsdtExtensions
{
    public static Crypto ToCrypto(this Usdt value, Usdt price) => (value / price).AsCrypto();
}
