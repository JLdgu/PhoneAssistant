using Microsoft.EntityFrameworkCore;

namespace PhoneAssistant.Model;

public sealed class ImportHistoryRepository(PhoneAssistantDbContext dbContext) : IImportHistoryRepository
{
    private readonly PhoneAssistantDbContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    public async Task<ImportHistory> CreateAsync(ImportType importType, string run)
    {
        ImportHistory importHistory = new() { Name = importType, Run = run, ImportDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") };
        _dbContext.Imports.Add(importHistory);
        await _dbContext.SaveChangesAsync();
        return importHistory;
    }

    public async Task<bool> RunExistsAsync(ImportType importType, string run)
    {
        ImportHistory? importHistory = await _dbContext.Imports.FirstOrDefaultAsync(i => i.Name == importType && i.Run == run);
        return importHistory is not null;
    }

    public async Task<ImportHistory?> GetLatestImportAsync(ImportType importType)
    {
        return await _dbContext.Imports
            .OrderByDescending(i => i.Run)
            .FirstOrDefaultAsync(i => i.Name == importType);
    }
}
