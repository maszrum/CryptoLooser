using NUnit.Framework;
using CryptoLooser.Core.Algorithms;
using CryptoLooser.Core.Models;

namespace CryptoLooser.Core.Tests;

[TestFixture]
public class DateRangeAlgorithmsFixture
{
    [Test]
    public void reduce_for_empty_should_return_empty()
    {
        var noDateRanges = Array.Empty<DateRange>();

        var result = new DateRangeAlgorithms()
            .Reduce(noDateRanges);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void reduce_for_two_overlapping_elements_should_return_one_reduced_element()
    {
        var dateRanges = new[]
        {
            new DateRange(
                from: new DateTime(2022, 3, 4, 3, 23, 44),
                to: new DateTime(2022, 3, 24, 12, 44, 54),
                marketCode: MarketCode.Parse("PLN-BTC"),
                resolution: ChartResolution.OneDay),
            new DateRange(
                from: new DateTime(2022, 3, 12, 22, 12, 4),
                to: new DateTime(2022, 3, 28, 23, 45, 12),
                marketCode: MarketCode.Parse("PLN-BTC"),
                resolution: ChartResolution.OneDay)
        };

        var result = new DateRangeAlgorithms()
            .Reduce(dateRanges);

        Assert.That(result, Has.Length.EqualTo(1));
        Assert.That(result[0].From, Is.EqualTo(new DateTime(2022, 3, 4, 3, 23, 44)));
        Assert.That(result[0].To, Is.EqualTo(new DateTime(2022, 3, 28, 23, 45, 12)));
    }

    [Test]
    public void reduce_for_three_overlapping_elements_should_return_one_reduced_element()
    {
        var dateRanges = new[]
        {
            new DateRange(
                from: new DateTime(2022, 3, 4, 3, 23, 44),
                to: new DateTime(2022, 3, 24, 12, 44, 54),
                marketCode: MarketCode.Parse("PLN-BTC"),
                resolution: ChartResolution.OneDay),
            new DateRange(
                from: new DateTime(2022, 3, 2, 22, 12, 4),
                to: new DateTime(2022, 3, 25, 23, 45, 12),
                marketCode: MarketCode.Parse("PLN-BTC"),
                resolution: ChartResolution.OneDay),
            new DateRange(
                from: new DateTime(2022, 3, 12, 22, 12, 4),
                to: new DateTime(2022, 4, 28, 23, 45, 12),
                marketCode: MarketCode.Parse("PLN-BTC"),
                resolution: ChartResolution.OneDay)
        };

        var result = new DateRangeAlgorithms()
            .Reduce(dateRanges);

        Assert.That(result, Has.Length.EqualTo(1));
        Assert.That(result[0].From, Is.EqualTo(new DateTime(2022, 3, 2, 22, 12, 4)));
        Assert.That(result[0].To, Is.EqualTo(new DateTime(2022, 4, 28, 23, 45, 12)));
    }

    [Test]
    public void reduce_for_four_overlapping_elements_should_return_two_reduced_element()
    {
        var dateRanges = new[]
        {
            new DateRange(
                from: new DateTime(2022, 4, 3, 23, 45, 12),
                to: new DateTime(2022, 4, 28, 23, 45, 12),
                marketCode: MarketCode.Parse("PLN-BTC"),
                resolution: ChartResolution.OneDay),
            new DateRange(
                from: new DateTime(2022, 3, 4, 3, 23, 44),
                to: new DateTime(2022, 3, 24, 12, 44, 54),
                marketCode: MarketCode.Parse("PLN-BTC"),
                resolution: ChartResolution.OneDay),
            new DateRange(
                from: new DateTime(2022, 3, 2, 22, 12, 4),
                to: new DateTime(2022, 3, 25, 23, 45, 12),
                marketCode: MarketCode.Parse("PLN-BTC"),
                resolution: ChartResolution.OneDay),
            new DateRange(
                from: new DateTime(2022, 3, 26, 22, 12, 4),
                to: new DateTime(2022, 4, 3, 23, 45, 12),
                marketCode: MarketCode.Parse("PLN-BTC"),
                resolution: ChartResolution.OneDay)
        };

        var result = new DateRangeAlgorithms()
            .Reduce(dateRanges);

        Assert.That(result, Has.Length.EqualTo(2));
        Assert.That(result[0].From, Is.EqualTo(new DateTime(2022, 3, 2, 22, 12, 4)));
        Assert.That(result[0].To, Is.EqualTo(new DateTime(2022, 3, 25, 23, 45, 12)));
        Assert.That(result[1].From, Is.EqualTo(new DateTime(2022, 3, 26, 22, 12, 4)));
        Assert.That(result[1].To, Is.EqualTo(new DateTime(2022, 4, 28, 23, 45, 12)));
    }

    [Test]
    public void reduce_for_two_separate_elements_should_return_same_as_input()
    {
        var dateRanges = new[]
        {
            new DateRange(
                from: new DateTime(2022, 3, 4, 3, 23, 44),
                to: new DateTime(2022, 3, 24, 12, 44, 54),
                marketCode: MarketCode.Parse("PLN-BTC"),
                resolution: ChartResolution.OneDay),
            new DateRange(
                from: new DateTime(2022, 3, 25, 22, 12, 4),
                to: new DateTime(2022, 3, 28, 23, 45, 12),
                marketCode: MarketCode.Parse("PLN-BTC"),
                resolution: ChartResolution.OneDay)
        };

        var result = new DateRangeAlgorithms()
            .Reduce(dateRanges);

        Assert.That(result, Has.Length.EqualTo(2));
        CollectionAssert.AreEquivalent(dateRanges, result);
    }

    [Test]
    public void reduce_for_elements_with_different_market_code_should_throw()
    {
        var dateRanges = new[]
        {
            new DateRange(
                from: new DateTime(2022, 3, 4, 3, 23, 44),
                to: new DateTime(2022, 3, 24, 12, 44, 54),
                marketCode: MarketCode.Parse("PLN-ETH"),
                resolution: ChartResolution.OneDay),
            new DateRange(
                from: new DateTime(2022, 3, 25, 22, 12, 4),
                to: new DateTime(2022, 3, 28, 23, 45, 12),
                marketCode: MarketCode.Parse("PLN-BTC"),
                resolution: ChartResolution.OneDay)
        };

        Assert.Throws<ArgumentException>(() =>
        {
            _ = new DateRangeAlgorithms()
                .Reduce(dateRanges);
        });
    }

    [Test]
    public void reduce_for_elements_with_different_chart_resolution_should_throw()
    {
        var dateRanges = new[]
        {
            new DateRange(
                from: new DateTime(2022, 3, 4, 3, 23, 44),
                to: new DateTime(2022, 3, 24, 12, 44, 54),
                marketCode: MarketCode.Parse("PLN-BTC"),
                resolution: ChartResolution.OneMinute),
            new DateRange(
                from: new DateTime(2022, 3, 25, 22, 12, 4),
                to: new DateTime(2022, 3, 28, 23, 45, 12),
                marketCode: MarketCode.Parse("PLN-BTC"),
                resolution: ChartResolution.OneDay)
        };

        Assert.Throws<ArgumentException>(() =>
        {
            _ = new DateRangeAlgorithms()
                .Reduce(dateRanges);
        });
    }

    [Test]
    public void check_if_overlaps_should_return_true_with_middle_as_common_part()
    {
        var first = new DateRange(
            from: new DateTime(2022, 4, 5, 12, 54, 23),
            to: new DateTime(2022, 4, 12, 15, 23, 11),
            marketCode: MarketCode.Parse("PLN-BTC"),
            resolution: ChartResolution.FifteenMinutes);

        var second = new DateRange(
            from: new DateTime(2022, 4, 8, 3, 44, 12),
            to: new DateTime(2022, 4, 17, 22, 13, 3),
            marketCode: MarketCode.Parse("PLN-BTC"),
            resolution: ChartResolution.FifteenMinutes);

        var overlaps = new DateRangeAlgorithms()
            .CheckIfOverlaps(first, second, out var commonPart);

        var overlapsInverse = new DateRangeAlgorithms()
            .CheckIfOverlaps(second, first, out var commonPartInverse);

        Assert.That(overlaps, Is.True);
        Assert.That(overlapsInverse, Is.True);

        if (commonPart is null || commonPartInverse is null)
        {
            throw new Exception();
        }

        Assert.That(commonPart.From, Is.EqualTo(second.From));
        Assert.That(commonPart.To, Is.EqualTo(first.To));
        Assert.That(commonPartInverse.From, Is.EqualTo(second.From));
        Assert.That(commonPartInverse.To, Is.EqualTo(first.To));
    }

    [Test]
    public void check_if_overlaps_should_return_true_with_narrower_as_common_part()
    {
        var first = new DateRange(
            from: new DateTime(2022, 4, 5, 12, 54, 23),
            to: new DateTime(2022, 4, 12, 15, 23, 11),
            marketCode: MarketCode.Parse("PLN-BTC"),
            resolution: ChartResolution.FifteenMinutes);

        var second = new DateRange(
            from: new DateTime(2022, 4, 6, 3, 44, 12),
            to: new DateTime(2022, 4, 11, 22, 13, 3),
            marketCode: MarketCode.Parse("PLN-BTC"),
            resolution: ChartResolution.FifteenMinutes);

        var overlaps = new DateRangeAlgorithms()
            .CheckIfOverlaps(first, second, out var commonPart);

        var overlapsInverse = new DateRangeAlgorithms()
            .CheckIfOverlaps(second, first, out var commonPartInverse);

        Assert.That(overlaps, Is.True);
        Assert.That(overlapsInverse, Is.True);

        if (commonPart is null || commonPartInverse is null)
        {
            throw new Exception();
        }

        Assert.That(commonPart.From, Is.EqualTo(second.From));
        Assert.That(commonPart.To, Is.EqualTo(second.To));
        Assert.That(commonPartInverse.From, Is.EqualTo(second.From));
        Assert.That(commonPartInverse.To, Is.EqualTo(second.To));
    }

    [Test]
    public void check_if_overlaps_should_return_false_with_null_as_common_part()
    {
        var first = new DateRange(
            from: new DateTime(2022, 4, 5, 12, 54, 23),
            to: new DateTime(2022, 4, 12, 15, 23, 11),
            marketCode: MarketCode.Parse("PLN-BTC"),
            resolution: ChartResolution.FifteenMinutes);

        var second = new DateRange(
            from: new DateTime(2022, 4, 13, 3, 44, 12),
            to: new DateTime(2022, 4, 24, 22, 13, 3),
            marketCode: MarketCode.Parse("PLN-BTC"),
            resolution: ChartResolution.FifteenMinutes);

        var overlaps = new DateRangeAlgorithms()
            .CheckIfOverlaps(first, second, out var commonPart);

        var overlapsInverse = new DateRangeAlgorithms()
            .CheckIfOverlaps(second, first, out var commonPartInverse);

        Assert.That(overlaps, Is.False);
        Assert.That(overlapsInverse, Is.False);
        Assert.That(commonPart, Is.Null);
        Assert.That(commonPartInverse, Is.Null);
    }

    [Test]
    public void check_if_overlaps_with_different_market_codes_should_throw()
    {
        var first = new DateRange(
            from: new DateTime(2022, 4, 5, 12, 54, 23),
            to: new DateTime(2022, 4, 12, 15, 23, 11),
            marketCode: MarketCode.Parse("PLN-ETH"),
            resolution: ChartResolution.FifteenMinutes);

        var second = new DateRange(
            from: new DateTime(2022, 4, 13, 3, 44, 12),
            to: new DateTime(2022, 4, 24, 22, 13, 3),
            marketCode: MarketCode.Parse("PLN-BTC"),
            resolution: ChartResolution.FifteenMinutes);

        Assert.Throws<ArgumentException>(() =>
        {
            _ = new DateRangeAlgorithms()
                .CheckIfOverlaps(first, second, out _);
        });

        Assert.Throws<ArgumentException>(() =>
        {
            _ = new DateRangeAlgorithms()
                .CheckIfOverlaps(second, first, out _);
        });
    }
    
    [Test]
    public void subtract_should_return_empty_result()
    {
        var minuend = new DateRange(
            from: new DateTime(2022, 3, 10, 14, 41, 2),
            to: new DateTime(2022, 3, 18, 21, 4, 21),
            marketCode: MarketCode.Parse("PLN-BTC"),
            resolution: ChartResolution.FifteenMinutes);
        
        var subtrahend = new DateRange(
            from: new DateTime(2022, 3, 10, 14, 41, 2),
            to: new DateTime(2022, 3, 18, 21, 4, 21),
            marketCode: MarketCode.Parse("PLN-BTC"),
            resolution: ChartResolution.FifteenMinutes);
    }
}