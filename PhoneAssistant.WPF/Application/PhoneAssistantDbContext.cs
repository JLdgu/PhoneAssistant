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

        PhoneEntity[] testdata = new PhoneEntity[]
        {
            new PhoneEntity() {Id = 1, IMEI = "353427866717729", FormerUser = "Rehana Kausar", Wiped = true, Status = "In Stock", OEM = "Samsung", AssetTag = null, Note = null  },
            new PhoneEntity() {Id = 2, IMEI = "355808981132845", FormerUser = null, Wiped = true, Status = "Production", OEM = "Samsung", AssetTag = "MP00017", Note = "Replacement"},
            new PhoneEntity() {Id = 3, IMEI = "355808983976082", FormerUser = "Charlie Baker", Wiped = true, Status = "In Stock", OEM = "Samsung", AssetTag = null, Note = null},
            new PhoneEntity() {Id = 4, IMEI = "355808981101899", FormerUser = "Olatunji Okedeyi", Wiped = false, Status = "In Stock", OEM = "Samsung", AssetTag = null, Note = null},
            new PhoneEntity() {Id = 5, IMEI = "353427861419768", FormerUser = "James Tisshaw", Wiped = false, Status = "In Stock", OEM = "Samsung", AssetTag = null, Note = null},
            new PhoneEntity() {Id = 6, IMEI = "351554742085336", FormerUser = null, Wiped = true, Status = "Production", OEM = "Samsung", AssetTag = "MP00016", Note = "Replacement"}
            };
        modelBuilder.Entity<PhoneEntity>().HasData(testdata);

        //modelBuilder.Entity<SmartCard>()
        //    .HasIndex(sc => sc.SimNumber).IsUnique();
        //modelBuilder.Entity<SmartCard>()
        //    .HasIndex(sc => sc.PhoneNumber).IsUnique();

        modelBuilder.Entity<SettingEntity>().HasData(new SettingEntity { Id = 1, MinimumVersion = "0.0.0.1" });

        base.OnModelCreating(modelBuilder);
    }
}
