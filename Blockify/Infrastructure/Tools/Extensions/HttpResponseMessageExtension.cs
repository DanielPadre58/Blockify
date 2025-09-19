using System.Text.Json;

namespace Blockify.Infrastructure.Tools.Extensions;
public static class HttpResponseMessageExtension
{
    public static int? GetRetryAfterSeconds(this HttpResponseMessage response)
    {
        if (response.Headers.TryGetValues("Retry-After", out var values))
        {
            var retryAfterValue = values.FirstOrDefault();
            if (int.TryParse(retryAfterValue, out int seconds))
            {
                return seconds;
            }
        }

        return null;
    }

    public static async Task<string> GetErrorMessageAsync(this HttpResponseMessage response)
    {
        try
        {
            var content = await response.Content.ReadAsStringAsync();
            if (!string.IsNullOrWhiteSpace(content))
            {
                using var doc = JsonDocument.Parse(content);
                if (doc.RootElement.TryGetProperty("error", out var error))
                {
                    if (error.TryGetProperty("message", out var message))
                        return message.GetString() ?? response.ReasonPhrase ?? "Unknown error";
                }
            }
        }
        catch{}

        return "Error message not provided on response";
    }
} 
