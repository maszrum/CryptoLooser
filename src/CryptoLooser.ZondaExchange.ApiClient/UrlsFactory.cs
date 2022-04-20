using System.Text;
using CryptoLooser.Core.Models;

namespace CryptoLooser.ZondaExchange.ApiClient;

internal class UrlsFactory
{
    private readonly string _baseUrl;

    public UrlsFactory(string baseUrl)
    {
        if (baseUrl.EndsWith('/'))
        {
            baseUrl = baseUrl[..^1];
        }

        _baseUrl = baseUrl;
    }

    public string GetBaseUrl() => _baseUrl;

    public string GetCandlestickChartUrl(
        MarketCode marketCode,
        ChartResolution resolution,
        DateTime from,
        DateTime to)
    {
        var resolutionInSeconds = (int) resolution;
        var fromInUnixMilliseconds = from.ToUnixTimeMilliseconds();
        var toInUnixMilliseconds = to.ToUnixTimeMilliseconds();

        return new StringBuilder()
            .Append("rest/trading/candle/history/")
            .Append(marketCode.ToString())
            .Append('/')
            .Append(resolutionInSeconds)
            .Append("?from=")
            .Append(fromInUnixMilliseconds)
            .Append("&to=")
            .Append(toInUnixMilliseconds)
            .ToString();
    }
}