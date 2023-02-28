using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PhoneAssistant.WPF.Features.Application;
using PhoneAssistant.WPF.Features.MainWindow;
using PhoneAssistant.WPF.Models;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;

namespace PhoneAssistant.WPF;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
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

        MainWindow = _host.Services.GetRequiredService<MainWindow>();
        MainWindow.Show();

        base.OnStartup(e);
    }

    private void ConfigureServices(HostBuilderContext context, IServiceCollection services)
    {
        string? connectionString = context.Configuration.GetConnectionString("Default");

        // App Host
        //services.AddHostedService<ApplicationHostService>();

        // Activation Handlers

        // Core Services
        //services.AddSingleton<IFileService, FileService>();
        services.AddDbContext<PhoneAssistantDbContext>(
                        options => options.UseSqlite(connectionString));  //,ServiceLifetime.Singleton);
        // Services
        //services.AddSingleton<IApplicationInfoService, ApplicationInfoService>();
        //services.AddSingleton<ISystemService, SystemService>();
        //services.AddSingleton<IPersistAndRestoreService, PersistAndRestoreService>();
        //services.AddSingleton<IThemeSelectorService, ThemeSelectorService>();
        //services.AddSingleton<ISampleDataService, SampleDataService>();
        //services.AddSingleton<IPageService, PageService>();
        //services.AddSingleton<INavigationService, NavigationService>();

        // Windows
        services.AddSingleton<MainWindow>();
        services.AddTransient<PhoneRepository>();
        services.AddTransient<SimRepository>();
        services.AddTransient<StateRepository>();

        // Views and ViewModels
        //services.AddTransient<IShellWindow, ShellWindow>();
        //services.AddTransient<ShellViewModel>();

        services.AddTransient<MainWindowViewModel>();
        services.AddScoped<MainWindow>(s => new MainWindow(s.GetRequiredService<MainWindowViewModel>()));
        //services.AddTransient<MainPage>();

        //services.AddTransient<BlankViewModel>();
        //services.AddTransient<BlankPage>();
       
        //services.AddTransient<SettingsViewModel>();
        //services.AddTransient<SettingsPage>();

        //// Configuration
        //services.Configure<AppConfig>(context.Configuration.GetSection(nameof(AppConfig)));
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
