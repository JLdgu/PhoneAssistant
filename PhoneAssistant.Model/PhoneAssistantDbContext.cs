﻿using Microsoft.EntityFrameworkCore;
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

    public DbSet<UpdateHistoryPhone> UpdateHistoryPhones => Set<UpdateHistoryPhone>();

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
        modelBuilder.Entity<BaseReport>(entity =>
        {
            entity.HasKey(e => e.PhoneNumber);
        });

        modelBuilder.Entity<ImportHistory>(entity =>
        {
            entity.ToTable("ImportHistory");

            //entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasConversion(n => n.ToString(), n => (ImportType)Enum.Parse(typeof(ImportType), n));
            entity.Property(e => e.ImportDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<Location>(entity =>
        {
            entity.HasKey(e => e.Name);
        });

        modelBuilder.Entity<Phone>(entity =>
        {
            entity.HasKey(e => e.Imei);

            entity.HasIndex(e => e.AssetTag, "IX_Phones_AssetTag").IsUnique();

            entity.Property(e => e.Imei).HasColumnName("IMEI");
            entity.Property(e => e.Condition).HasColumnName("NorR");
            entity.Property(e => e.LastUpdate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.OEM).HasConversion(o => o.ToString(), o => (Manufacturer)Enum.Parse(typeof(Manufacturer), o));
            entity.Property(e => e.SR)
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

            modelBuilder.Entity<UpdateHistoryPhone>(entity =>
        {
            entity.Property(e => e.Condition).HasColumnName("NorR");
            entity.Property(e => e.Imei).HasColumnName("IMEI");
            entity.Property(e => e.LastUpdate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.OEM).HasConversion(o => o.ToString(), o => (Manufacturer)Enum.Parse(typeof(Manufacturer), o));
            entity.Property(e => e.SimNumber)
                .HasColumnName("SIMNumber");
            entity.Property(e => e.SR)
                .HasColumnType("INTEGER")
                .HasColumnName("SRNumber");
            entity.Property(e => e.UpdateType).HasConversion(u => u.ToString(), u => (UpdateTypes)Enum.Parse(typeof(UpdateTypes), u));
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
