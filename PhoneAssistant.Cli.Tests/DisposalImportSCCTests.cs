using FluentResults;
using System.Data;

namespace PhoneAssistant.Cli.Tests;

public sealed class DisposalImportSCCTests()
{
    private static System.Data.DataRow CreateTestRow(params object?[] values)
    {
        var table = new DataTable();
        for (int i = 0; i < 10; i++)
        {
            table.Columns.Add($"Col{i}", typeof(object));
        }
        var row = table.NewRow();
        for (int i = 0; i < values.Length && i < table.Columns.Count; i++)
        {
            row[i] = values[i] ?? DBNull.Value;
        }
        table.Rows.Add(row);
        return row;
    }

    [Test]
    public async Task GetDisposal_ShouldFail_WhenAccountCellNotD1024CTAsync()
    {
        var row = CreateTestRow("not expected value");

        Result<SccDisposal> result = DisposalImportSCC.GetDisposal(row);

        await Assert.That(result.IsFailed).IsTrue();
        await Assert.That(result.Errors[0].Message).IsEqualTo("Ignore: Account not D1024CT");
    }

    [Test]
    public async Task GetDisposal_ShouldFail_WhenAccountCellNullAsync()
    {
        var row = CreateTestRow(null, null, "ignore");

        Result<SccDisposal> result = DisposalImportSCC.GetDisposal(row);

        await Assert.That(result.IsFailed).IsTrue();
        await Assert.That(result.Errors[0].Message).IsEqualTo("Ignore: Account not D1024CT");
    }

    [Test]
    public async Task GetDisposal_ShouldFail_WhenAccountCellTypeInvalidAsync()
    {
        var row1 = CreateTestRow(3.14);
        var row2 = CreateTestRow("3.14");

        Result<SccDisposal> number = DisposalImportSCC.GetDisposal(row1);
        Result<SccDisposal> formula = DisposalImportSCC.GetDisposal(row2);

        await Assert.That(number.IsFailed).IsTrue();
        await Assert.That(number.Errors.First().Message).IsEqualTo("Ignore: Account not D1024CT");
        await Assert.That(formula.IsFailed).IsTrue();
        await Assert.That(formula.Errors.First().Message).IsEqualTo("Ignore: Account not D1024CT");
    }

    [Test]
    public async Task GetDisposal_ShouldFail_WhenProductTypeToBeIgnoredAsync()
    {
        var row = CreateTestRow(
            "D1024CT",                          // Account = 0
            null,                               // pad = 1
            null,                               // TrackerId = 2
            "serialNumber",                     // SerialNumber = 3
            null,                               // AssetNumber = 4
            "MONITORS",                         // ProductType = 5
            null,                               // pad = 6
            null,                               // pad = 7
            "Despatched - Recycled");           // Status = 8

        Result<SccDisposal> result = DisposalImportSCC.GetDisposal(row);

        await Assert.That(result.IsFailed).IsTrue();
        await Assert.That(result.Errors[0].Message).IsEqualTo("Ignore: Invalid product type");
    }

    [Test]
    public async Task GetDisposal_ShouldFail_WhenRowNullAsync()
    {
        Result<SccDisposal> result = DisposalImportSCC.GetDisposal(null!);

        await Assert.That(result.IsFailed).IsTrue();
        await Assert.That(result.Errors[0].Message).IsEqualTo("Ignore: Null row");
    }

    [Test]
    [Arguments("NONE")]
    [Arguments("UNREADABLE")]
    public async Task GetDisposal_ShouldFail_WhenSerialNumberToBeIgnoredAsync(string serialNumber)
    {
        var row = CreateTestRow(
            "D1024CT",                          // Account = 0
            null,                               // pad = 1
            null,                               // TrackerId = 2
            serialNumber,                       // SerialNumber = 3
            null,                               // AssetNumber = 4
            null,                               // ProductType = 5
            null,                               // pad = 6
            null,                               // pad = 7
            "Despatched - Recycled");           // Status = 8

        Result<SccDisposal> result = DisposalImportSCC.GetDisposal(row);

        await Assert.That(result.IsFailed).IsTrue();
        await Assert.That(result.Errors[0].Message).IsEqualTo("Ignore: Unidentifiable");
    }

    [Test]
    [Arguments("To be Sold")]
    [Arguments("On Hold")]
    public async Task GetDisposal_ShouldFail_WhenStatusNotDespatched(string status)
    {
        var row = CreateTestRow(
            "D1024CT",                          // Account = 0
            null,                               // pad = 1
            42,                                 // TrackerId = 2
            "123456789012345",                  // SerialNumber = 3
            "NONE",                             // AssetNumber = 4
            "productType",                      // ProductType = 5
            null,                               // pad = 6
            null,                               // pad = 7
            status);                            // Status = 8

        Result<SccDisposal> result = DisposalImportSCC.GetDisposal(row);

        await Assert.That(result.IsFailed).IsTrue();
        await Assert.That(result.Errors[0].Message).IsEqualTo("Ignore: Status not despatched");
    }

    [Test]
    [Arguments("123456789012345", "NONE", null, 15, "Despatched - Recycled / Disposed")]
    [Arguments("123456789054321", "PC12345", "PC12345", 16, "Despatched - Sold")]
    public async Task GetDisposal_ShouldSucceed(string serialNumber, string? assetNumber, string? expectedAssetNumber, int certificate, string status)
    {
        var row = CreateTestRow(
            "D1024CT",                          // Account = 0
            null,                               // pad = 1
            certificate,                        // TrackerId = 2
            serialNumber,                       // SerialNumber = 3
            assetNumber,                        // AssetNumber = 4
            "productType",                      // ProductType = 5
            null,                               // pad = 6
            null,                               // pad = 7
            status);                            // Status = 8

        Result<SccDisposal> result = DisposalImportSCC.GetDisposal(row);

        SccDisposal actual = result.Value;
        await Assert.That(result.IsSuccess).IsTrue();
        await Assert.That(actual.PrimaryKey).IsEqualTo(serialNumber);
        await Assert.That(actual.SecondaryKey).IsEqualTo(expectedAssetNumber);
        await Assert.That(actual.Certificate).IsEqualTo(certificate);
    }
}
