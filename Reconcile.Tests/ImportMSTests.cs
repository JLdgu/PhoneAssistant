using FluentAssertions;

using FluentResults;

using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Reconcile.Tests;

public sealed class ImportMSTests
{
    [Fact]
    public void GetDevice_ShouldFail_WhenItemTypeToBeIgnored()
    {
        IRow row = SetupRow("Item Type", "", "", "", "", "", "");

        Result<Device> actual = ImportMS.GetDevice(row);

        actual.IsFailed.Should().BeTrue();
        actual.Errors.First().Message.Should().Be("Ignore: Item Type");
    }

    [Fact]
    public void GetDevice_ShouldFail_WhenItemTypePhoneAndModelToBeIgnored()
    {
        IRow row = SetupRow("Phone", "", "", "", "oem", "SIM Card", "");

        Result<Device> actual = ImportMS.GetDevice(row);

        actual.IsFailed.Should().BeTrue();
        actual.Errors.First().Message.Should().Be("Ignore: Model");
    }

    [Fact]
    public void GetDevice_ShouldFail_WhenItemTypeComputerAndOEMInvalid()
    {
        IRow row = SetupRow("Computer", "", "", "", "oem", "", "");

        Result<Device> actual = ImportMS.GetDevice(row);

        actual.IsFailed.Should().BeTrue();
        actual.Errors.First().Message.Should().Be("Ignore: Manufacturer");
    }

    [Theory]
    [InlineData("Computer", "computer", "asset tag", "status", "Apple", "model", "serial")]
    [InlineData("Computer", "computer", "", "", "Apple", "model", "")]
    [InlineData("Phone", "phone", "asset tag", "status", "oem", "model", "serial")]
    [InlineData("Phone", "phone", "", "", "oem", "model", "")]
    public void GetDevice_ShouldSucceed_WhenValidDevice(string itemType, string name, string assetTag, string status, string oem, string model, string serialNumber)
    {
        IRow row = SetupRow(itemType, name, assetTag, status, oem, model, serialNumber);

        Result<Device> result = ImportMS.GetDevice(row);
        Device device = result.Value;

        result.IsSuccess.Should().BeTrue();
        device.Name.Should().Be(name);
        device.AssetTag.Should().Be(assetTag);
    }

    private static IRow SetupRow(string itemType, string name, string assetTag, string status, string oem, string model, string serialNumber)
    {
        using XSSFWorkbook workbook = new();
        ISheet sheet = workbook.CreateSheet("Data");
        IRow row = sheet.CreateRow(0);
        row.CreateCell(ImportMS.ItemType).SetCellValue(itemType);
        row.CreateCell(ImportMS.Name).SetCellValue(name);
        row.CreateCell(ImportMS.AssetTag).SetCellValue(assetTag);
        row.CreateCell(ImportMS.Status).SetCellValue(status);
        row.CreateCell(ImportMS.OEM).SetCellValue(oem);
        row.CreateCell(ImportMS.Model).SetCellValue(model);
        row.CreateCell(ImportMS.SerialNumber).SetCellValue(serialNumber);
        return row;
    }
}
