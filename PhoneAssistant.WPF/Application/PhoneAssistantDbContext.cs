using Microsoft.EntityFrameworkCore;

using PhoneAssistant.WPF.Application.Entities;

namespace PhoneAssistant.WPF.Features.Application;

public sealed class PhoneAssistantDbContext : DbContext
{
    public DbSet<PhoneEntity> Phones { get; set; }
    //public DbSet<SimEntity> Sims { get; set; }
    public DbSet<ServiceRequest> ServiceRequests { get; set; }
    public DbSet<SettingEntity> Setting { get; set; }
    public DbSet<StateEntity> States { get; set; }


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
        modelBuilder.Entity<PhoneEntity>()
            .HasIndex(p => p.IMEI).IsUnique();        

        modelBuilder.Entity<StateEntity>()
            .HasKey(s => s.Status);                

        //modelBuilder.Entity<SmartCard>()
        //    .HasIndex(sc => sc.SimNumber).IsUnique();
        //modelBuilder.Entity<SmartCard>()
        //    .HasIndex(sc => sc.PhoneNumber).IsUnique();

        modelBuilder.Entity<SettingEntity>().HasData(new SettingEntity ( 1, "0.0.0.1" ));

        base.OnModelCreating(modelBuilder);
    }
}
