using FluentResults;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace PhoneAssistant.Cli.Tests;

public sealed class DisposalImportMSTests
{
    [Test]
    public async Task GetDevice_ShouldFail_WhenItemTypeToBeIgnoredAsync()
    {
        IRow row = SetupRow("Item Type", "", "", "", "", "", "");

        Result<Device> actual = DisposalImportMS.GetDevice(row);

        await Assert.That(actual.IsFailed).IsTrue();
        await Assert.That(actual.Errors.First().Message).IsEqualTo("Ignore: Item Type");
    }

    [Test]
    public async Task GetDevice_ShouldFail_WhenItemTypePhoneAndModelToBeIgnoredAsync()
    {
        IRow row = SetupRow("Phone", "", "", "", "oem", "SIM Card", "");

        Result<Device> actual = DisposalImportMS.GetDevice(row);

        await Assert.That(actual.IsFailed).IsTrue();
        await Assert.That(actual.Errors.First().Message).IsEqualTo("Ignore: Model");
    }

    [Test]
    public async Task GetDevice_ShouldFail_WhenItemTypeComputerAndOEMInvalidAsync()
    {
        IRow row = SetupRow("Computer", "", "", "", "oem", "", "");

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
        IRow row = SetupRow(itemType, name, assetTag, status, oem, model, serialNumber);

        Result<Device> result = DisposalImportMS.GetDevice(row);
        Device device = result.Value;

        await Assert.That(result.IsSuccess).IsTrue();
        await Assert.That(device.Name).IsEqualTo(name);
        await Assert.That(device.AssetTag).IsEqualTo(assetTag);
    }

    private static IRow SetupRow(string itemType, string name, string assetTag, string status, string oem, string model, string serialNumber)
    {
        using XSSFWorkbook workbook = new();
        ISheet sheet = workbook.CreateSheet("Data");
        IRow row = sheet.CreateRow(0);
        row.CreateCell(DisposalImportMS.ItemType).SetCellValue(itemType);
        row.CreateCell(DisposalImportMS.Name).SetCellValue(name);
        row.CreateCell(DisposalImportMS.AssetTag).SetCellValue(assetTag);
        row.CreateCell(DisposalImportMS.Status).SetCellValue(status);
        row.CreateCell(DisposalImportMS.OEM).SetCellValue(oem);
        row.CreateCell(DisposalImportMS.Model).SetCellValue(model);
        row.CreateCell(DisposalImportMS.SerialNumber).SetCellValue(serialNumber);
        return row;
    }
}
