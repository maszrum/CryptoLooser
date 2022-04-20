using System.Collections.Immutable;
using CryptoLooser.Core.Models;

namespace CryptoLooser.Core.Interfaces;

public interface IAvailableDateRangesRepository
{
    Task<ImmutableArray<DateRange>> GetAvailableDateRanges(
        MarketCode marketCode, 
        ChartResolution chartResolution);
}