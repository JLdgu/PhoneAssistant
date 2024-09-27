using System.ComponentModel.DataAnnotations;

using FluentResults;

using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Reconcile.Tests;

public class ImportMSTests
{
    [Fact]
    public void GetDataSheet_ShouldFail_WhenHeaderColumnsInvalid()
    {
        using XSSFWorkbook workbook = new();
        ISheet sheet = workbook.CreateSheet("Data");
        IRow row = sheet.CreateRow(0);
        row.CreateCell(0).SetCellValue("NotCategory");

        Result<ISheet> actual = ImportMS.GetDataSheet(workbook);

        Assert.True(actual.IsFailed);
        Assert.Equal("Unable to find 'Category' column, check you are using a myScomis export", actual.Errors.First().Message);
    }

    [Fact]
    public void GetDataSheet_ShouldFail_WhenNoRows()
    {
        using XSSFWorkbook workbook = new();
        workbook.CreateSheet("Data");

        Result<ISheet> actual = ImportMS.GetDataSheet(workbook);

        Assert.True(actual.IsFailed);
        Assert.Equal("No rows found in sheet Data", actual.Errors.First().Message);
    }

    [Fact]
    public void GetDataSheet_ShouldFail_WhenSheetNameNotData()
    {
        using XSSFWorkbook workbook = new();
        workbook.CreateSheet("Sheet 1");

        Result<ISheet> actual = ImportMS.GetDataSheet(workbook);

        Assert.True(actual.IsFailed);
        Assert.Equal("Unable to find sheet named Data in workbook", actual.Errors.First().Message);
    }

    [Fact]
    public void GetDataSheet_ShouldSucceed_WhenValidSheet()
    {
        using XSSFWorkbook workbook = new();
        ISheet sheet = workbook.CreateSheet("Data");
        IRow row = sheet.CreateRow(0);
        row.CreateCell(0).SetCellValue("Category");

        Result<ISheet> actual = ImportMS.GetDataSheet(workbook);

        Assert.True(actual.IsSuccess);
    }

    [Fact]
    public void GetDevice_ShouldFail_WhenInvalidItemType()
    {
        IRow row = SetupRow("Item Type", "", "", "", "", "");

        Result<Device> actual = ImportMS.GetDevice(row);

        Assert.True(actual.IsFailed);
        Assert.Equal("Invalid Item Type", actual.Errors.First().Message);
    }

    [Fact]
    public void GetDevice_ShouldFail_WhenItemTypeComputerAndOEMInvalid()
    {
        IRow row = SetupRow("Computer", "", "", "", "oem", "");

        Result<Device> actual = ImportMS.GetDevice(row);

        Assert.True(actual.IsFailed);
        Assert.Equal("Invalid Manufacturer", actual.Errors.First().Message);
    }

    [Theory]
    [InlineData("Computer", "computer", "asset tag", "status", "Apple", "serial")]
    [InlineData("Computer", "computer", "", "", "Apple", "")]
    [InlineData("Phone", "phone", "asset tag", "status", "oem", "serial")]
    [InlineData("Phone", "phone", "", "", "oem", "")]
    public void GetDevice_ShouldSucceed_WhenValidDevice(string itemType, string name, string assetTag, string status, string oem, string serialNumber)
    {
        IRow row = SetupRow(itemType, name, assetTag, status, oem, serialNumber);

        Result<Device> result = ImportMS.GetDevice(row);
        var device = result.Value;

        Assert.True(result.IsSuccess);
        Assert.Equal(name, device.Name);
        Assert.Equal(assetTag, device.AssetTag);
    }

    private static IRow SetupRow(string itemType, string name, string assetTag, string status, string oem, string serialNumber)
    {
        using XSSFWorkbook workbook = new();
        ISheet sheet = workbook.CreateSheet("Data");
        IRow row = sheet.CreateRow(0);
        row.CreateCell(1).SetCellValue(itemType);
        row.CreateCell(3).SetCellValue(name);
        row.CreateCell(4).SetCellValue(assetTag);
        row.CreateCell(7).SetCellValue(status);
        row.CreateCell(8).SetCellValue(oem);
        row.CreateCell(10).SetCellValue(oem);
        return row;
    }
}
