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
    public async Task RunExistsAsync_ShouldReturnFalse_WhenRunDoesNotExist()
    {
        bool actual = await _repository.RunExistsAsync(ImportType.BaseReport, "DoesNotExist");

        await Assert.That(actual).IsFalse();
    }

    [Test]
    public async Task RunExistsAsync_ShouldReturnTrue_WhenRunExists()
    {
        _helper.DbContext.Imports.Add(new ImportHistory { Name = ImportType.BaseReport, Run = "202602", ImportDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") });
        await _helper.DbContext.SaveChangesAsync();

        bool actual = await _repository.RunExistsAsync(ImportType.BaseReport, "202602");

        await Assert.That(actual).IsTrue();
    }

    [Test]
    public async Task GetLatestImport_WhenImportExists_ShouldReturnLatest()
    {
        ImportHistory import1 = new() {Name = ImportType.BaseReport, Run = "202602", ImportDate = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss") };
        ImportHistory import2 = new() {Name = ImportType.BaseReport, Run = "202601", ImportDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") };

        _helper.DbContext.Imports.Add(import1);
        _helper.DbContext.Imports.Add(import2);
        await _helper.DbContext.SaveChangesAsync();

        ImportHistory? actual = await _repository.GetLatestImportAsync(ImportType.BaseReport);

        await Assert.That(actual).IsEqualTo(import1);
    }
}
