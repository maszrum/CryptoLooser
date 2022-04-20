namespace CryptoLooser.ZondaExchange.ApiClient.DTOs;

internal class CandlestickChartResponse
{
    public string Status { get; set; }
    public List<List<object>> Items { get; set; }
}