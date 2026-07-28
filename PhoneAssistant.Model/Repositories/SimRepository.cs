using System.Numerics;

using Microsoft.EntityFrameworkCore;

namespace PhoneAssistant.Model;

public interface ISimRepository
{
    Task CreateAsync(Sim sim);
    Task<string> GetLatestBillingPeriod();
    Task<IEnumerable<Sim>> GetSimsForPhoneNumber(string phoneNumber);
    Task<IEnumerable<Sim>> GetSimsForSimNumber(string simNumber);
    Task<IEnumerable<Sim>> GetSimsForUserName(string userName);
    Task<string?> GetSimNumber(string phoneNumber);
    Task UpdateOrCreateAsync(Sim sim);
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

    public async Task<IEnumerable<string>> GetEsims()
    {
        IEnumerable<string> phoneNumbers = await dbContext.Sims
            .Where(p => p.Esim == true)
            .AsNoTracking()
            .Select(s => s.PhoneNumber)
            .Distinct()
            .ToListAsync();

        return phoneNumbers;
    }

    public async Task<string?> GetSimNumber(string phoneNumber)
    {
        string? simNumber = await dbContext.Sims
            .Where(p => p.PhoneNumber == phoneNumber)
            .OrderByDescending(p => p.BillingPeriod)
            .AsNoTracking()
            .Select(s => s.SIMNumber)
            .FirstOrDefaultAsync();

        return simNumber;
    }

    public async Task<IEnumerable<Sim>> GetSimsForPhoneNumber(string phoneNumber)
    {
        IEnumerable<Sim> sims = await dbContext.Sims
            .Where(p => p.PhoneNumber.Contains(phoneNumber))
            .OrderByDescending(p => p.BillingPeriod)
            .AsNoTracking()
            .ToListAsync();
        return sims;
    }

    public async Task<IEnumerable<Sim>> GetSimsForSimNumber(string simNumber)
    {
        IEnumerable<Sim> sims = await dbContext.Sims
            .Where(s => s.SIMNumber.Contains(simNumber))
            .OrderByDescending(s => s.BillingPeriod)
            .AsNoTracking()
            .ToListAsync();
        return sims;
    }

    public async Task<IEnumerable<Sim>> GetSimsForUserName(string userName)
    {
        IEnumerable<Sim> sims = await dbContext.Sims
            .Where(s => s.UserName.Contains(userName))
            .OrderByDescending(s => s.BillingPeriod)
            .AsNoTracking()
            .ToListAsync();
        return sims;
    }

    public async Task UpdateOrCreateAsync(Sim sim)
    {
        Sim? dbSim = await dbContext.Sims.FindAsync(sim.PhoneNumber, sim.BillingPeriod);
        if (dbSim is  null)
        {
            await CreateAsync(sim);
            return;
        }
        dbSim.SIMNumber = sim.SIMNumber;
        dbSim.UserName = sim.UserName;
        dbSim.BroadbandData = sim.BroadbandData;
        dbSim.TextMessages = sim.TextMessages;
        dbSim.VoiceCalls = sim.VoiceCalls;
        await dbContext.SaveChangesAsync();
    }
}
