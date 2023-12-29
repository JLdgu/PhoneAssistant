using Microsoft.EntityFrameworkCore;

using PhoneAssistant.WPF.Application.Entities;

using static System.Net.Mime.MediaTypeNames;

namespace PhoneAssistant.WPF.Application;

public sealed class PhoneAssistantDbContext : DbContext
{
    public DbSet<Link> Links => Set<Link>();

    public DbSet<Phone> Phones => Set<Phone>();

    public DbSet<Sim> Sims => Set<Sim>();

    //public DbSet<Disposal> Disposals => Set<Disposal>();

    public DbSet<ServiceRequest> ServiceRequests => Set<ServiceRequest>();

    public PhoneAssistantDbContext(DbContextOptions options) : base(options) { }

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
                //s.Property(s => s.LastUpdate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

        modelBuilder.Entity<ServiceRequest>(
            sr =>
            {
                sr.HasKey(sr => sr.ServiceRequestNumber);
                sr.Property(sr => sr.ServiceRequestNumber).HasColumnName("SRNumber");
                //sr.Property(sr => sr.LastUpdate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

        modelBuilder.Entity<Link>(
            l =>
            {
                l.Property("PhoneImei").HasColumnName("Imei");
                l.Property("SimPhoneNumber").HasColumnName("PhoneNumber");
                l.Property("ServiceRequestNumber").HasColumnName("SRNumber");
            });

        base.OnModelCreating(modelBuilder);
    }
}
