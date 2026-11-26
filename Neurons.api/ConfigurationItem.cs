using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Neurons.Api;

public sealed class ConfigurationItem
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? RecId { get; init; }

    public string? Name { get; init; }

    [JsonPropertyName("AC_PurchaseOrderID")]
    public string? AcPurchaseOrderId { get; init; }

    [JsonPropertyName("AC_PurchaseDate")]
    public string? AcPurchaseDate { get; init; }

    [JsonPropertyName("AC_WarrantyExpiry")]
    public string? AcWarrantyExpiry { get; init; }

    public string? AssetTag { get; init; }

    [JsonPropertyName("CIType")]
    public string? CiType { get; init; }

    public string? Description { get; init; }

    public string? Manufacturer { get; init; }

    public string? Model { get; init; }

    public string? SerialNumber { get; init; }

    public string? Status { get; init; }

    public string? SubLocation { get; init; }

    public string? Supplier { get; init; }

    [JsonPropertyName("TelephonyCIType")]
    public string? TelephonyCiType { get; init; }
}

public sealed class ConfigurationItemClient(HttpClient httpClient)
{
    public async Task<ConfigurationItem> CreateAsync(
        ConfigurationItem configurationItem,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(configurationItem);

        using HttpResponseMessage response = await httpClient.PostAsJsonAsync(
            "businessobject/ci__phones",
            configurationItem,
            ConfigurationItemJsonContext.Default.ConfigurationItem,
            cancellationToken);
        response.EnsureSuccessStatusCode();

        ConfigurationItem? createdItem = await response.Content.ReadFromJsonAsync(
            ConfigurationItemJsonContext.Default.ConfigurationItem,
            cancellationToken);
        return createdItem
            ?? throw new InvalidOperationException("The create response did not contain a configuration item.");
    }

    public async Task<ConfigurationItem?> GetByNameAsync(
        string name,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        string escapedName = name.Replace("'", "''", StringComparison.Ordinal);
        string url = $"businessobject/ci__phones?$filter=Name eq '{escapedName}'";

        using HttpResponseMessage response = await httpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();

        string content = await response.Content.ReadAsStringAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(content))
        {
            return null;
        }

        ConfigurationItemCollection? items = JsonSerializer.Deserialize(
            content,
            ConfigurationItemJsonContext.Default.ConfigurationItemCollection);

        return items?.Value?.FirstOrDefault();
    }

    public async Task UpdateTelephonyCITypeAsync(
        string recId,
        string telephonyCiType,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(recId);
        ArgumentException.ThrowIfNullOrWhiteSpace(telephonyCiType);

        string escapedRecId = recId.Replace("'", "''", StringComparison.Ordinal);
        string url = $"businessobject/ci__phones('{escapedRecId}')";
        using HttpResponseMessage response = await httpClient.PatchAsJsonAsync(
            url,
            new TelephonyCITypeUpdate(telephonyCiType),
            ConfigurationItemJsonContext.Default.TelephonyCITypeUpdate,
            cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteAsync(
        string recId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(recId);

        string escapedRecId = recId.Replace("'", "''", StringComparison.Ordinal);
        string url = $"businessobject/ci__phones('{escapedRecId}')";
        using HttpResponseMessage response = await httpClient.DeleteAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}

internal sealed class ConfigurationItemCollection
{
    [JsonPropertyName("value")]
    public List<ConfigurationItem>? Value { get; init; }
}

internal sealed record TelephonyCITypeUpdate(
    string TelephonyCIType);

[JsonSerializable(typeof(ConfigurationItemCollection))]
[JsonSerializable(typeof(ConfigurationItem))]
[JsonSerializable(typeof(TelephonyCITypeUpdate))]
internal partial class ConfigurationItemJsonContext : JsonSerializerContext
{
}
