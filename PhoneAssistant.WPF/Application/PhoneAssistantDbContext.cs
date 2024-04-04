using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using PhoneAssistant.WPF.Application.Entities;

namespace PhoneAssistant.WPF.Application;

public sealed class PhoneAssistantDbContext : DbContext
{
    private static readonly ILoggerFactory _loggerFactory = LoggerFactory.Create(builder => { });

    public DbSet<EEBaseReport> BaseReport => Set<EEBaseReport>();

    public DbSet<Disposal> Disposals => Set<Disposal>();

    public DbSet<ImportHistory> Imports => Set<ImportHistory>();

    public DbSet<Location> Locations => Set<Location>();

    public DbSet<Phone> Phones => Set<Phone>();

    public DbSet<Sim> Sims => Set<Sim>();

    public DbSet<UpdateHistoryPhone> UpdateHistoryPhones => Set<UpdateHistoryPhone>();

    public PhoneAssistantDbContext(DbContextOptions options) : base(options) { }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
            throw new ArgumentException("DbContextOptionsBuilder has not been configured");

#if DEBUG
        optionsBuilder.UseLoggerFactory(_loggerFactory);
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

        modelBuilder.Entity<ImportHistory>(i =>
        {
            i.ToTable("ImportHistory");
            i.Property(i => i.Name).HasConversion(n => n.ToString(), n => (ImportType)Enum.Parse(typeof(ImportType), n));
            i.Property(i => i.ImportDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<Location>(l =>
        {
            l.HasKey(l => l.Name);
        });
        
        modelBuilder.Entity<Phone>(p => 
            {
                p.HasKey(p => p.Imei);
                p.Property(p => p.Condition).HasColumnName("NorR");
                p.ToTable(p => p.HasCheckConstraint("CK_NorR", "\"NorR\" = 'N' OR \"NorR\" = 'R'"));
                p.Property(p => p.SR).HasColumnName("SRNumber");
                p.Property(p => p.OEM).HasConversion(o => o.ToString(), o => (OEMs)Enum.Parse(typeof(OEMs), o));
                p.ToTable(p => p.HasCheckConstraint("CK_OEM", "\"OEM\" = 'Apple' OR \"OEM\" = 'Nokia' OR \"OEM\" = 'Samsung' OR \"OEM\" = 'Other'"));
                p.Property(p => p.LastUpdate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                p.HasIndex(p => p.AssetTag).IsUnique();
            });

        modelBuilder.Entity<Sim>(s =>
            {
                s.HasKey(s => s.PhoneNumber);
                s.Property(s => s.LastUpdate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                s.HasIndex(s => s.SimNumber).IsUnique();
            });

        modelBuilder.Entity<UpdateHistoryPhone>(p =>
            {
                p.Property(p => p.UpdateType).HasConversion(u => u.ToString(), u => (UpdateTypes)Enum.Parse(typeof(UpdateTypes), u));
                p.Property(p => p.Condition).HasColumnName("NorR");
                p.Property(p => p.SR).HasColumnName("SRNumber");
                p.Property(p => p.OEM).HasConversion(o => o.ToString(), o => (OEMs)Enum.Parse(typeof(OEMs), o));
    });

        base.OnModelCreating(modelBuilder);
    }
}
