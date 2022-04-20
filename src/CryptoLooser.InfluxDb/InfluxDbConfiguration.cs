namespace CryptoLooser.InfluxDb;

public record InfluxDbConfiguration(
    string Address,
    string Token,
    string Bucket,
    string Organization);