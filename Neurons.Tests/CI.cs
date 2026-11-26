using System.Text.Json;
using Neurons.Api;
using TUnit.Core.Logging;

namespace Neurons.Tests;

[Category("Neurons")]
internal sealed class CI : TestBase
{
    [Test]
    internal async Task GetPhoneConfigurationItemByName_ReturnsCI()
    {
        ConfigurationItemClient client = new(HttpClient);
        ConfigurationItem? configurationItem = await client.GetByNameAsync("350868238038075");

        await Assert.That(configurationItem).IsNotNull();
        await Assert.That(configurationItem!.Name).IsNotNullOrEmpty();
        await Assert.That(configurationItem.SerialNumber).IsEqualTo("G41D7KVH46");

        Console.WriteLine(configurationItem.AssetTag);
        Console.WriteLine(configurationItem.CiType);
    }

    [Test]
    internal async Task CreateAndUpdatePhoneConfigurationItem_DeletesCI()
    {
        ConfigurationItemClient client = new(HttpClient);
        ConfigurationItem? createdItem = null;

        try
        {
            createdItem = await client.CreateAsync(new ConfigurationItem
            {
                Name = UniqueId,
                AcPurchaseOrderId = "130010227",
                AcPurchaseDate = "2025-09-05",
                AcWarrantyExpiry = "2025-07-22",
                AssetTag = "MP90001",
                CiType = "Phone",
                Description = "Apple iPhone 16e",
                Manufacturer = "Apple",
                Model = "iPhone 16E",
                SerialNumber = "G41D7KVH46",
                Status = "In Stock",
                SubLocation = "L87 CH",
                Supplier = "EE",
                TelephonyCiType = "SIM Card"
            });

            await Assert.That(createdItem.RecId).IsNotNullOrEmpty();
            await Assert.That(createdItem.TelephonyCiType).IsEqualTo("SIM Card");

            await client.UpdateTelephonyCITypeAsync(createdItem.RecId!, "Tablet");
        }
        finally
        {
            if (!string.IsNullOrWhiteSpace(createdItem?.RecId))
            {
                await client.DeleteAsync(createdItem.RecId);
            }
        }
    }

    [Test]
    internal async Task GetIncidentByNumber_ReturnsValidResponse()
    {
        // Arrange
        string url = "businessobject/incidents?$filter=IncidentNumber eq 1339884";

        // Act
        var response = await HttpClient.GetAsync(url);
        Console.WriteLine(response.Content);

        // Assert
        await Assert.That(response.IsSuccessStatusCode).IsTrue();
        var content = await response.Content.ReadAsStringAsync();
        await Assert.That(content).IsNotNullOrEmpty();

        // Optionally, parse JSON and check structure
        using var doc = JsonDocument.Parse(content);
        var root = doc.RootElement;
        // Assuming the response has a structure, e.g., value array
        await Assert.That(root.TryGetProperty("value", out var value)).IsTrue();
        await Assert.That(value.ValueKind).IsEqualTo(JsonValueKind.Array);
        foreach (var incident in value.EnumerateArray())
        {
            string owner = incident.TryGetProperty("Owner", out var ownerElement) &&
                ownerElement.ValueKind != JsonValueKind.Null
                ? ownerElement.ToString()
                : "null";
            Console.WriteLine(owner);
        }
    }

    [Test]
    internal async Task GetIncidentByNumber_ReturnsExpectedComputer()
    {
        string url = "businessobject/incidents?$filter=IncidentNumber eq 1339884";

        var response = await HttpClient.GetAsync(url);
        await Assert.That(response.IsSuccessStatusCode).IsTrue();

        var content = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(content);
        var root = doc.RootElement;
        await Assert.That(root.TryGetProperty("value", out var value)).IsTrue();
        await Assert.That(value.ValueKind).IsEqualTo(JsonValueKind.Array);

        bool containsExpectedComputer = false;
        var logger = TestContext.Current!.GetDefaultLogger();

        foreach (var incident in value.EnumerateArray())
        {
            var Owner = incident.TryGetProperty("Owner", out var ownerElement) && ownerElement.ValueKind != JsonValueKind.Null
                ? ownerElement.ToString() : "null";
            logger.LogInformation($"Owner: {Owner}");

            if (incident.TryGetProperty("CauseCode", out var ciType) &&
                ciType.ValueKind == JsonValueKind.String &&
                ciType.GetString() == "Advice Given")
            {
                containsExpectedComputer = true;
                break;
            }
        }

        await Assert.That(containsExpectedComputer).IsTrue();
    }
}
