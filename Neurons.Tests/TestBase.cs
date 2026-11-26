using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;

namespace Neurons.Tests;

internal abstract class TestBase : IAsyncDisposable
{
    private static long _nextNumber = 999900000000;
    protected string UniqueId { get; } = Interlocked.Increment(ref _nextNumber).ToString();

    protected HttpClient HttpClient { get; init; }

    protected TestBase()
    {
        IConfiguration config = new ConfigurationBuilder()
            .AddUserSecrets<TestBase>(optional: false)  // For local dev
            .Build();

        string odataUrl = $"https://{config["Uat:Url"]}/api/odata/";
        string? apiKey = config["Uat:Key"];

        HttpClient = new HttpClient()
        {
            BaseAddress = new Uri(odataUrl),
            DefaultRequestHeaders =
            {
                Accept = { new MediaTypeWithQualityHeaderValue("application/json") }
            }
        };
        HttpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"rest_api_key={apiKey}");

    }

    public ValueTask DisposeAsync()
    {
        HttpClient.Dispose();
        return ValueTask.CompletedTask;
    }
}
