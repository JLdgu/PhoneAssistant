using Microsoft.EntityFrameworkCore;

using PhoneAssistant.WPF.Application.Entities;

namespace PhoneAssistant.WPF.Application;

public sealed class PhoneAssistantDbContext : DbContext
{
    //public DbSet<Link> Links => Set<Link>();

    public DbSet<Phone> Phones => Set<Phone>();

    public DbSet<v1Sim> Sims => Set<v1Sim>();

    //public DbSet<Disposal> Disposals => Set<Disposal>();

    //public DbSet<ServiceRequest> ServiceRequests => Set<ServiceRequest>();

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
        //modelBuilder.Entity<Phone>()
        //    .HasIndex(p => p.Imei).IsUnique();
        //modelBuilder.Entity<Phone>()
        //    .HasIndex(p => p.AssetTag).IsUnique();
        modelBuilder.Entity<Phone>(
            p => 
            {
                p.HasKey(x => x.Imei);
                p.Property(x => x.SR).HasColumnName("SRNumber");
                p.Property(x => x.LastUpdate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

        //modelBuilder.Entity<Sim>()
        //    .HasIndex(sc => sc.PhoneNumber).IsUnique();
        //modelBuilder.Entity<Sim>()
        //    .HasIndex(sc => sc.SimNumber).IsUnique();
        modelBuilder.Entity<v1Sim>(
            s =>
            {
                s.HasKey(s => s.PhoneNumber);
                s.Property(s => s.LastUpdate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

        //modelBuilder.Entity<ServiceRequest>()
        //    .HasIndex(sr => sr.ServiceRequestNumber).IsUnique();

        base.OnModelCreating(modelBuilder);
    }
}
