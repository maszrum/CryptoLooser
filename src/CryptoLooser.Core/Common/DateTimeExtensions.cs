using CryptoLooser.Core.Models;

namespace CryptoLooser.Core.Common;

public static class DateTimeExtensions
{
    public static bool IsBetweenInclusive(this DateTime dateTime, DateRange dateRange) => 
        dateTime >= dateRange.From && dateTime <= dateRange.To;
    
    public static bool IsBetweenExclusive(this DateTime dateTime, DateRange dateRange) => 
        dateTime > dateRange.From && dateTime < dateRange.To;
}