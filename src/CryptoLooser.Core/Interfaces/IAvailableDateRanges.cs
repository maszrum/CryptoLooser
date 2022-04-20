using System.Collections.Immutable;
using CryptoLooser.Core.Models;

namespace CryptoLooser.Core.Interfaces;

public interface IAvailableDateRanges
{
    Task<ImmutableArray<DateRange>> GetAvailableDateRanges(
        MarketCode marketCode, 
        ChartResolution chartResolution);
}