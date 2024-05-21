using Microsoft.EntityFrameworkCore;

using PhoneAssistant.WPF.Application.Entities;

namespace PhoneAssistant.WPF.Application.Repositories;

public sealed class ImportHistoryRepository(PhoneAssistantDbContext dbContext) : IImportHistoryRepository
{
    private readonly PhoneAssistantDbContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    public async Task<ImportHistory> CreateAsync(ImportType importType, string file)
    {
        ImportHistory importHistory = new() { Name = importType, File = file, ImportDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") };
        _dbContext.Imports.Add(importHistory);
        await _dbContext.SaveChangesAsync();
        return importHistory;
    }

    public async Task<ImportHistory?> GetLatestImportAsync(ImportType importType)
    {
        return await _dbContext.Imports
            .OrderByDescending(i => i.ImportDate)
            .FirstOrDefaultAsync(i => i.Name == importType);
    }
}
