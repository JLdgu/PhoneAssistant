namespace PhoneAssistant.Model.Tests; 

public class ImportHistoryRepositoryTests
{
    readonly DbTestHelper _helper = new();
    readonly ImportHistoryRepository _repository;

    public ImportHistoryRepositoryTests()
    {
        _repository = new(_helper.DbContext);
    }

    [Test]
    async Task GetLatestImport_WhenImportExists_ShouldReturnLatest()
    {
        ImportHistory import1 = new() {Id = 1, Name = ImportType.BaseReport, File = "Import 1", ImportDate = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss") };
        ImportHistory import2 = new() {Id = 2, Name = ImportType.BaseReport, File = "Import 2", ImportDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") };

        _helper.DbContext.Imports.Add(import1);
        _helper.DbContext.Imports.Add(import2);
        await _helper.DbContext.SaveChangesAsync();

        ImportHistory? actual = await _repository.GetLatestImportAsync(ImportType.BaseReport);

        await Assert.That(actual).IsEqualTo(import2);
    }
}
