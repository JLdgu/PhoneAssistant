using Microsoft.EntityFrameworkCore;

using PhoneAssistant.WPF.Application.Entities;

namespace PhoneAssistant.WPF.Application;

public sealed class v1PhoneAssistantDbContext : DbContext
{
    public DbSet<v1Phone> Phones => Set<v1Phone>();

    //public DbSet<Sim> Sims => Set<Sim>();

    //public DbSet<Disposal> Disposals => Set<State>();

    public v1PhoneAssistantDbContext(DbContextOptions options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
            throw new ArgumentException("DbContextOptionsBuilder has not been configured");
#if DEBUG
        optionsBuilder.EnableSensitiveDataLogging();
#endif
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<v1Phone>(
            p => 
            {
                p.HasKey(x => x.Imei);
                //p.Property(x => x.Imei).HasColumnName("IMEI");
                //p.Property(x => x.PhoneNumber).HasColumnName("PhoneNumber");
                //p.Property(x => x.SimNumber).HasColumnName("SIMNumber");
                //p.Property(x => x.FormerUser).HasColumnName("FormerUser");
                p.Property(x => x.SR).HasColumnName("SRNumber");
                //p.Property(x => x.AssetTag).HasColumnName("AssetTag");
                //p.Property(x => x.NewUser).HasColumnName("NewUser");
                p.Property(x => x.LastUpdate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            }
        );

        base.OnModelCreating(modelBuilder);
    }
}
