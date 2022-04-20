using CryptoLooser.Core.Models;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;

namespace CryptoLooser.InfluxDb;

public class ExchangeHistoryRepository
{
    private readonly ConnectionFactory _connectionFactory;

    public ExchangeHistoryRepository(ConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }
    
    public async Task WriteCandlestick(
        CandlestickChartEntry entry, MarketCode marketCode, ChartResolution resolution)
    {
        var point = EntryToPoint(entry, marketCode, resolution);
        
        using var connection = _connectionFactory.OpenWriteApi();
        
        await connection.Api.WritePointAsync(
            point: point,
            bucket: connection.Bucket,
            org: connection.Organization);
    }

    public async Task WriteCandlesticks(
        IEnumerable<CandlestickChartEntry> entries, MarketCode marketCode, ChartResolution resolution)
    {
        var points = entries
            .Select(entry => EntryToPoint(entry, marketCode, resolution))
            .ToArray();

        using var connection = _connectionFactory.OpenWriteApi();

        await connection.Api.WritePointsAsync(
            points: points,
            bucket: connection.Bucket,
            org: connection.Organization);
    }
    
    public async Task<CandlestickChart> ReadCandlesticks(
        DateTime @from, DateTime to, MarketCode marketCode, ChartResolution resolution)
    {
        using var connection = _connectionFactory.OpenQueryApi();
        
        var query = "from(bucket:\"test-bucket\") |> range(start: 0)";
        
        var tables = await connection.Api.QueryAsync(query, connection.Organization);
        
        throw new NotImplementedException();
    }

    private static PointData EntryToPoint(
        CandlestickChartEntry entry, MarketCode marketCode, ChartResolution resolution)
    {
        return PointData
            .Measurement("exchange-history")
            .Tag("market-code", marketCode.ToString())
            .Tag("chart-resolution", resolution.ToString())
            .Field("opening-price", entry.OpeningPrice)
            .Field("closing-price", entry.ClosingPrice)
            .Field("highest-price", entry.HighestPrice)
            .Field("lowest-price", entry.LowestPrice)
            .Field("generated-volume", entry.GeneratedVolume)
            .Timestamp(entry.Timestamp, WritePrecision.S);
    }
}