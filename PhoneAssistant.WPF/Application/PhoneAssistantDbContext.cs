using Microsoft.EntityFrameworkCore;

using PhoneAssistant.WPF.Application.Entities;

namespace PhoneAssistant.WPF.Features.Application;

public sealed class PhoneAssistantDbContext : DbContext
{
    public DbSet<Link> Links {get; set; }

    public DbSet<Phone> Phones { get; set; }

    public DbSet<Sim> Sims { get; set; }

    public DbSet<ServiceRequest> ServiceRequests { get; set; }
    
    public DbSet<Setting> Setting { get; set; }
    
    public DbSet<State> States { get; set; }


    public PhoneAssistantDbContext(DbContextOptions options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite(@"data source=phoneassistant.db;");
        }
        //optionsBuilder.UseLazyLoadingProxies(); //requires entityframeworkcore.proxies package
        optionsBuilder.EnableSensitiveDataLogging();
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

        modelBuilder.Entity<Setting>().HasData(new Setting ( 1, "0.0.0.1" ));

        base.OnModelCreating(modelBuilder);
    }
}
