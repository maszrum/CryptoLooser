using NUnit.Framework;
using RestSharp;

namespace CryptoLooser.ZondaExchange.ApiClient.Tests;

[TestFixture]
public class ExchangeApiClientTests
{
    private const string ExchangeApiBase = "https://api.zonda.exchange";

    [Test]
    public async Task request_for_btc_pln_market_one_day_resolution_chart_should_succeed()
    {
        var restClient = new RestClient(ExchangeApiBase);
        var client = new ExchangeApiClient(restClient, ExchangeApiBase);
        
        var dateFrom = new DateTime(2022, 4, 3, 13, 56, 10);
        var dateTo = dateFrom.AddDays(10);
        
        var candlestickChartData = await client.GetCandlestickChartData(
            "BTC-PLN", 
            ChartResolution.OneDay, 
            dateFrom, 
            dateTo);
        
        Assert.Multiple(() =>
        {
            Assert.That(
                candlestickChartData, 
                Has.Length.EqualTo(10));
            
            Assert.That(
                candlestickChartData.First().Timestamp.Day,
                Is.EqualTo(4));
            
            Assert.That(
                candlestickChartData.Last().Timestamp.Day,
                Is.EqualTo(13));
        });
    }

    [Test]
    public async Task request_for_eth_pln_market_one_minute_resolution_chart_should_succeed()
    {
        var restClient = new RestClient(ExchangeApiBase);
        var client = new ExchangeApiClient(restClient, ExchangeApiBase);
        
        var dateFrom = new DateTime(2022, 4, 3, 13, 56, 10);
        var dateTo = dateFrom.AddHours(10);
        
        var candlestickChartData = await client.GetCandlestickChartData(
            "ETH-PLN", 
            ChartResolution.FifteenMinutes, 
            dateFrom, 
            dateTo);
        
        Assert.Multiple(() =>
        {
            Assert.That(
                candlestickChartData, 
                Has.Length.EqualTo(40));
            
            Assert.That(
                candlestickChartData.First().Timestamp.Hour,
                Is.EqualTo(14));
            
            Assert.That(
                candlestickChartData.First().Timestamp.Minute,
                Is.EqualTo(0));
            
            Assert.That(
                candlestickChartData.Last().Timestamp.Hour,
                Is.EqualTo(23));
            
            Assert.That(
                candlestickChartData.Last().Timestamp.Minute,
                Is.EqualTo(45));
        });
    }
}