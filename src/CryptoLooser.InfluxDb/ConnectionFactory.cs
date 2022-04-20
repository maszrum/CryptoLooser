using InfluxDB.Client;

namespace CryptoLooser.InfluxDb;

public class ConnectionFactory
{
    private readonly InfluxDbConfiguration _configuration;

    private string? _organizationId;

    private ConnectionFactory(InfluxDbConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string OrganizationId => _organizationId ?? throw new InvalidOperationException();

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

    public Connection<BucketsApi> OpenBucketsApi()
    {
        var client = CreateClient();
        var bucketApi = client.GetBucketsApi();

        return new Connection<BucketsApi>(
            client,
            bucketApi,
            _configuration.Bucket,
            _configuration.Organization);
    }

    private async Task InitializeOrganizationId()
    {
        var client = CreateClient();
        var organizations = await client.GetOrganizationsApi()
            .FindOrganizationsAsync(org: _configuration.Organization);

        if (organizations.Count != 1)
        {
            throw new InvalidOperationException(
                $"Cannot find organization with specified name: {_configuration.Organization}");
        }

        _organizationId = organizations[0].Id;
    }

    private InfluxDBClient CreateClient() =>
        InfluxDBClientFactory.Create(_configuration.Address, _configuration.Token);

    public static async Task<ConnectionFactory> Create(InfluxDbConfiguration configuration)
    {
        var connectionFactory = new ConnectionFactory(configuration);
        await connectionFactory.InitializeOrganizationId();

        return connectionFactory;
    }
}