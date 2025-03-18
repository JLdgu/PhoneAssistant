using FluentResults;

using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Reconcile.Tests;
public sealed class ImportSCCTests()
{
    [Test]
    public async Task GetDisposal_ShouldFail_WhenAccountCellNotD1024CTAsync()
    {
        using XSSFWorkbook workbook = new();
        ISheet sheet = workbook.CreateSheet("Data");
        IRow row = sheet.CreateRow(0);
        row.CreateCell(ImportSCC.Account).SetCellValue("not expected value");

        Result<Disposal> result = ImportSCC.GetDisposal(row);

        await Assert.That(result.IsFailed).IsTrue();
        await Assert.That(result.Errors.First().Message).IsEqualTo("Ignore: Account not D1024CT");
    }

    [Test]
    public async Task GetDisposal_ShouldFail_WhenAccountCellNullAsync()
    {
        using XSSFWorkbook workbook = new();
        ISheet sheet = workbook.CreateSheet("Data");
        IRow row = sheet.CreateRow(0);
        row.CreateCell(ImportSCC.Account + 1).SetCellValue("ignore");

        Result<Disposal> result = ImportSCC.GetDisposal(row);

        await Assert.That(result.IsFailed).IsTrue();
        await Assert.That(result.Errors.First().Message).IsEqualTo("Ignore: Account not D1024CT");
    }

    [Test]
    public async Task GetDisposal_ShouldFail_WhenAccountCellTypeInvalidAsync()
    {
        using XSSFWorkbook workbook = new();
        ISheet sheet = workbook.CreateSheet("Data");
        IRow row0 = sheet.CreateRow(0);
        row0.CreateCell(ImportSCC.Account).SetCellFormula("IF(TRUE,15,20)");
        IRow row1 = sheet.CreateRow(1);
        row1.CreateCell(ImportSCC.Account).SetCellValue(3.14);

        Result<Disposal> formula = ImportSCC.GetDisposal(row0);
        Result<Disposal> number = ImportSCC.GetDisposal(row1);

        await Assert.That(formula.IsFailed).IsTrue();
        await Assert.That(formula.Errors.First().Message).IsEqualTo("Ignore: Account not D1024CT");
        await Assert.That(number.IsFailed).IsTrue();
        await Assert.That(number.Errors.First().Message).IsEqualTo("Ignore: Account not D1024CT");
    }

    [Test]
    public async Task GetDisposal_ShouldFail_WhenProductTypeToBeIgnoredAsync()
    {
        using XSSFWorkbook workbook = new();
        ISheet sheet = workbook.CreateSheet("Data");
        IRow row = sheet.CreateRow(0);
        row.CreateCell(ImportSCC.Account).SetCellValue("D1024CT");
        row.CreateCell(ImportSCC.ProductType).SetCellValue("MONITORS");
        row.CreateCell(ImportSCC.SerialNumber).SetCellValue("serialNumber");

        Result<Disposal> result = ImportSCC.GetDisposal(row);

        await Assert.That(result.IsFailed).IsTrue();
        await Assert.That(result.Errors.First().Message).IsEqualTo("Ignore: Invalid product type");
    }

    [Test]
    public async Task GetDisposal_ShouldFail_WhenRowNullAsync()
    {
        using XSSFWorkbook workbook = new();
        ISheet sheet = workbook.CreateSheet("Data");
        IRow row = sheet.GetRow(0);

        Result<Disposal> result = ImportSCC.GetDisposal(row);

        await Assert.That(result.IsFailed).IsTrue();
        await Assert.That(result.Errors.First().Message).IsEqualTo("Ignore: Null row");
    }

    [Test]
    [Arguments("NONE")]
    [Arguments("UNREADABLE")]
    public async Task GetDisposal_ShouldFail_WhenSerialNumberToBeIgnoredAsync(string serialNumber)
    {
        using XSSFWorkbook workbook = new();
        ISheet sheet = workbook.CreateSheet("Data");
        IRow row = sheet.CreateRow(0);
        row.CreateCell(ImportSCC.Account).SetCellValue("D1024CT");
        row.CreateCell(ImportSCC.SerialNumber).SetCellValue(serialNumber);

        Result<Disposal> result = ImportSCC.GetDisposal(row);

        await Assert.That(result.IsFailed).IsTrue();
        await Assert.That(result.Errors.First().Message).IsEqualTo("Ignore: Unidentifiable");
    }

    [Test]
    [Arguments("123456789012345", "NONE", null, 15)]
    [Arguments("123456789054321", "PC12345", "PC12345", 16)]
    public async Task GetDisposal_ShouldSucceedAsync(string serialNumber, string? assetNumber, string? expectedAssetNumber, int certificate)
    {
        using XSSFWorkbook workbook = new();
        ISheet sheet = workbook.CreateSheet("Data");
        IRow row = sheet.CreateRow(0);
        row.CreateCell(ImportSCC.Account).SetCellValue("D1024CT");
        row.CreateCell(ImportSCC.AssetNumber).SetCellValue(assetNumber);
        row.CreateCell(ImportSCC.ProductType).SetCellValue("productType");
        row.CreateCell(ImportSCC.SerialNumber).SetCellValue(serialNumber);
        row.CreateCell(ImportSCC.TrackerId).SetCellValue(certificate);

        Result<Disposal> result = ImportSCC.GetDisposal(row);

        Disposal actual = result.Value;
        await Assert.That(result.IsSuccess).IsTrue();
        await Assert.That(actual.PrimaryKey).IsEqualTo(serialNumber);
        await Assert.That(actual.SecondaryKey).IsEqualTo(expectedAssetNumber);
        await Assert.That(actual.Certificate).IsEqualTo(certificate);
    }
}
