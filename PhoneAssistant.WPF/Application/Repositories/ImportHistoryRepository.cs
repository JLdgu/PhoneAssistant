using PhoneAssistant.WPF.Application.Entities;

namespace PhoneAssistant.WPF.Application.Repositories;

public sealed class ImportHistoryRepository(PhoneAssistantDbContext dbContext)
{
    private readonly PhoneAssistantDbContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    public async Task<ImportHistory> CreateAsync(string file)
    {
        ImportHistory importHistory = new() { Name = ImportType.BaseReport, File = file, ImportDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") };
        _dbContext.Imports.Add(importHistory);
        await _dbContext.SaveChangesAsync();
        return importHistory;
    }

    public ImportHistory? GetLatestImport()
    {
        return _dbContext.Imports.OrderByDescending(i => i.ImportDate).FirstOrDefault();
    }
}
