using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace PhoneAssistant.Model;

public sealed class SimCacheDbContext(DbContextOptions<SimCacheDbContext> options) : DbContext(options)
{
    public DbSet<Sim> Sims => Set<Sim>();

    internal DbSet<SimCacheState> CacheState => Set<SimCacheState>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.CommandExecuted));
        base.OnConfiguring(optionsBuilder);
    }

    public static string GetDatabasePath()
    {
#if DEBUG
        const string databaseName = "PA-debug.db";
#else
        const string databaseName = "PA-release.db";
#endif
        string directory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "PhoneAssistantCache");
        Directory.CreateDirectory(directory);
        return Path.Combine(directory, databaseName);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Sim>(entity =>
        {
            entity.HasKey(sim => new { sim.PhoneNumber, sim.BillingPeriod });
            entity.Property(sim => sim.Esim).HasColumnName("eSIM");
        });

        modelBuilder.Entity<SimCacheState>(entity =>
        {
            entity.ToTable("CacheState");
            entity.HasKey(state => state.Id);
        });
    }
}

internal sealed class SimCacheState
{
    public int Id { get; set; }
    public string? LatestBillingPeriod { get; set; }
}
