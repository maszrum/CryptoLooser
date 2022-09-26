using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using CryptoLooser.Core.Common;
using CryptoLooser.Core.Models;

namespace CryptoLooser.Core.Algorithms;

internal class DateRangeAlgorithms
{
    public ImmutableArray<DateRange> Reduce(IReadOnlyList<DateRange> inputDateRanges)
    {
        if (inputDateRanges.Count == 0)
        {
            return ImmutableArray<DateRange>.Empty;
        }

        ThrowIfInconsistentProperties(inputDateRanges);

        var builder = ImmutableArray.CreateBuilder<DateRange>();

        DateRange? previous = null;

        foreach (var dateRange in inputDateRanges.OrderBy(r => r.From))
        {
            if (previous is null) // first
            {
                builder.Add(dateRange);
                previous = dateRange;
                continue;
            }

            var overlapsWithPrevious = CheckIfOverlaps(
                first: previous,
                second: dateRange,
                checkConsistency: false,
                out _);

            var almostOverlapsWithPrevious = previous.To == dateRange.From;

            if (overlapsWithPrevious || almostOverlapsWithPrevious)
            {
                var joinedDateRange = new DateRange(
                    from: previous.From,
                    to: previous.To > dateRange.To ? previous.To : dateRange.To,
                    marketCode: previous.MarketCode,
                    resolution: previous.Resolution);

                builder[^1] = joinedDateRange;
                previous = joinedDateRange;
            }
            else
            {
                builder.Add(dateRange);
                previous = dateRange;
            }
        }

        return builder.ToImmutable();
    }

    public ImmutableArray<DateRange> Subtract(DateRange minuend, DateRange subtrahend)
    {
        ThrowIfInconsistentProperties(minuend, subtrahend);

        if (subtrahend.From <= minuend.From && subtrahend.To >= minuend.To)
        {
            return ImmutableArray<DateRange>.Empty;
        }

        if (subtrahend.To <= minuend.From || subtrahend.From >= minuend.To)
        {
            return ImmutableArray.Create(minuend);
        }

        if (subtrahend.From > minuend.From && subtrahend.To < minuend.To)
        {
            var firstPart = new DateRange(
                from: minuend.From,
                to: subtrahend.From,
                marketCode: minuend.MarketCode,
                resolution: minuend.Resolution);

            var secondPart = new DateRange(
                from: subtrahend.To,
                to: minuend.To,
                marketCode: minuend.MarketCode,
                resolution: minuend.Resolution);

            return ImmutableArray.Create(firstPart, secondPart);
        }

        if (subtrahend.From.IsBetweenExclusive(minuend))
        {
            var result = new DateRange(
                from: subtrahend.From,
                to: minuend.To,
                marketCode: minuend.MarketCode,
                resolution: minuend.Resolution);

            return ImmutableArray.Create(result);
        }

        if (subtrahend.To.IsBetweenExclusive(minuend))
        {
            var result = new DateRange(
                from: minuend.From,
                to: subtrahend.To,
                marketCode: minuend.MarketCode,
                resolution: minuend.Resolution);

            return ImmutableArray.Create(result);
        }

        throw new InvalidOperationException("Case not supported.");
    }

    public bool CheckIfOverlaps(
        DateRange first,
        DateRange second,
        [NotNullWhen(true)] out DateRange? commonPart)
    {
        return CheckIfOverlaps(first, second, checkConsistency: true, out commonPart);
    }

    private static bool CheckIfOverlaps(
        DateRange first,
        DateRange second,
        bool checkConsistency,
        [NotNullWhen(true)] out DateRange? commonPart)
    {
        if (checkConsistency)
        {
            ThrowIfInconsistentProperties(first, second);
        }

        if (first.From > second.From)
        {
            (first, second) = (second, first);
        }

        var overlaps = second.From >= first.From && second.From < first.To;

        if (overlaps)
        {
            var completelyInside = second.To <= first.To;

            commonPart = completelyInside
                ? second
                : new DateRange(second.From, first.To, first.MarketCode, first.Resolution);
        }
        else
        {
            commonPart = default;
        }

        return overlaps;
    }

    private static void ThrowIfInconsistentProperties(IReadOnlyList<DateRange> inputDateRanges)
    {
        var validChartResolution = inputDateRanges[0].Resolution;
        var validMarketCode = inputDateRanges[0].MarketCode;

        var isCollectionContainingInvalid = inputDateRanges
            .Skip(1)
            .Any(r => r.Resolution != validChartResolution || r.MarketCode != validMarketCode);

        if (isCollectionContainingInvalid)
        {
            throw new ArgumentException(
                $"Collection should contain elements with same " +
                $"{nameof(DateRange.Resolution)} and {nameof(DateRange.MarketCode)}",
                nameof(inputDateRanges));
        }
    }

    private static void ThrowIfInconsistentProperties(DateRange first, DateRange second)
    {
        var isBroken = first.Resolution != second.Resolution ||
                       first.MarketCode != second.MarketCode;

        if (isBroken)
        {
            throw new ArgumentException(
                $"Elements {nameof(first)} and {nameof(second)} should have same " +
                $"{nameof(DateRange.Resolution)} and {nameof(DateRange.MarketCode)}");
        }
    }
}