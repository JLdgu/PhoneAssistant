using System.Text.Json.Serialization;

namespace Neurons.Api;

public sealed class ConfigurationItem
{
    public string? Name { get; init; }

    [JsonPropertyName("AC_PurchaseOrderID")]
    public string? AcPurchaseOrderId { get; init; }

    [JsonPropertyName("AC_PurchaseDate")]
    public string? AcPurchaseDate { get; init; }

    [JsonPropertyName("AC_WarrantyExpiry")]
    public string? AcWarrantyExpiry { get; init; }
    public string? AssetTag { get; init; }
    public string? CiType { get; init; }
    public string? Description { get; init; }
    public string? Manufacturer { get; init; }
    public string? Model { get; init; }
    public string? SerialNumber { get; init; }
    public string? Status { get; init; }
    public string? SubLocation { get; init; }
    public string? Supplier { get; init; }
    public string? TelephonyCiType { get; init; }
}

public sealed class ConfigurationItemClient(HttpClient httpClient)
{
    public async Task<ConfigurationItem?> GetByNameAsync(
        string name,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        string escapedName = name.Replace("'", "''", StringComparison.Ordinal);
        string url = $"businessobject/ci__phones?$filter=Name eq '{escapedName}'";

        using HttpResponseMessage response = await httpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();

        ConfigurationItemCollection? items = await response.Content.ReadFromJsonAsync(
            ConfigurationItemJsonContext.Default.ConfigurationItemCollection,
            cancellationToken);

        return items?.Value?.FirstOrDefault();
    }
}

internal sealed class ConfigurationItemCollection
{
    [JsonPropertyName("value")]
    public List<ConfigurationItem>? Value { get; init; }
}

[JsonSerializable(typeof(ConfigurationItemCollection))]
internal partial class ConfigurationItemJsonContext : JsonSerializerContext
{
}
