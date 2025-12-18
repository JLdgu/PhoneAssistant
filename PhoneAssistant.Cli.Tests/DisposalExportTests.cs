using FluentResults;

namespace PhoneAssistant.Cli.Tests;

public sealed class DisposalExportTests()
{
    [Test]
    public async Task AddRow_ShouldAddHeaderAndDataRow_WhenFirstRowAddedAsync()
    {
        var export = new DisposalExport([], [], new DirectoryInfo("c:"));

        export.AddRow(new("name", 5));

        await Assert.That(export.RowCount).IsEqualTo(1);
        var header = export.DisposalsSheet.GetRow(0);
        await Assert.That(header.GetCell(DisposalExport.Name.Item1).StringCellValue).IsEqualTo(DisposalExport.Name.Item2);
        await Assert.That(header.GetCell(DisposalExport.Name.Item1).StringCellValue).IsEqualTo(DisposalExport.Name.Item2);
        await Assert.That(header.GetCell(DisposalExport.Owner.Item1).StringCellValue).IsEqualTo(DisposalExport.Owner.Item2);
        await Assert.That(header.GetCell(DisposalExport.Status.Item1).StringCellValue).IsEqualTo(DisposalExport.Status.Item2);
        await Assert.That(header.GetCell(DisposalExport.SubLocation.Item1).StringCellValue).IsEqualTo(DisposalExport.SubLocation.Item2);
        var actualRow = export.DisposalsSheet.GetRow(1);
        await Assert.That(actualRow.GetCell(DisposalExport.Name.Item1).StringCellValue).IsEqualTo("name");
        await Assert.That(actualRow.GetCell(DisposalExport.Owner.Item1).StringCellValue).IsEqualTo("");
        await Assert.That(actualRow.GetCell(DisposalExport.Status.Item1).StringCellValue).IsEqualTo("Disposed");
        await Assert.That(actualRow.GetCell(DisposalExport.SubLocation.Item1).StringCellValue).IsEqualTo("");
        var notesHeader = export.NotesSheet.GetRow(0);
        await Assert.That(notesHeader.GetCell(DisposalExport.Name.Item1).StringCellValue).IsEqualTo(DisposalExport.Name.Item2);
        await Assert.That(notesHeader.GetCell(DisposalExport.Notes.Item1).StringCellValue).IsEqualTo(DisposalExport.Notes.Item2);
        var notesRow = export.NotesSheet.GetRow(1);
        await Assert.That(notesRow.GetCell(DisposalExport.Name.Item1).StringCellValue).IsEqualTo("name");
        await Assert.That(notesRow.GetCell(DisposalExport.Notes.Item1).StringCellValue).IsEqualTo("SCC Certificate # 5");
    }

    [Test]
    public async Task AddRow_ShouldOnlyAddDataRow_WhenSubsequentRowsAddedAsync()
    {
        var export = new DisposalExport([], [], new DirectoryInfo("c:"));
        export.AddRow(new("name", 6));
        export.AddRow(new("name2", 7));

        await Assert.That(export.RowCount).IsEqualTo(2);
        var actualRow = export.DisposalsSheet.GetRow(2);
        await Assert.That(actualRow.GetCell(DisposalExport.Name.Item1).StringCellValue).IsEqualTo("name2");
        await Assert.That(actualRow.GetCell(DisposalExport.Owner.Item1).StringCellValue).IsEqualTo("");
        await Assert.That(actualRow.GetCell(DisposalExport.Status.Item1).StringCellValue).IsEqualTo("Disposed");
        await Assert.That(actualRow.GetCell(DisposalExport.SubLocation.Item1).StringCellValue).IsEqualTo("");
        var notesRow = export.NotesSheet.GetRow(2);
        await Assert.That(notesRow.GetCell(DisposalExport.Name.Item1).StringCellValue).IsEqualTo("name2");
        await Assert.That(notesRow.GetCell(DisposalExport.Notes.Item1).StringCellValue).IsEqualTo($"SCC Certificate # 7");
    }

    [Test]
    [MethodDataSource(nameof(NotFoundOrStatusDisposedTestData))]
    public async Task GetDevice_ShouldFail_WhenNotFoundOrStatusDisposedAsync(SccDisposal disposal, string status)
    {
        var disposals = new List<SccDisposal>();
        var devices = new List<Device>() { new("name", "assetTag", "serialNumber", status) };
        var export = new DisposalExport(disposals, devices, new DirectoryInfo("c:"));

        Result<ExcelRow> result = export.GetDevice(disposal);

        await Assert.That(result.IsFailed).IsTrue();
    }
    public static IEnumerable<Func<(SccDisposal disposal, string status)>> NotFoundOrStatusDisposedTestData()
    {
        yield return () => (new SccDisposal("notfound", null, 0), "");
        yield return () => (new SccDisposal("notfound", "notfound", 1), "");
        yield return () => (new SccDisposal("name", null, 2), "Disposed");
        yield return () => (new SccDisposal("assetTag", null, 3), "Disposed");
        yield return () => (new SccDisposal("serialNumber", null, 4), "Disposed");
        yield return () => (new SccDisposal("notfound", "name", 5), "Disposed");
        yield return () => (new SccDisposal("notfound", "assetTag", 6), "Disposed");
        yield return () => (new SccDisposal("notfound", "serialNumber", 7), "Disposed");
    }

    [Test]
    [MethodDataSource(nameof(FoundTestData))]
    public async Task GetDeviceShouldSucceedWhenFoundAsync(SccDisposal disposal)
    {
        var disposals = new List<SccDisposal>();
        var devices = new List<Device>() { new("name", "assetTag", "serialNumber", "status") };
        var export = new DisposalExport(disposals, devices, new DirectoryInfo("c:"));

        Result<ExcelRow> result = export.GetDevice(disposal);

        await Assert.That(result.IsSuccess).IsTrue();
        await Assert.That(result.Value.Certificate).IsEqualTo(disposal.Certificate);
    }
    public static IEnumerable<Func<SccDisposal>> FoundTestData()
    {
        yield return () => new SccDisposal("name", null, 1);
        yield return () => new SccDisposal("assetTag", null, 2);
        yield return () => new SccDisposal("serialNumber", null, 3);
        yield return () => new SccDisposal("notfound", "name", 4);
        yield return () => new SccDisposal("notfound", "assetTag", 5);
        yield return () => new SccDisposal("notfound", "serialNumber", 6);
    }
}
