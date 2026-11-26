namespace Neurons.Api;

public class ConfigurationItem(HttpClient httpClient)
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task<string> GetDataAsync()
    {
        return await _httpClient.GetStringAsync("/api/data");
    }
}
