using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PhoneAssistant.WPF.Application;
using PhoneAssistant.WPF.Features.Application;
using PhoneAssistant.WPF.Features.MainWindow;
using PhoneAssistant.WPF.Features.Phones;
using PhoneAssistant.WPF.Features.ServiceRequest;
using PhoneAssistant.WPF.Features.Settings;
using PhoneAssistant.WPF.Features.SimCard;
using PhoneAssistant.WPF.Models;
using System.Configuration;
using System.Windows;
using System.Windows.Threading;

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

        PhoneAssistantDbContext dbContext = _host.Services.GetRequiredService<PhoneAssistantDbContext>();

        //dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();
        //dbContext.Database.Migrate();

        var _appRepository = _host.Services.GetRequiredService<AppRepository>();
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

    private void ConfigureServices(HostBuilderContext context, IServiceCollection services)
    {
        string? connectionString = context.Configuration.GetConnectionString("Default");

        services.AddSingleton<AppRepository>();
        services.AddTransient<PhonesRepository>();
        services.AddTransient<SimRepository>();
        services.AddTransient<ISettingsRepository, SettingsRepository>();
        services.AddTransient<StateRepository>();

        services.AddTransient<PhonesMainViewModel>();
        services.AddTransient<SimCardMainViewModel>();
        services.AddTransient<ServiceRequestMainViewModel>();
        services.AddTransient<SettingsMainViewModel>();
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
