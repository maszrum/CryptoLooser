using System.Collections.Immutable;
using RestSharp;
using CryptoLooser.ZondaExchange.ApiClient.DTOs;

namespace CryptoLooser.ZondaExchange.ApiClient;

public class ExchangeApiClient
{
    private readonly RestClient _restClient;
    private readonly UrlsFactory _urlsFactory;

    public ExchangeApiClient(
        RestClient restClient,
        string baseApiUrl)
    {
        _restClient = restClient;
        _urlsFactory = new UrlsFactory(baseApiUrl);
    }

    public async Task<ImmutableArray<CandlestickChartEntry>> GetCandlestickChartData(
        string marketCode,
        ChartResolution resolution,
        DateTime from,
        DateTime to)
    {
        var url = _urlsFactory.GetCandlestickChartUrl(marketCode, resolution, from, to);

        var response = await _restClient.GetJsonAsync<CandlestickChartResponse>(url);

        if (response is null)
        {
            throw new InvalidOperationException(
                "Cannot get candlestick chart data.");
        }

        var converter = new CandlestickChartResponseConverter();
        var entries = converter.ConvertToEntries(response);

        return entries;
    }
}