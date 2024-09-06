using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Serilog;

namespace Import;

public class ImportDbContext : DbContext
{
    public ImportDbContext(DbContextOptions options)
        : base(options) { }

    public DbSet<BaseReport> BaseReport => Set<BaseReport>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
#if DEBUG
        optionsBuilder.LogTo(Log.Information, LogLevel.None);
        //optionsBuilder.EnableDetailedErrors();
        //optionsBuilder.EnableSensitiveDataLogging();        
#endif
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BaseReport>(entity =>
        {
            entity.HasKey(e => e.PhoneNumber);
        });
    }
}
