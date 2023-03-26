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
    private readonly IHost _host;

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

        var appRepository = _host.Services.GetRequiredService<IAppRepository>();
        bool invalidVersion = appRepository.InvalidVersionAsync().Result;
        if (invalidVersion)
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

        if (dbContext.Phones.Any()) return;

        Phone[] testPhones = new Phone[]
        {
        new Phone() {Id = 1, Imei = "353427866717729", FormerUser = "Rehana Kausar", Wiped = true, Status = "In Stock", OEM = "Samsung", AssetTag = null, Note = null  },
        new Phone() {Id = 2, Imei = "355808981132845", FormerUser = null, Wiped = true, Status = "Production", OEM = "Samsung", AssetTag = "MP00017", Note = "Replacement"},
        new Phone() {Id = 3, Imei = "355808983976082", FormerUser = "Charlie Baker", Wiped = true, Status = "In Stock", OEM = "Samsung", AssetTag = null, Note = null},
        new Phone() {Id = 4, Imei = "355808981101899", FormerUser = "Olatunji Okedeyi", Wiped = false, Status = "In Stock", OEM = "Samsung", AssetTag = null, Note = null},
        new Phone() {Id = 5, Imei = "353427861419768", FormerUser = "James Tisshaw", Wiped = false, Status = "In Stock", OEM = "Samsung", AssetTag = null, Note = null},
        new Phone() {Id = 6, Imei = "351554742085336", FormerUser = "Unknown", Wiped = true, Status = "Production", OEM = "Samsung", AssetTag = "MP00016", Note = "Replacement"}
        };
        dbContext.Phones.AddRange(testPhones);

        ServiceRequest[] testSRs = new ServiceRequest[]
        {
            new ServiceRequest() {Id = 1, ServiceRequestNumber = 101, NewUser= "Test User 101"},
            new ServiceRequest() {Id = 2, ServiceRequestNumber = 202, NewUser= "Test User 202", DespatchDetails="DD202"},
            new ServiceRequest() {Id = 3, ServiceRequestNumber = 303, NewUser= "Test User 303"},
            new ServiceRequest() {Id = 4, ServiceRequestNumber = 404, NewUser= "Test User 404", DespatchDetails="DD404"},
            new ServiceRequest() {Id = 5, ServiceRequestNumber = 505, NewUser= "Test User 505"}
        };
        dbContext.ServiceRequests.AddRange(testSRs);

        State[] testStates = new State[]{
                new State ( "In Stock" ),
                new State ( "In Repair" ),
                new State ( "Production" )
        };
        dbContext.States.AddRange(testStates);

        dbContext.SaveChanges();
    }

    private void ConfigureServices(HostBuilderContext context, IServiceCollection services)
    {
        string? connectionString = context.Configuration.GetConnectionString("Default");

        services.AddSingleton<IAppRepository, AppRepository>();
        services.AddTransient<IPhonesRepository, PhonesRepository>();
        services.AddTransient<ISimsRepository,SimsRepository>();
        services.AddTransient<IServiceRequestsRepository, ServiceRequestsRepository>();
        services.AddTransient<ISettingsRepository, SettingsRepository>();
        services.AddTransient<IStateRepository,StateRepository>();

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
