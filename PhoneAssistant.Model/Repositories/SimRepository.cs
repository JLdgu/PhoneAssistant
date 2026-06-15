using Microsoft.EntityFrameworkCore;

namespace PhoneAssistant.Model;

public interface ISimRepository
{
    Task CreateAsync(Sim sim);
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
        Sim? sim = await dbContext.Sims.FindAsync(phoneNumber);

        return sim?.SIMNumber;
    }
}
