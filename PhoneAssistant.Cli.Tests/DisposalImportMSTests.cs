using FluentResults;
using System.Data;

namespace PhoneAssistant.Cli.Tests;

public sealed class DisposalImportMSTests
{
    private static System.Data.DataRow SetupRow(string itemType, string name, string assetTag, string status, string oem, string model, string serialNumber)
    {
        var table = new DataTable();
        for (int i = 0; i < 11; i++)
        {
            table.Columns.Add($"Col{i}", typeof(object));
        }
        var row = table.NewRow();
        row[DisposalImportMS.ItemType] = itemType;
        row[DisposalImportMS.Name] = name;
        row[DisposalImportMS.AssetTag] = assetTag;
        row[DisposalImportMS.Status] = status;
        row[DisposalImportMS.OEM] = oem;
        row[DisposalImportMS.Model] = model;
        row[DisposalImportMS.SerialNumber] = serialNumber;
        table.Rows.Add(row);
        return row;
    }

    [Test]
    public async Task GetDevice_ShouldFail_WhenItemTypeToBeIgnoredAsync()
    {
        var row = SetupRow("Item Type", "", "", "", "", "", "");

        Result<Device> actual = DisposalImportMS.GetDevice(row);

        await Assert.That(actual.IsFailed).IsTrue();
        await Assert.That(actual.Errors.First().Message).IsEqualTo("Ignore: Item Type");
    }

    [Test]
    public async Task GetDevice_ShouldFail_WhenItemTypePhoneAndModelToBeIgnoredAsync()
    {
        var row = SetupRow("Phone", "", "", "", "oem", "SIM Card", "");

        Result<Device> actual = DisposalImportMS.GetDevice(row);

        await Assert.That(actual.IsFailed).IsTrue();
        await Assert.That(actual.Errors.First().Message).IsEqualTo("Ignore: Model");
    }

    [Test]
    public async Task GetDevice_ShouldFail_WhenItemTypeComputerAndOEMInvalidAsync()
    {
        var row = SetupRow("Computer", "", "", "", "oem", "", "");

        Result<Device> actual = DisposalImportMS.GetDevice(row);

        await Assert.That(actual.IsFailed).IsTrue();
        await Assert.That(actual.Errors.First().Message).IsEqualTo("Ignore: Manufacturer");
    }

    [Test]
    [Arguments("Computer", "computer", "asset tag", "status", "Apple", "model", "serial")]
    [Arguments("Computer", "computer", "", "", "Apple", "model", "")]
    [Arguments("Phone", "phone", "asset tag", "status", "oem", "model", "serial")]
    [Arguments("Phone", "phone", "", "", "oem", "model", "")]
    public async Task GetDevice_ShouldSucceed_WhenValidDeviceAsync(string itemType, string name, string assetTag, string status, string oem, string model, string serialNumber)
    {
        var row = SetupRow(itemType, name, assetTag, status, oem, model, serialNumber);

        Result<Device> result = DisposalImportMS.GetDevice(row);
        Device device = result.Value;

        await Assert.That(result.IsSuccess).IsTrue();
        await Assert.That(device.Name).IsEqualTo(name);
        await Assert.That(device.AssetTag).IsEqualTo(assetTag);
    }
}
