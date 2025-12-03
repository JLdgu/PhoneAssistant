using FluentResults;

using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace PhoneAssistant.Cli.Tests;

public sealed class DisposalImportTests
{
    [Test]
    public async Task IsValidSheet_ShouldFail_WhenCheckCellInvalidAsync()
    {
        using XSSFWorkbook workbook = new();
        ISheet sheet = workbook.CreateSheet("SheetName");
        IRow row = sheet.CreateRow(0);
        row.CreateCell(0).SetCellValue("invalid");

        Result<ISheet> actual = DisposalImport.IsValidSheet(workbook, "SheetName" , "CheckValue", "A1");

        await Assert.That(actual.IsFailed).IsTrue();
        await Assert.That(actual.Errors.First().Message).IsEqualTo("Unable to find 'CheckValue' in cell A1");
    }

    [Test]
    public async Task IsValidSheet_ShouldFail_WhenNoRowsAsync()
    {
        using XSSFWorkbook workbook = new();
        _ = workbook.CreateSheet("SheetName");

        Result<ISheet> actual = DisposalImport.IsValidSheet(workbook, "SheetName", "CheckValue", "A1");

        await Assert.That(actual.IsFailed).IsTrue();
        await Assert.That(actual.Errors.First().Message).IsEqualTo("No rows found in sheet Data");
    }

    [Test]
    public async Task IsValidSheet_ShouldFail_WhenSheetNameInvalidAsync()
    {
        using XSSFWorkbook workbook = new();
        workbook.CreateSheet("invalid");

        Result<ISheet> actual = DisposalImport.IsValidSheet(workbook, "SheetName", "CheckValue", "A1");

        await Assert.That(actual.IsFailed).IsTrue();
        await Assert.That(actual.Errors.First().Message).IsEqualTo($"First sheet in workbook not named SheetName");
    }

    [Test]
    public async Task IsValidSheet_ShouldSucceed_WhenValidSheetAsync()
    {
        using XSSFWorkbook workbook = new();
        ISheet sheet = workbook.CreateSheet("SheetName");
        IRow row = sheet.CreateRow(0);
        row.CreateCell(DisposalImportMS.Category).SetCellValue("CheckValue");

        Result<ISheet> actual = DisposalImport.IsValidSheet(workbook, "SheetName", "CheckValue", "A1");

        await Assert.That(actual.IsSuccess).IsTrue();
    }
}
