using FluentResults;

namespace Reconcile.Tests;

public sealed class ExportTests()
{
    [Test]
    public async Task AddRow_ShouldAddHeaderAndDataRow_WhenFirstRowAddedAsync()
    {
        var export = new Export(0, [], [], new DirectoryInfo("c:"));

        export.AddRow(new("name", 5));

        await Assert.That(export.RowCount).IsEqualTo(1);
        var header = export.DisposalsSheet.GetRow(0);
        await Assert.That(header.GetCell(Export.Name.Item1).StringCellValue).IsEqualTo(Export.Name.Item2);
        await Assert.That(header.GetCell(Export.Owner.Item1).StringCellValue).IsEqualTo(Export.Owner.Item2);
        await Assert.That(header.GetCell(Export.Status.Item1).StringCellValue).IsEqualTo(Export.Status.Item2);
        await Assert.That(header.GetCell(Export.SubLocation.Item1).StringCellValue).IsEqualTo(Export.SubLocation.Item2);
        var actualRow = export.DisposalsSheet.GetRow(1);
        await Assert.That(actualRow.GetCell(Export.Name.Item1).StringCellValue).IsEqualTo("name");
        await Assert.That(actualRow.GetCell(Export.Owner.Item1).StringCellValue).IsEqualTo("");
        await Assert.That(actualRow.GetCell(Export.Status.Item1).StringCellValue).IsEqualTo("Disposed");
        await Assert.That(actualRow.GetCell(Export.SubLocation.Item1).StringCellValue).IsEqualTo("");
        var notesHeader = export.NotesSheet.GetRow(0);
        await Assert.That(notesHeader.GetCell(Export.Name.Item1).StringCellValue).IsEqualTo(Export.Name.Item2);
        await Assert.That(notesHeader.GetCell(Export.Notes.Item1).StringCellValue).IsEqualTo(Export.Notes.Item2);
        var notesRow = export.NotesSheet.GetRow(1);
        await Assert.That(notesRow.GetCell(Export.Name.Item1).StringCellValue).IsEqualTo("name");
        await Assert.That(notesRow.GetCell(Export.Notes.Item1).StringCellValue).IsEqualTo("SCC Certificate # 5");
    }

    [Test]
    public async Task AddRow_ShouldOnlyAddDataRow_WhenSubsequentRowsAddedAsync()
    {
        var export = new Export(0, [], [], new DirectoryInfo("c:"));
        export.AddRow(new("name", 6));
        export.AddRow(new("name2", 7));

        await Assert.That(export.RowCount).IsEqualTo(2);
        var actualRow = export.DisposalsSheet.GetRow(2);
        await Assert.That(actualRow.GetCell(Export.Name.Item1).StringCellValue).IsEqualTo("name2");
        await Assert.That(actualRow.GetCell(Export.Owner.Item1).StringCellValue).IsEqualTo("");
        await Assert.That(actualRow.GetCell(Export.Status.Item1).StringCellValue).IsEqualTo("Disposed");
        await Assert.That(actualRow.GetCell(Export.SubLocation.Item1).StringCellValue).IsEqualTo("");
        var notesRow = export.NotesSheet.GetRow(2);
        await Assert.That(notesRow.GetCell(Export.Name.Item1).StringCellValue).IsEqualTo("name2");
        await Assert.That(notesRow.GetCell(Export.Notes.Item1).StringCellValue).IsEqualTo($"SCC Certificate # 7");
    }

    [Test]
    [MethodDataSource(nameof(NotFoundOrStatusDisposedTestData))]
    public async Task GetDevice_ShouldFail_WhenNotFoundOrStatusDisposedAsync(Disposal disposal, string status)
    {
        var disposals = new List<Disposal>();
        var devices = new List<Device>() { new("name", "assetTag", "serialNumber", status) };
        var export = new Export(0, disposals, devices, new DirectoryInfo("c:"));

        Result<ExcelRow> result = export.GetDevice(disposal);

        await Assert.That(result.IsFailed).IsTrue();
    }
    public static IEnumerable<Func<(Disposal disposal, string status)>> NotFoundOrStatusDisposedTestData()
    {
        yield return () => (new Disposal("notfound", null, 0), "");
        yield return () => (new Disposal("notfound", "notfound", 1), "");
        yield return () => (new Disposal("name", null, 2), "Disposed");
        yield return () => (new Disposal("assetTag", null, 3), "Disposed");
        yield return () => (new Disposal("serialNumber", null, 4), "Disposed");
        yield return () => (new Disposal("notfound", "name", 5), "Disposed");
        yield return () => (new Disposal("notfound", "assetTag", 6), "Disposed");
        yield return () => (new Disposal("notfound", "serialNumber", 7), "Disposed");
    }

    [Test]
    [MethodDataSource(nameof(FoundTestData))]
    public async Task GetDeviceShouldSucceedWhenFoundAsync(Disposal disposal)
    {
        var disposals = new List<Disposal>();
        var devices = new List<Device>() { new("name", "assetTag", "serialNumber", "status") };
        var export = new Export(0, disposals, devices, new DirectoryInfo("c:"));

        Result<ExcelRow> result = export.GetDevice(disposal);

        await Assert.That(result.IsSuccess).IsTrue();
        await Assert.That(result.Value.Certificate).IsEqualTo(disposal.Certificate);
    }
    public static IEnumerable<Func<Disposal>> FoundTestData()
    {
        yield return () => new Disposal("name", null, 1);
        yield return () => new Disposal("assetTag", null, 2);
        yield return () => new Disposal("serialNumber", null, 3);
        yield return () => new Disposal("notfound", "name", 4);
        yield return () => new Disposal("notfound", "assetTag", 5);
        yield return () => new Disposal("notfound", "serialNumber", 6);
    }
}
