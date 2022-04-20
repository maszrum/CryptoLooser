namespace CryptoLooser.ZondaExchange.ApiClient.DTOs;

internal static class ApiResponseExtensions
{
    public static void EnsureStatusOk(this ApiResponse response)
    {
        if (!response.Status.Equals("Ok", StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                $"Returned status is not ok. It is {response.Status}");
        }
    }
}