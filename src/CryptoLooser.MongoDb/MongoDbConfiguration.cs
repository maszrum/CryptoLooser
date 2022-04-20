namespace CryptoLooser.MongoDb;

public record MongoDbConfiguration(
    string ConnectionString, 
    string Database);