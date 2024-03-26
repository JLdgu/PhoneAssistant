using Microsoft.EntityFrameworkCore;
using PhoneAssistant.WPF.Application.Entities;

namespace PhoneAssistant.WPF.Application.Repositories;

public sealed class ImportHistoryRepository(PhoneAssistantDbContext dbContext)
{
    private readonly PhoneAssistantDbContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    public async Task<IEnumerable<EEBaseReport>> GetBaseReportAsync()
    {
        List<EEBaseReport> report = await _dbContext.BaseReport.AsNoTracking().ToListAsync();
        return report;
    }

    public async Task CreateAsync(ImportHistory import)
    {
        _dbContext.Imports.Add(import);
        await _dbContext.SaveChangesAsync();
    }
}
