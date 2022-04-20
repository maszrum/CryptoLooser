namespace CryptoLooser.MongoDb;

public class MongoDbConfiguration
{
    public const string Section = "Mongo";

    public string ConnectionString { get; init; } = null!;

    public string Database { get; init; } = null!;
}