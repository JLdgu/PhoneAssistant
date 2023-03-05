using Microsoft.EntityFrameworkCore;

using PhoneAssistant.WPF.Application.Entities;

namespace PhoneAssistant.WPF.Features.Application;

public sealed class PhoneAssistantDbContext : DbContext
{
    //public DbSet<StateDTO> State { get; set; }
    //public DbSet<LinkDTO> Links { get; set; }
    public DbSet<PhoneEntity> MobilePhones { get; set; }
    //public DbSet<SimCardDTO> SimCards { get; set; }

    public DbSet<SettingEntity> Setting { get; set; }

    public PhoneAssistantDbContext(DbContextOptions options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite(@"data source=phoneassistant.db;");
        }
        //optionsBuilder.UseLazyLoadingProxies(); //requires entityframeworkcore.proxies package
        //optionsBuilder.EnableSensitiveDataLogging();
        //optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information).EnableSensitiveDataLogging();
        //optionsBuilder.LogTo(Console.WriteLine , new[] { DbLoggerCategory.Query.Name, DbLoggerCategory.Update.Name}).EnableSensitiveDataLogging();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //modelBuilder.Entity<StateDTO>()
        //    .HasKey(s => s.Status);                

        //StateDTO[] states = new StateDTO[]{
        //        new StateDTO { Status = "In Stock" },
        //        new StateDTO { Status = "In Repair" },
        //        new StateDTO { Status = "Production" }
        //};
        //modelBuilder.Entity<StateDTO>().HasData(states);

        modelBuilder.Entity<PhoneEntity>()
            .HasIndex(p => p.IMEI).IsUnique();        

        //modelBuilder.Entity<SmartCard>()
        //    .HasIndex(sc => sc.SimNumber).IsUnique();
        //modelBuilder.Entity<SmartCard>()
        //    .HasIndex(sc => sc.PhoneNumber).IsUnique();

        modelBuilder.Entity<SettingEntity>().HasData(new SettingEntity { Id = 1, MinimumVersion = "0.0.0.1" });

        base.OnModelCreating(modelBuilder);
    }
}
