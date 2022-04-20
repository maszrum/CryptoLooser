using MongoDB.Bson;
using CryptoLooser.Core.Models;

namespace CryptoLooser.MongoDb;

internal class DateRangeBsonConverter
{
    public BsonDocument ConvertToBson(DateRange dateRange)
    {
        var bson = new BsonDocument
        {
            {"from", new BsonDateTime(dateRange.From)},
            {"to", new BsonDateTime(dateRange.To)},
            {"marketCode", ConvertMarketCode(dateRange.MarketCode)},
            {"chartResolution", ConvertChartResolution(dateRange.Resolution)}
        };

        return bson;
    }

    public DateRange ConvertToObject(BsonDocument bsonDocument)
    {
        return new DateRange(
            from: bsonDocument["from"].AsBsonDateTime.ToLocalTime(),
            to: bsonDocument["to"].AsBsonDateTime.ToLocalTime(),
            marketCode: MarketCode.Parse(bsonDocument["marketCode"].AsString),
            resolution: (ChartResolution) bsonDocument["chartResolution"].AsInt32);
    }

    private static BsonValue ConvertMarketCode(MarketCode marketCode) =>
        new BsonString(marketCode.ToString());

    private static BsonValue ConvertChartResolution(ChartResolution resolution) =>
        new BsonInt32((int) resolution);
}