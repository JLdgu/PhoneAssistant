using Microsoft.EntityFrameworkCore;

using PhoneAssistant.WPF.Application.Entities;

namespace PhoneAssistant.WPF.Application;

public sealed class PhoneAssistantDbContext : DbContext
{
    //public DbSet<Link> Links => Set<Link>();

    public DbSet<Phone> Phones => Set<Phone>();

    //public DbSet<Sim> Sims => Set<Sim>();

    public DbSet<ServiceRequest> ServiceRequests => Set<ServiceRequest>();

    public DbSet<Setting> Setting => Set<Setting>();

    public DbSet<State> States => Set<State>();

    public PhoneAssistantDbContext(DbContextOptions options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
            throw new ArgumentException("DbContextOptionsBuilder has not been configured");
#if DEBUG
        optionsBuilder.EnableSensitiveDataLogging();
#endif
        base.OnConfiguring(optionsBuilder);

        //optionsBuilder.UseLazyLoadingProxies(); //requires entityframeworkcore.proxies package
        //optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information).EnableSensitiveDataLogging();
        //optionsBuilder.LogTo(Console.WriteLine , new[] { DbLoggerCategory.Query.Name, DbLoggerCategory.Update.Name}).EnableSensitiveDataLogging();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Phone>()
            .HasIndex(p => p.Imei).IsUnique();
        modelBuilder.Entity<Phone>()
            .HasIndex(p => p.AssetTag).IsUnique();

        modelBuilder.Entity<State>()
            .HasKey(s => s.Status);

        modelBuilder.Entity<Sim>()
            .HasIndex(sc => sc.PhoneNumber).IsUnique();
        modelBuilder.Entity<Sim>()
            .HasIndex(sc => sc.SimNumber).IsUnique();

        modelBuilder.Entity<ServiceRequest>()
            .HasIndex(sr => sr.ServiceRequestNumber).IsUnique();

        modelBuilder.Entity<Setting>().HasData(new Setting(1, "0.0.0.1"));

        base.OnModelCreating(modelBuilder);
    }
}
