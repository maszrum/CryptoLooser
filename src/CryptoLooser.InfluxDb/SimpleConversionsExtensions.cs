using InfluxDB.Client.Core.Flux.Domain;
using CryptoLooser.Core.Models;

namespace CryptoLooser.InfluxDb;

internal static class SimpleConversionsExtensions
{
    public static string ToJsonFormat(this DateTime dateTime) => 
        dateTime.ToString("yyyy-MM-ddTHH:mm:ssZ");
    
    public static string ToIntegerString(this ChartResolution resolution) => 
        ((int) resolution).ToString();
    
    public static double GetDoubleValueByKey(this FluxRecord record, string key) => 
        (double) record.GetValueByKey(key);
}