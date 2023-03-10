using System.Reflection.Emit;
using System.Windows;
using System.Windows.Threading;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using PhoneAssistant.WPF.Application;
using PhoneAssistant.WPF.Application.Entities;
using PhoneAssistant.WPF.Features.Application;
using PhoneAssistant.WPF.Features.MainWindow;
using PhoneAssistant.WPF.Features.Phones;
using PhoneAssistant.WPF.Features.ServiceRequests;
using PhoneAssistant.WPF.Features.Settings;
using PhoneAssistant.WPF.Features.Sims;

namespace PhoneAssistant.WPF;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : System.Windows.Application
{
    private IHost _host;

    public App()
    {
        // For more information about .NET generic host see  https://docs.microsoft.com/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-3.0
        _host = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration(ch => { })
            .ConfigureServices(ConfigureServices)
            .Build();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        _host.Start();

        ConfigureDatabase();

        var _appRepository = _host.Services.GetRequiredService<IAppRepository>();
        bool InvalidVersion = _appRepository.InvalidVersionAsync().Result;
        if (InvalidVersion)
        {
            MessageBox.Show("Invalid version, please update to the latest version", "Phone Assistant",
                MessageBoxButton.OK, MessageBoxImage.Stop);
            Current.Shutdown();
            return;
        }

        MainWindow = _host.Services.GetRequiredService<MainWindow>();
        MainWindow.Show();

        base.OnStartup(e);
    }

    private void ConfigureDatabase()
    {
        PhoneAssistantDbContext dbContext = _host.Services.GetRequiredService<PhoneAssistantDbContext>();

        //dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();
        //dbContext.Database.Migrate();

        if (dbContext.MobilePhones.Count() > 0) return;

        PhoneEntity[] testdata = new PhoneEntity[]
        {
        new PhoneEntity() {Id = 1, IMEI = "353427866717729", FormerUser = "Rehana Kausar", Wiped = true, Status = "In Stock", OEM = "Samsung", AssetTag = null, Note = null  },
        new PhoneEntity() {Id = 2, IMEI = "355808981132845", FormerUser = null, Wiped = true, Status = "Production", OEM = "Samsung", AssetTag = "MP00017", Note = "Replacement"},
        new PhoneEntity() {Id = 3, IMEI = "355808983976082", FormerUser = "Charlie Baker", Wiped = true, Status = "In Stock", OEM = "Samsung", AssetTag = null, Note = null},
        new PhoneEntity() {Id = 4, IMEI = "355808981101899", FormerUser = "Olatunji Okedeyi", Wiped = false, Status = "In Stock", OEM = "Samsung", AssetTag = null, Note = null},
        new PhoneEntity() {Id = 5, IMEI = "353427861419768", FormerUser = "James Tisshaw", Wiped = false, Status = "In Stock", OEM = "Samsung", AssetTag = null, Note = null},
        new PhoneEntity() {Id = 6, IMEI = "351554742085336", FormerUser = "Unknown", Wiped = true, Status = "Production", OEM = "Samsung", AssetTag = "MP00016", Note = "Replacement"}
        };

        dbContext.MobilePhones.AddRange(testdata);
        dbContext.SaveChanges();
    }

    private void ConfigureServices(HostBuilderContext context, IServiceCollection services)
    {
        string? connectionString = context.Configuration.GetConnectionString("Default");

        services.AddSingleton<IAppRepository, AppRepository>();
        services.AddTransient<IPhonesRepository, PhonesRepository>();
        services.AddTransient<SimsRepository>();
        services.AddTransient<ISettingsRepository, SettingsRepository>();
        services.AddTransient<StateRepository>();

        services.AddTransient<IPhonesMainViewModel,PhonesMainViewModel>();
        services.AddTransient<ISimsMainViewModel,SimsMainViewModel>();
        services.AddTransient<IServiceRequestsMainViewModel,ServiceRequestsMainViewModel>();
        services.AddTransient<ISettingsMainViewModel,SettingsMainViewModel>();
        services.AddTransient<MainWindowViewModel>();

        services.AddDbContext<PhoneAssistantDbContext>(
                        options => options.UseSqlite(connectionString));  //,ServiceLifetime.Singleton);

        services.AddScoped(s => new MainWindow(s.GetRequiredService<MainWindowViewModel>()));
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _host.Dispose();

        base.OnExit(e);
    }

    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        // TODO: Please log and handle the exception as appropriate to your scenario
        // For more info see https://docs.microsoft.com/dotnet/api/system.windows.application.dispatcherunhandledexception?view=netcore-3.0
    }
}
