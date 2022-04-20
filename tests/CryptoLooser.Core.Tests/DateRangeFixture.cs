using CryptoLooser.Core.Models;
using NUnit.Framework;

namespace CryptoLooser.Core.Tests;

[TestFixture]
public class DateRangeFixture
{
    [Test]
    public void ctor_should_throw_on_invalid_dates()
    {
        var from = new DateTime(2022, 4, 7, 12, 30, 1);
        var to = new DateTime(2022, 4, 7, 12, 29, 59);

        Assert.Throws<ArgumentException>(() =>
        {
            _ = new DateRange(
                from,
                to,
                MarketCode.Parse("PLN-BTC"),
                ChartResolution.OneWeek);
        });
    }

    [Test]
    public void ctor_should_not_throw_on_invalid_dates()
    {
        var from = new DateTime(2022, 4, 7, 12, 29, 59);
        var to = new DateTime(2022, 4, 7, 12, 30, 1);

        Assert.DoesNotThrow(() =>
        {
            _ = new DateRange(
                from,
                to,
                MarketCode.Parse("PLN-BTC"),
                ChartResolution.OneWeek);
        });
    }
}