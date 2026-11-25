using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Data.Sqlite;
using PhoneAssistant.Model;
using PhoneAssistant.WPF.Application;
using PhoneAssistant.WPF.Features.Settings;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using Velopack;

namespace PhoneAssistant.WPF;

public partial class App : System.Windows.Application
{
    private IHost? _host;
    private bool _startupCompleted;

    [STAThread]
    private static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console(theme: AnsiConsoleTheme.Sixteen)
#if DEBUG
            .MinimumLevel.Verbose()
#else
            .MinimumLevel.Information()
#endif
            .WriteTo.File("PhoneAssistant.log")
            .CreateLogger();
        Log.Information("Starting Phone Assistant");

        VelopackApp.Build().Run();
        
        App? app = null;
        try
        {
            app = new App();
            app.InitializeComponent();
            app.Run();

            if (app._startupCompleted && app._host is not null)
                DatabaseServices.BackupDatabase(app._host);

            app._host?.StopAsync().GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            LogAndHandleException(ex);
        }
        finally
        {
            app?._host?.Dispose();
            Log.CloseAndFlush();
        }
    }

    private async Task InitializeApplicationAsync()
    {
        _host = Host.CreateDefaultBuilder()
            .ConfigureApplicationServices()
            .Build();

        await _host.StartAsync();

        MainWindow mainWindow = _host.Services.GetRequiredService<MainWindow>();
        MainWindow = mainWindow;
        SplashWindow splashWindow = new(mainWindow);
        splashWindow.Show();

        ISimCacheService simCacheService = _host.Services.GetRequiredService<ISimCacheService>();
        try
        {
            bool refreshed = await simCacheService.RefreshIfNewerAsync();
            if (refreshed)
                Log.Information("SIM cache refreshed from the shared database.");
        }
        catch (SqliteException ex)
        {
            Log.Warning(ex, "Unable to refresh the local SIM cache; using the last cached data if available.");
        }

        ISettingsMainViewModel settings = _host.Services.GetRequiredService<ISettingsMainViewModel>();
        await settings.LoadAsync();

        mainWindow.ContentRendered += (_, _) => splashWindow.Close();
        mainWindow.Show();
        _startupCompleted = true;
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
#if DEBUG
        ApplicationSettingsRepository repo = new();
        new ThemeWrapper().ModifyTheme(repo.ApplicationSettings.DarkMode, Colors.Orange, Colors.Yellow);
#endif

        base.OnStartup(e);

        try
        {
            await InitializeApplicationAsync();
        }
        catch (Exception ex)
        {
            LogAndHandleException(ex);
        }
    }

    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        e.Handled = true;
        LogAndHandleException(e.Exception);
    }

    private static void LogAndHandleException(Exception ex)
    {
        var exception = ex;
        while (exception is not null)
        {

            Log.Error(ex.GetType().FullName ?? "PhoneAssistant");
            Log.Error("HResult=" + ex.HResult);
            Log.Error("Message=" + ex.Message);
            Log.Error("StackTrace:");
            Log.Error(ex.StackTrace ?? "No StackTrace found");

            exception = exception.InnerException;
        }
        Log.Error("-----------------------------------------------------------------------------");

        MessageBox.Show($"An unexpected exception has occurred {Environment.NewLine}See PhoneAssistant.log for more details.",
            "Phone Assistant", MessageBoxButton.OK, MessageBoxImage.Stop);

        App.Current.Shutdown();
    }
}
