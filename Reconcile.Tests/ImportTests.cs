using FluentAssertions;

using FluentResults;

using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Reconcile.Tests;

public sealed class ImportTests
{
    [Fact]
    public void IsValidSheet_ShouldFail_WhenCheckCellInvalid()
    {
        using XSSFWorkbook workbook = new();
        ISheet sheet = workbook.CreateSheet("SheetName");
        IRow row = sheet.CreateRow(0);
        row.CreateCell(0).SetCellValue("invalid");

        Result<ISheet> actual = Import.IsValidSheet(workbook, "SheetName" , "CheckValue", "A1");

        actual.IsFailed.Should().BeTrue();
        actual.Errors.First().Message.Should().Be("Unable to find 'CheckValue' in cell A1");
    }

    [Fact]
    public void IsValidSheet_ShouldFail_WhenNoRows()
    {
        using XSSFWorkbook workbook = new();
        _ = workbook.CreateSheet("SheetName");

        Result<ISheet> actual = Import.IsValidSheet(workbook, "SheetName", "CheckValue", "A1");

        actual.IsFailed.Should().BeTrue();
        actual.Errors.First().Message.Should().Be("No rows found in sheet Data");
    }

    [Fact]
    public void IsValidSheet_ShouldFail_WhenSheetNameInvalid()
    {
        using XSSFWorkbook workbook = new();
        workbook.CreateSheet("invalid");

        Result<ISheet> actual = Import.IsValidSheet(workbook, "SheetName", "CheckValue", "A1");

        actual.IsFailed.Should().BeTrue();
        actual.Errors.First().Message.Should().Be($"First sheet in workbook not named SheetName");
    }

    [Fact]
    public void IsValidSheet_ShouldSucceed_WhenValidSheet()
    {
        using XSSFWorkbook workbook = new();
        ISheet sheet = workbook.CreateSheet("SheetName");
        IRow row = sheet.CreateRow(0);
        row.CreateCell(ImportMS.Category).SetCellValue("CheckValue");

        Result<ISheet> actual = Import.IsValidSheet(workbook, "SheetName", "CheckValue", "A1");

        actual.IsSuccess.Should().BeTrue();
    }
}
