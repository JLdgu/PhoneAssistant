using PhoneAssistant.WPF.Application.Entities;

namespace PhoneAssistant.WPF.Application.Repositories;

public sealed class ImportHistoryRepository(PhoneAssistantDbContext dbContext)
{
    private readonly PhoneAssistantDbContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    public async Task CreateAsync(string file)
    {
        ImportHistory import = new() { Name = ImportType.BaseReport, File = file, ImportDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") };
        _dbContext.Imports.Add(import);
        await _dbContext.SaveChangesAsync();
    }

    public ImportHistory? GetLatestImport()
    {
        return _dbContext.Imports.OrderByDescending(i => i.ImportDate).FirstOrDefault();
    }
}
