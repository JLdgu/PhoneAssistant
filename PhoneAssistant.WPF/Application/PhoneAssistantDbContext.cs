using Microsoft.EntityFrameworkCore;

using PhoneAssistant.WPF.Application.Entities;

namespace PhoneAssistant.WPF.Application;

public sealed class PhoneAssistantDbContext : DbContext
{
    public DbSet<EEBaseReport> BaseReport => Set<EEBaseReport>();

    public DbSet<Disposal> Disposals => Set<Disposal>();

    public DbSet<ImportHistory> Imports => Set<ImportHistory>();

    public DbSet<Location> Locations => Set<Location>();

    public DbSet<Phone> Phones => Set<Phone>();

    public DbSet<Sim> Sims => Set<Sim>();

    public DbSet<ServiceRequest> ServiceRequests => Set<ServiceRequest>();

    public PhoneAssistantDbContext(DbContextOptions options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
            throw new ArgumentException("DbContextOptionsBuilder has not been configured");

#if DEBUG
        //optionsBuilder.EnableSensitiveDataLogging();        
#endif
      
        //optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Disposal>(d =>
        {
            d.ToTable("ReconcileDisposals");
            d.HasKey(d => d.Imei);
        });

        modelBuilder.Entity<EEBaseReport>(b => 
        {
            b.ToTable("BaseReport");
            b.HasKey(b => b.PhoneNumber);
        });

        modelBuilder.Entity<ImportHistory>(d =>
        {
            d.ToTable("ImportHistory");
            d.Property(i => i.Name).HasConversion(n => n.ToString(), n => (ImportType)Enum.Parse(typeof(ImportType), n));
        });

        modelBuilder.Entity<Location>(l =>
        {
            l.HasKey(l => l.Name);
        });
        
        modelBuilder.Entity<Phone>(
            p => 
            {
                p.HasKey(p => p.Imei);
                p.Property(p => p.SR).HasColumnName("SRNumber");                
                p.HasIndex(p => p.AssetTag).IsUnique();
                p.Property(p => p.LastUpdate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });
        modelBuilder.Entity<Phone>()
            .ToTable(p => p.HasCheckConstraint("CK_NorR", "\"NorR\" = 'N' OR \"NorR\" = 'R'"))
            .ToTable(p => p.HasCheckConstraint("CK_OEM", "\"OEM\" = 'Apple' OR \"OEM\" = 'Nokia' OR \"OEM\" = 'Samsung'"))
;

        modelBuilder.Entity<Sim>(
            s =>
            {
                s.HasKey(s => s.PhoneNumber);
                s.HasIndex(s => s.SimNumber).IsUnique();
            });

        modelBuilder.Entity<ServiceRequest>(
            sr =>
            {
                sr.HasKey(sr => sr.ServiceRequestNumber);
                sr.Property(sr => sr.ServiceRequestNumber).HasColumnName("SRNumber");
            });

        base.OnModelCreating(modelBuilder);
    }
}
