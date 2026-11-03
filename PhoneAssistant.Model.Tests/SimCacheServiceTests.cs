using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace PhoneAssistant.Model.Tests;

internal sealed class SimCacheServiceTests
{
    [Test]
    internal async Task RefreshIfNewerAsync_should_seed_then_only_replace_cache_when_billing_period_advances()
    {
        using DbTestHelper sourceHelper = new();
        using SqliteConnection cacheConnection = new("Data Source=:memory:");
        await cacheConnection.OpenAsync();
        DbContextOptions<SimCacheDbContext> cacheOptions = new DbContextOptionsBuilder<SimCacheDbContext>()
            .UseSqlite(cacheConnection)
            .Options;
        await using SimCacheDbContext cacheDbContext = new(cacheOptions);

        Sim originalSim = new()
        {
            PhoneNumber = "100",
            BillingPeriod = "202601",
            SIMNumber = "SIM-100",
            UserName = "Original User",
            BroadbandData = 100,
            TextMessages = 10,
            VoiceCalls = 5
        };
        sourceHelper.DbContext.Sims.Add(originalSim);
        await sourceHelper.DbContext.SaveChangesAsync();

        SimCacheService service = new(sourceHelper.DbContext, cacheDbContext);

        await Assert.That(await service.RefreshIfNewerAsync()).IsTrue();
        await Assert.That(await cacheDbContext.Sims.CountAsync()).IsEqualTo(1);

        originalSim.UserName = "Changed User";
        await sourceHelper.DbContext.SaveChangesAsync();
        await Assert.That(await service.RefreshIfNewerAsync()).IsFalse();
        await Assert.That(await cacheDbContext.Sims.Select(sim => sim.UserName).SingleAsync())
            .IsEqualTo("Original User");

        sourceHelper.DbContext.Sims.Add(new Sim
        {
            PhoneNumber = "200",
            BillingPeriod = "202602",
            SIMNumber = "SIM-200",
            UserName = "New User",
            BroadbandData = 200,
            TextMessages = 20,
            VoiceCalls = 10
        });
        await sourceHelper.DbContext.SaveChangesAsync();

        await Assert.That(await service.RefreshIfNewerAsync()).IsTrue();
        await Assert.That(await cacheDbContext.Sims.CountAsync()).IsEqualTo(2);
        await Assert.That(await cacheDbContext.Sims
            .Where(sim => sim.PhoneNumber == "100")
            .Select(sim => sim.UserName)
            .SingleAsync()).IsEqualTo("Changed User");
    }
}
