using FluentAssertions;
using FluentResults;

using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Reconcile.Tests;
public sealed class ImportSCCTests()
{
    [Fact]
    public void GetDisposal_ShouldFail_WhenAccountCellNotD1024CT()
    {
        using XSSFWorkbook workbook = new();
        ISheet sheet = workbook.CreateSheet("Data");
        IRow row = sheet.CreateRow(0);
        row.CreateCell(ImportSCC.Account).SetCellValue("not expected value");

        Result<Disposal> result = ImportSCC.GetDisposal(row);

        result.IsFailed.Should().BeTrue();
        result.Errors.First().Message.Should().Be("Ignore: Account not D1024CT");
    }

    [Fact]
    public void GetDisposal_ShouldFail_WhenAccountCellNull()
    {
        using XSSFWorkbook workbook = new();
        ISheet sheet = workbook.CreateSheet("Data");
        IRow row = sheet.CreateRow(0);
        row.CreateCell(ImportSCC.Account + 1).SetCellValue("ignore");

        Result<Disposal> result = ImportSCC.GetDisposal(row);

        result.IsFailed.Should().BeTrue();
        result.Errors.First().Message.Should().Be("Ignore: Account not D1024CT");
    }

    [Fact]
    public void GetDisposal_ShouldFail_WhenAccountCellTypeInvalid()
    {
        using XSSFWorkbook workbook = new();
        ISheet sheet = workbook.CreateSheet("Data");
        IRow row0 = sheet.CreateRow(0);
        row0.CreateCell(ImportSCC.Account).SetCellFormula("IF(TRUE,15,20)");
        IRow row1 = sheet.CreateRow(1);
        row1.CreateCell(ImportSCC.Account).SetCellValue(3.14);

        Result<Disposal> formula = ImportSCC.GetDisposal(row0);
        Result<Disposal> number = ImportSCC.GetDisposal(row1);

        formula.IsFailed.Should().BeTrue();
        formula.Errors.First().Message.Should().Be("Ignore: Account not D1024CT");
        number.IsFailed.Should().BeTrue();
        number.Errors.First().Message.Should().Be("Ignore: Account not D1024CT");
    }

    [Fact]
    public void GetDisposal_ShouldFail_WhenRowNull()
    {
        using XSSFWorkbook workbook = new();
        ISheet sheet = workbook.CreateSheet("Data");
        IRow row = sheet.GetRow(0);

        Result<Disposal> result = ImportSCC.GetDisposal(row);

        result.IsFailed.Should().BeTrue();
        result.Errors.First().Message.Should().Be("Ignore: Null row");
    }

    [Theory]
    [InlineData("NONE")]
    [InlineData("UNREADABLE")]
    public void GetDisposal_ShouldFail_WhenSerialNumberToBeIgnored(string serialNumber)
    {
        using XSSFWorkbook workbook = new();
        ISheet sheet = workbook.CreateSheet("Data");
        IRow row = sheet.CreateRow(0);
        row.CreateCell(ImportSCC.Account).SetCellValue("D1024CT");
        row.CreateCell(ImportSCC.SerialNumber).SetCellValue(serialNumber);

        Result<Disposal> result = ImportSCC.GetDisposal(row);

        result.IsFailed.Should().BeTrue();
        result.Errors.First().Message.Should().Be("Ignore: Unidentifiable");
    }

    [Theory]
    [InlineData("123456789012345", "NONE", null, 15)]
    [InlineData("123456789054321", "PC12345", "PC12345", 16)]
    public void GetDisposal_ShouldSucceed(string serialNumber, string? assetNumber, string? expectedAssetNumber, int certificate)
    {
        using XSSFWorkbook workbook = new();
        ISheet sheet = workbook.CreateSheet("Data");
        IRow row = sheet.CreateRow(0);
        row.CreateCell(ImportSCC.Account).SetCellValue("D1024CT");
        row.CreateCell(ImportSCC.TrackerId).SetCellValue(certificate);
        row.CreateCell(ImportSCC.SerialNumber).SetCellValue(serialNumber);
        row.CreateCell(ImportSCC.AssetNumber).SetCellValue(assetNumber);

        Result<Disposal> result = ImportSCC.GetDisposal(row);

        Disposal actual = result.Value;
        result.IsSuccess.Should().BeTrue();
        actual.PrimaryKey.Should().Be(serialNumber);
        actual.SecondaryKey.Should().Be(expectedAssetNumber);
        actual.Certificate.Should().Be(certificate);
    }
}
