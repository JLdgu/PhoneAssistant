using Microsoft.EntityFrameworkCore;

using PhoneAssistant.WPF.Application.Entities;

namespace PhoneAssistant.WPF.Application;

public sealed class v1PhoneAssistantDbContext : DbContext
{
    public DbSet<v1Phone> Phones => Set<v1Phone>();

    public DbSet<v1Sim> Sims => Set<v1Sim>();

    //public DbSet<Disposal> Disposals => Set<State>();

    public v1PhoneAssistantDbContext(DbContextOptions options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
            throw new ArgumentException("DbContextOptionsBuilder has not been configured");

#if DEBUG
        optionsBuilder.EnableSensitiveDataLogging();
#endif
      
        //optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<v1Phone>(
            p => 
            {
                p.HasKey(x => x.Imei);
                p.Property(x => x.SR).HasColumnName("SRNumber");
                p.Property(x => x.LastUpdate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

        modelBuilder.Entity<v1Sim>(
            s =>
            {
                s.HasKey(s => s.PhoneNumber);
                s.Property(s => s.LastUpdate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

        base.OnModelCreating(modelBuilder);
    }
}
