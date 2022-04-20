using System.Collections.Immutable;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Core.Flux.Domain;
using InfluxDB.Client.Writes;
using CryptoLooser.Core.Models;
using CryptoLooser.Core.Interfaces;

namespace CryptoLooser.InfluxDb;

public class ExchangeHistoryRepository : IExchangeHistoryRepository
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
        DateTime from, DateTime to, MarketCode marketCode, ChartResolution resolution)
    {
        using var connection = _connectionFactory.OpenQueryApi();

        var query = new FluxQueryBuilder()
            .FromBucket(connection.Bucket)
            .InRange(from.ToUniversalTime(), to.ToUniversalTime())
            .Filter("_measurement", "exchange_history")
            .Filter("market_code", marketCode.ToString())
            .Filter("chart_resolution", resolution.ToIntegerString())
            .WithDefaultPivot()
            .Build();

        var tables = await connection.Api.QueryAsync(query, connection.Organization);

        if (tables.Count == 0)
        {
            return new CandlestickChart(
                marketCode,
                resolution,
                ImmutableArray<CandlestickChartEntry>.Empty);
        }

        if (tables.Count > 1)
        {
            throw new InvalidOperationException(
                $"Received invalid data from database. It has {tables.Count} tables count.");
        }

        var entries = tables[0].Records
            .Select(RecordToEntry)
            .ToImmutableArray();

        return new CandlestickChart(
            marketCode,
            resolution,
            entries);
    }

    private static PointData EntryToPoint(
        CandlestickChartEntry entry, MarketCode marketCode, ChartResolution resolution)
    {
        return PointData
            .Measurement("exchange_history")
            .Tag("market_code", marketCode.ToString())
            .Tag("chart_resolution", resolution.ToIntegerString())
            .Field("opening_price", entry.OpeningPrice)
            .Field("closing_price", entry.ClosingPrice)
            .Field("highest_price", entry.HighestPrice)
            .Field("lowest_price", entry.LowestPrice)
            .Field("generated_volume", entry.GeneratedVolume)
            .Timestamp(entry.Timestamp.ToUniversalTime(), WritePrecision.S);
    }

    private static CandlestickChartEntry RecordToEntry(FluxRecord record)
    {
        return new CandlestickChartEntry(
            Timestamp: record.GetTimeInDateTime()!.Value.ToLocalTime(),
            OpeningPrice: record.GetDoubleValueByKey("opening_price"),
            ClosingPrice: record.GetDoubleValueByKey("closing_price"),
            HighestPrice: record.GetDoubleValueByKey("highest_price"),
            LowestPrice: record.GetDoubleValueByKey("lowest_price"),
            GeneratedVolume: record.GetDoubleValueByKey("generated_volume"));
    }
}