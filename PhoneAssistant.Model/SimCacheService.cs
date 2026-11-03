using Microsoft.EntityFrameworkCore;

namespace PhoneAssistant.Model;

public interface ISimCacheService
{
    Task<bool> RefreshIfNewerAsync();
}

public sealed class SimCacheService(
    PhoneAssistantDbContext sourceDbContext,
    SimCacheDbContext cacheDbContext) : ISimCacheService
{
    public async Task<bool> RefreshIfNewerAsync()
    {
        await cacheDbContext.Database.EnsureCreatedAsync();

        string? latestBillingPeriod = await sourceDbContext.Sims
            .Select(sim => sim.BillingPeriod)
            .MaxAsync();

        SimCacheState? cacheState = await cacheDbContext.CacheState
            .SingleOrDefaultAsync(state => state.Id == 1);
        if (cacheState is not null &&
            string.Compare(latestBillingPeriod, cacheState.LatestBillingPeriod, StringComparison.Ordinal) <= 0)
        {
            return false;
        }

        List<Sim> sims = await sourceDbContext.Sims
            .AsNoTracking()
            .ToListAsync();

        await using var transaction = await cacheDbContext.Database.BeginTransactionAsync();
        await cacheDbContext.Database.ExecuteSqlRawAsync("DELETE FROM \"Sims\";");
        await cacheDbContext.Database.ExecuteSqlRawAsync("DELETE FROM \"CacheState\";");
        cacheDbContext.ChangeTracker.Clear();
        cacheDbContext.Sims.AddRange(sims);
        cacheDbContext.CacheState.Add(new SimCacheState
        {
            Id = 1,
            LatestBillingPeriod = latestBillingPeriod
        });

        await cacheDbContext.SaveChangesAsync();
        await transaction.CommitAsync();
        return true;
    }
}
