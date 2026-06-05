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
        var headerCell = export.DisposalsSheet.Cell(1, DisposalExport.Name.Item1 + 1).Value;
        await Assert.That(headerCell.ToString()).IsEqualTo(DisposalExport.Name.Item2);
        var ownerCell = export.DisposalsSheet.Cell(1, DisposalExport.Owner.Item1 + 1).Value;
        await Assert.That(ownerCell.ToString()).IsEqualTo(DisposalExport.Owner.Item2);
        var statusCell = export.DisposalsSheet.Cell(1, DisposalExport.Status.Item1 + 1).Value;
        await Assert.That(statusCell.ToString()).IsEqualTo(DisposalExport.Status.Item2);
        var subLocCell = export.DisposalsSheet.Cell(1, DisposalExport.SubLocation.Item1 + 1).Value;
        await Assert.That(subLocCell.ToString()).IsEqualTo(DisposalExport.SubLocation.Item2);

        var actualRow = export.DisposalsSheet.Row(2);
        var nameCell = actualRow.Cell(DisposalExport.Name.Item1 + 1).Value;
        await Assert.That(nameCell.ToString()).IsEqualTo("name");
        var ownerDataCell = actualRow.Cell(DisposalExport.Owner.Item1 + 1).Value;
        await Assert.That(ownerDataCell.ToString()).IsEqualTo("");
        var statusDataCell = actualRow.Cell(DisposalExport.Status.Item1 + 1).Value;
        await Assert.That(statusDataCell.ToString()).IsEqualTo("Disposed");
        var subLocDataCell = actualRow.Cell(DisposalExport.SubLocation.Item1 + 1).Value;
        await Assert.That(subLocDataCell.ToString()).IsEqualTo("");

        var notesHeader = export.NotesSheet.Row(1);
        var notesNameCell = notesHeader.Cell(DisposalExport.Name.Item1 + 1).Value;
        await Assert.That(notesNameCell.ToString()).IsEqualTo(DisposalExport.Name.Item2);
        var notesCell = notesHeader.Cell(DisposalExport.Notes.Item1 + 1).Value;
        await Assert.That(notesCell.ToString()).IsEqualTo(DisposalExport.Notes.Item2);

        var notesRow = export.NotesSheet.Row(2);
        var notesRowName = notesRow.Cell(DisposalExport.Name.Item1 + 1).Value;
        await Assert.That(notesRowName.ToString()).IsEqualTo("name");
        var notesRowValue = notesRow.Cell(DisposalExport.Notes.Item1 + 1).Value;
        await Assert.That(notesRowValue.ToString()).IsEqualTo("SCC Certificate # 5");
    }

    [Test]
    public async Task AddRow_ShouldOnlyAddDataRow_WhenSubsequentRowsAddedAsync()
    {
        var export = new DisposalExport([], [], new DirectoryInfo("c:"));
        export.AddRow(new("name", 6));
        export.AddRow(new("name2", 7));

        await Assert.That(export.RowCount).IsEqualTo(2);
        var actualRow = export.DisposalsSheet.Row(3);
        var nameCell = actualRow.Cell(DisposalExport.Name.Item1 + 1).Value;
        await Assert.That(nameCell.ToString()).IsEqualTo("name2");
        var ownerCell = actualRow.Cell(DisposalExport.Owner.Item1 + 1).Value;
        await Assert.That(ownerCell.ToString()).IsEqualTo("");
        var statusCell = actualRow.Cell(DisposalExport.Status.Item1 + 1).Value;
        await Assert.That(statusCell.ToString()).IsEqualTo("Disposed");
        var subLocCell = actualRow.Cell(DisposalExport.SubLocation.Item1 + 1).Value;
        await Assert.That(subLocCell.ToString()).IsEqualTo("");

        var notesRow = export.NotesSheet.Row(3);
        var notesNameCell = notesRow.Cell(DisposalExport.Name.Item1 + 1).Value;
        await Assert.That(notesNameCell.ToString()).IsEqualTo("name2");
        var notesValue = notesRow.Cell(DisposalExport.Notes.Item1 + 1).Value;
        await Assert.That(notesValue.ToString()).IsEqualTo($"SCC Certificate # 7");
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
