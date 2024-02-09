using Microsoft.EntityFrameworkCore;
using PhoneAssistant.WPF.Application.Entities;

namespace PhoneAssistant.WPF.Application.Repositories;

public sealed class BaseReportRepository(PhoneAssistantDbContext dbContext)
{
    private readonly PhoneAssistantDbContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    public async Task<IEnumerable<EEBaseReport>> GetBaseReportAsync()
    {
        List<EEBaseReport> report = await _dbContext.BaseReport.AsNoTracking().ToListAsync();
        return report;
    }

    public async Task CreateAsync(EEBaseReport report)
    {
        await _dbContext.BaseReport.AddAsync(report);
        await _dbContext.SaveChangesAsync();
    }

    public async Task TruncateAsync()
    {
        await _dbContext.Database.ExecuteSqlRawAsync("DELETE FROM BaseReport;");
        await _dbContext.Database.ExecuteSqlRawAsync("VACUUM;");
    }
}
