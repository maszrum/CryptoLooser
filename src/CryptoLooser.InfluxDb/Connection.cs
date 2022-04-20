using InfluxDB.Client;

namespace CryptoLooser.InfluxDb;

public class Connection<TApi> : IDisposable
{
    private readonly InfluxDBClient _client;

    public Connection(
        InfluxDBClient client, 
        TApi api, 
        string bucket, 
        string organization)
    {
        _client = client;
        Api = api;
        Bucket = bucket;
        Organization = organization;
    }

    public TApi Api { get; }
    
    public string Bucket { get; }
    
    public string Organization { get; }
    
    public void Dispose()
    {
        _client.Dispose();
    }
}