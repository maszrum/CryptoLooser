using CryptoLooser.Core.Models;
using NUnit.Framework;

namespace CryptoLooser.Core.Tests;

[TestFixture]
public class MarketCodeFixture
{
    [TestCase("BTC_PLN")]
    [TestCase("BTC--PLN")]
    [TestCase("BTC-ETH-PLN")]
    [TestCase("BTCPLN")]
    [TestCase("-BTCPLN")]
    [TestCase("BTCPLN-")]
    [TestCase("-")]
    [TestCase("")]
    [TestCase("-PLN")]
    [TestCase("BTC-")]
    [TestCase("PLN")]
    public void parse_method_should_throw_on_invalid_input(string code)
    {
        Assert.Throws<ArgumentException>(() =>
        {
            _ = MarketCode.Parse(code);
        });
    }

    [TestCase("BTC-PLN")]
    [TestCase("PLN-BTC")]
    [TestCase("ETH-BTC")]
    [TestCase("PLN-ETH")]
    [TestCase("A-B")]
    public void parse_method_should_succeed(string code)
    {
        Assert.DoesNotThrow(() =>
        {
            _ = MarketCode.Parse(code);
        });
    }
}