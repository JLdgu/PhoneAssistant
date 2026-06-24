using Microsoft.EntityFrameworkCore;

namespace PhoneAssistant.Model;

public interface ISimRepository
{
    Task CreateAsync(Sim sim);
    Task<string> GetLatestBillingPeriod();
    Task<IEnumerable<Sim>> GetSim(string phoneNumber);
    Task<string?> GetSimNumberAsync(string phoneNumber);
}

public sealed class SimRepository(PhoneAssistantDbContext dbContext) : ISimRepository
{
    public async Task CreateAsync(Sim sim)
    {
        dbContext.Sims.Add(sim);
        await dbContext.SaveChangesAsync();
    }

    public async Task<string> GetLatestBillingPeriod()
    {
        string? latestBillingPeriod = await dbContext.Sims
            .OrderByDescending(s => s.BillingPeriod)
            .Select(s => s.BillingPeriod)
            .FirstOrDefaultAsync();
        return latestBillingPeriod ?? "Unknown";
    }

    public async Task<IEnumerable<Sim>> GetSim(string phoneNumber)
    {
        IEnumerable<Sim> sims = await dbContext.Sims
            .Where(p => p.PhoneNumber == phoneNumber)
            .OrderByDescending(p => p.BillingPeriod)
            .AsNoTracking()
            .ToListAsync();
        return sims;
    }

    public async Task<string?> GetSimNumberAsync(string phoneNumber)
    {
        string? simNumber = await dbContext.Sims
            .Where(p => p.PhoneNumber == phoneNumber)
            .OrderByDescending(p => p.BillingPeriod)
            .AsNoTracking()
            .Select(s => s.SIMNumber)
            .FirstOrDefaultAsync();

        return simNumber;
    }
}
