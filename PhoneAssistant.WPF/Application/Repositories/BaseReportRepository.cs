using Microsoft.EntityFrameworkCore;
using PhoneAssistant.WPF.Application.Entities;

namespace PhoneAssistant.WPF.Application.Repositories;

public sealed class BaseReportRepository(PhoneAssistantDbContext dbContext)
{
    private readonly PhoneAssistantDbContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    public async Task<IEnumerable<BaseReport>> GetBaseReportAsync()
    {
        List<BaseReport> report = await _dbContext.BaseReport.AsNoTracking().ToListAsync();
        return report;
    }

    public async Task CreateAsync(BaseReport report)
    {
        _dbContext.BaseReport.Add(report);
        await _dbContext.SaveChangesAsync();
    }

    public async Task TruncateAsync()
    {
        await _dbContext.Database.ExecuteSqlRawAsync("DELETE FROM BaseReport;");
        await _dbContext.Database.ExecuteSqlRawAsync("VACUUM;");
    }
}
