using System.Globalization;
using MongoDB.Bson;
using CryptoLooser.Core.Models;

namespace CryptoLooser.MongoDb;

internal class DateRangeToBsonConverter
{
    public BsonDocument Convert(DateRange dateRange)
    {
        var bson = new BsonDocument
        {
            { "from", dateRange.From.ToString(CultureInfo.InvariantCulture) },
            { "to", dateRange.To.ToString(CultureInfo.InvariantCulture) },
            { "marketCode", dateRange.MarketCode.ToString() },
            { "chartResolution", ((int)dateRange.Resolution).ToString() }
        };
        
        return bson;
    }
}