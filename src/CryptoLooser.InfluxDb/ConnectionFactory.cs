using InfluxDB.Client;

namespace CryptoLooser.InfluxDb;

public class ConnectionFactory
{
    private readonly InfluxDbConfiguration _configuration;

    public ConnectionFactory(InfluxDbConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Connection<QueryApi> OpenQueryApi()
    {
        var client = CreateClient();
        var queryApi = client.GetQueryApi();

        return new Connection<QueryApi>(
            client,
            queryApi,
            _configuration.Bucket,
            _configuration.Organization);
    }

    public Connection<WriteApiAsync> OpenWriteApi()
    {
        var client = CreateClient();
        var writeApi = client.GetWriteApiAsync();

        return new Connection<WriteApiAsync>(
            client,
            writeApi,
            _configuration.Bucket,
            _configuration.Organization);
    }

    public Connection<DeleteApi> OpenDeleteApi()
    {
        var client = CreateClient();
        var writeApi = client.GetDeleteApi();

        return new Connection<DeleteApi>(
            client,
            writeApi,
            _configuration.Bucket,
            _configuration.Organization);
    }

    private InfluxDBClient CreateClient() =>
        InfluxDBClientFactory.Create(_configuration.Address, _configuration.Token);
}