using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace PhoneAssistant.Model;

public partial class PhoneAssistantDbContext : DbContext
{
    private static readonly ILoggerFactory _loggerFactory = LoggerFactory.Create(builder => { });

    public PhoneAssistantDbContext() { }

    public PhoneAssistantDbContext(DbContextOptions options) : base(options) { }

    public DbSet<BaseReport> BaseReport => Set<BaseReport>();

    public DbSet<ImportHistory> Imports => Set<ImportHistory>();

    public DbSet<Location> Locations => Set<Location>();

    public DbSet<Phone> Phones => Set<Phone>();

    public DbSet<SchemaVersion> SchemaVersion => Set<SchemaVersion>();

    public DbSet<Sim> Sims => Set<Sim>();

    public DbSet<SimHistory> SimsHistory => Set<SimHistory>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
            optionsBuilder.UseSqlite(@"DataSource=c:\dev\PhoneAssistant.db;");

        optionsBuilder.UseTriggers(t => t.AddTrigger<PhoneTrigger>());

#if DEBUG
        optionsBuilder.UseLoggerFactory(_loggerFactory);  // Comment out to log EF calls
        //optionsBuilder.EnableDetailedErrors();
        //optionsBuilder.EnableSensitiveDataLogging();        
#endif

        //optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BaseReport>()            
            .HasKey(e => e.PhoneNumber);

        modelBuilder.Entity<ImportHistory>()
            .ToTable("ImportHistory")
            .HasKey(e => new { e.Name, e.Run });
        modelBuilder.Entity<ImportHistory>()
            .Property(e => e.Name).HasConversion(n => n.ToString(), n => Enum.Parse<ImportType>(n));
        modelBuilder.Entity<ImportHistory>()
            .Property(e => e.ImportDate).HasDefaultValueSql("CURRENT_TIMESTAMP");   

        modelBuilder.Entity<Location>(l =>
        {
            l.HasKey(e => e.Name);

            l.Property(e => e.Collection).HasColumnName("PrintDate");
        });

        modelBuilder.Entity<Phone>(entity =>
        {
            entity.HasKey(e => e.Imei);

            entity.HasIndex(e => e.AssetTag, "IX_Phones_AssetTag").IsUnique();

            entity.Property(e => e.Imei).HasColumnName("IMEI");
            entity.Property(e => e.Condition).HasColumnName("NorR");
            entity.Property(e => e.Esim).HasColumnName("eSIM");
            entity.Property(e => e.LastUpdate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.OEM).HasConversion(o => o.ToString(), o => (Manufacturer)Enum.Parse(typeof(Manufacturer), o));
            entity.Property(e => e.Ticket)
                .HasColumnType("INTEGER")
                .HasColumnName("SRNumber");

            entity.ToTable(p => p.HasCheckConstraint("CK_NorR", "\"NorR\" = 'N' OR \"NorR\" = 'R'"));
            entity.ToTable(p => p.HasCheckConstraint("CK_OEM", "\"OEM\" = 'Apple' OR \"OEM\" = 'Nokia' OR \"OEM\" = 'Samsung' OR \"OEM\" = 'Other'"));
        });

        modelBuilder.Entity<SchemaVersion>(entity =>
        {
            entity.ToTable("SchemaVersions");
            entity.HasIndex(e => e.ScriptName, "IX_SchemaVersion_ScriptName").IsUnique();
        });

        modelBuilder.Entity<Sim>()
            .HasKey(s => new { s.PhoneNumber, s.BillingPeriod});               

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
