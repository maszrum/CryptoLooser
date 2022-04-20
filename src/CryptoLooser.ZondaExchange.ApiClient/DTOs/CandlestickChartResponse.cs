namespace CryptoLooser.ZondaExchange.ApiClient.DTOs;

internal class CandlestickChartResponse : ApiResponse
{
    public List<List<object>> Items { get; set; } = null!;
}