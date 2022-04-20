using System.Collections.Immutable;
using MongoDB.Bson;
using MongoDB.Driver;
using CryptoLooser.Core.Interfaces;
using CryptoLooser.Core.Models;

namespace CryptoLooser.MongoDb;

public class AvailableDateRangesRepository : IAvailableDateRangesRepository
{
    private readonly ConnectionFactory _connectionFactory;
    private readonly DateRangeBsonConverter _bsonConverter = new();

    public AvailableDateRangesRepository(ConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<ImmutableArray<DateRange>> GetAvailableDateRanges(
        MarketCode marketCode,
        ChartResolution chartResolution)
    {
        var collection = _connectionFactory.GetDateRangesDocument();

        var filterBuilder = Builders<BsonDocument>.Filter;
        var filter = filterBuilder.Eq("marketCode", marketCode.ToString()) &
                     filterBuilder.Eq("chartResolution", (int) chartResolution);

        var cursor = await collection.FindAsync(filter);
        var bsonDocuments = await cursor.ToListAsync();

        return bsonDocuments
            .Select(bson => _bsonConverter.ConvertToObject(bson))
            .ToImmutableArray();
    }

    public async Task InsertAvailableDateRage(DateRange dateRange)
    {
        var collection = _connectionFactory.GetDateRangesDocument();

        var bsonDocument = _bsonConverter.ConvertToBson(dateRange);
        await collection.InsertOneAsync(bsonDocument);
    }
}