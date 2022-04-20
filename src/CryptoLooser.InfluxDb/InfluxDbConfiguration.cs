namespace CryptoLooser.InfluxDb;

public class InfluxDbConfiguration
{
    public const string Section = "Influx";

    public string Address { get; init; } = null!;

    public string Token { get; init; } = null!;

    public string Bucket { get; init; } = null!;

    public string Organization { get; init; } = null!;
}