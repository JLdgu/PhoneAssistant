using FluentResults;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace PhoneAssistant.Cli.Tests;

public sealed class DisposalImportSCCTests()
{
    [Test]
    public async Task GetDisposal_ShouldFail_WhenAccountCellNotD1024CTAsync()
    {
        using XSSFWorkbook workbook = new();
        ISheet sheet = workbook.CreateSheet("Data");
        IRow row = sheet.CreateRow(0);
        row.CreateCell(DisposalImportSCC.Account).SetCellValue("not expected value");

        Result<SccDisposal> result = DisposalImportSCC.GetDisposal(row);

        await Assert.That(result.IsFailed).IsTrue();
        await Assert.That(result.Errors[0].Message).IsEqualTo("Ignore: Account not D1024CT");
    }

    [Test]
    public async Task GetDisposal_ShouldFail_WhenAccountCellNullAsync()
    {
        using XSSFWorkbook workbook = new();
        ISheet sheet = workbook.CreateSheet("Data");
        IRow row = sheet.CreateRow(0);
        row.CreateCell(DisposalImportSCC.Account + 1).SetCellValue("ignore");

        Result<SccDisposal> result = DisposalImportSCC.GetDisposal(row);

        await Assert.That(result.IsFailed).IsTrue();
        await Assert.That(result.Errors[0].Message).IsEqualTo("Ignore: Account not D1024CT");
    }

    [Test]
    public async Task GetDisposal_ShouldFail_WhenAccountCellTypeInvalidAsync()
    {
        using XSSFWorkbook workbook = new();
        ISheet sheet = workbook.CreateSheet("Data");
        IRow row0 = sheet.CreateRow(0);
        row0.CreateCell(DisposalImportSCC.Account).SetCellFormula("IF(TRUE,15,20)");
        IRow row1 = sheet.CreateRow(1);
        row1.CreateCell(DisposalImportSCC.Account).SetCellValue(3.14);

        Result<SccDisposal> formula = DisposalImportSCC.GetDisposal(row0);
        Result<SccDisposal> number = DisposalImportSCC.GetDisposal(row1);

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
        row.CreateCell(DisposalImportSCC.Account).SetCellValue("D1024CT");
        row.CreateCell(DisposalImportSCC.ProductType).SetCellValue("MONITORS");
        row.CreateCell(DisposalImportSCC.SerialNumber).SetCellValue("serialNumber");

        Result<SccDisposal> result = DisposalImportSCC.GetDisposal(row);

        await Assert.That(result.IsFailed).IsTrue();
        await Assert.That(result.Errors[0].Message).IsEqualTo("Ignore: Invalid product type");
    }

    [Test]
    public async Task GetDisposal_ShouldFail_WhenRowNullAsync()
    {
        using XSSFWorkbook workbook = new();
        ISheet sheet = workbook.CreateSheet("Data");
        IRow row = sheet.GetRow(0);

        Result<SccDisposal> result = DisposalImportSCC.GetDisposal(row);

        await Assert.That(result.IsFailed).IsTrue();
        await Assert.That(result.Errors[0].Message).IsEqualTo("Ignore: Null row");
    }

    [Test]
    [Arguments("NONE")]
    [Arguments("UNREADABLE")]
    public async Task GetDisposal_ShouldFail_WhenSerialNumberToBeIgnoredAsync(string serialNumber)
    {
        using XSSFWorkbook workbook = new();
        ISheet sheet = workbook.CreateSheet("Data");
        IRow row = sheet.CreateRow(0);
        row.CreateCell(DisposalImportSCC.Account).SetCellValue("D1024CT");
        row.CreateCell(DisposalImportSCC.SerialNumber).SetCellValue(serialNumber);

        Result<SccDisposal> result = DisposalImportSCC.GetDisposal(row);

        await Assert.That(result.IsFailed).IsTrue();
        await Assert.That(result.Errors[0].Message).IsEqualTo("Ignore: Unidentifiable");
    }

    [Test]
    [Arguments("To be Sold")]
    [Arguments("On Hold")]
    public async Task GetDisposal_ShouldFail_WhenStatusNotDespatched(string status)
    {
        using XSSFWorkbook workbook = new();
        ISheet sheet = workbook.CreateSheet("Data");
        IRow row = sheet.CreateRow(0);
        row.CreateCell(DisposalImportSCC.Account).SetCellValue("D1024CT");
        row.CreateCell(DisposalImportSCC.AssetNumber).SetCellValue("NONE");
        row.CreateCell(DisposalImportSCC.SerialNumber).SetCellValue(123456789012345);
        row.CreateCell(DisposalImportSCC.ProductType).SetCellValue("productType");
        row.CreateCell(DisposalImportSCC.TrackerId).SetCellValue(42);
        row.CreateCell(DisposalImportSCC.Status).SetCellValue(status);

        Result<SccDisposal> result = DisposalImportSCC.GetDisposal(row);

        await Assert.That(result.IsFailed).IsTrue();
        await Assert.That(result.Errors[0].Message).IsEqualTo("Ignore: Status not despatched");
    }


    [Test]
    [Arguments("123456789012345", "NONE", null, 15, "Despatched - Recycled / Disposed")]
    [Arguments("123456789054321", "PC12345", "PC12345", 16, "Despatched - Sold")]
    public async Task GetDisposal_ShouldSucceed(string serialNumber, string? assetNumber, string? expectedAssetNumber, int certificate, string status)
    {
        using XSSFWorkbook workbook = new();
        ISheet sheet = workbook.CreateSheet("Data");
        IRow row = sheet.CreateRow(0);
        row.CreateCell(DisposalImportSCC.Account).SetCellValue("D1024CT");
        row.CreateCell(DisposalImportSCC.AssetNumber).SetCellValue(assetNumber);
        row.CreateCell(DisposalImportSCC.ProductType).SetCellValue("productType");
        row.CreateCell(DisposalImportSCC.SerialNumber).SetCellValue(serialNumber);
        row.CreateCell(DisposalImportSCC.TrackerId).SetCellValue(certificate);
        row.CreateCell(DisposalImportSCC.Status).SetCellValue(status);

        Result<SccDisposal> result = DisposalImportSCC.GetDisposal(row);

        SccDisposal actual = result.Value;
        await Assert.That(result.IsSuccess).IsTrue();
        await Assert.That(actual.PrimaryKey).IsEqualTo(serialNumber);
        await Assert.That(actual.SecondaryKey).IsEqualTo(expectedAssetNumber);
        await Assert.That(actual.Certificate).IsEqualTo(certificate);
    }
}
