using Blockify.Infrastructure.Tools.Extensions;

namespace Blockify.Infrastructure.External;

public class ExternalClient
{
    protected readonly HttpClient HttpClient;

    protected ExternalClient(HttpClient client)
    {
        HttpClient = client;
    }
    
    protected virtual async Task VerifyResponseAsync(HttpResponseMessage response, HttpRequestMessage request)
    {
        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = await response.GetErrorMessageAsync();
            var uri = request.RequestUri?.ToString();
            
            throw new Exception($"Error on request to {uri}: {errorMessage}");
        }
    }
}