using System.Collections.Immutable;
using System.Globalization;
using System.Text.Json;
using CryptoLooser.Core.Models;

namespace CryptoLooser.ZondaExchange.ApiClient.DTOs;

internal class CandlestickChartResponseConverter
{
    public ImmutableArray<CandlestickChartEntry> ConvertToEntries(CandlestickChartResponse response)
    {
        return response.Items
            .Select(item =>
            {
                var timestampJsonElement = (JsonElement) item[0];
                var dataJsonElement = (JsonElement) item[1];
                return JsonDataToEntry(timestampJsonElement, dataJsonElement);
            })
            .ToImmutableArray();
    }

    private static CandlestickChartEntry JsonDataToEntry(JsonElement timestampElement, JsonElement dataElement)
    {
        var timestamp = GetTimestampFromJsonElement(timestampElement);

        return new CandlestickChartEntry(
            Timestamp: timestamp,
            OpeningPrice: GetDoubleFromProperty(dataElement, "o"),
            ClosingPrice: GetDoubleFromProperty(dataElement, "c"),
            HighestPrice: GetDoubleFromProperty(dataElement, "h"),
            LowestPrice: GetDoubleFromProperty(dataElement, "l"),
            GeneratedVolume: GetDoubleFromProperty(dataElement, "v"));
    }

    private static DateTime GetTimestampFromJsonElement(JsonElement jsonElement)
    {
        var timestampAsString = jsonElement.GetString() ?? throw new InvalidOperationException(
            $"Ivalid element value, value kind: {jsonElement.ValueKind}");

        var timestampAsUnixMilliseconds = long.Parse(timestampAsString);
        var timestamp = timestampAsUnixMilliseconds.FromUnixMillisecondsToDateTime();

        return timestamp;
    }

    private static double GetDoubleFromProperty(JsonElement element, string propertyName)
    {
        var property = element.GetProperty(propertyName);

        var doubleAsString = property.GetString() ?? throw new InvalidOperationException(
            $"Ivalid element value, value kind: {property.ValueKind}");

        return double.Parse(doubleAsString, CultureInfo.InvariantCulture);
    }
}