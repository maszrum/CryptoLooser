namespace CryptoLooser.ZondaExchange.ApiClient;

internal static class DateTimeExtensions
{
    public static long ToUnixTimeMilliseconds(this DateTime dateTime) =>
        ((DateTimeOffset) dateTime.ToUniversalTime()).ToUnixTimeMilliseconds();
    
    public static DateTime FromUnixMillisecondsToDateTime(this long milliseconds) => 
        DateTimeOffset.FromUnixTimeMilliseconds(milliseconds).LocalDateTime;
}