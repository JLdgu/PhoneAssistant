using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
    [STAThread]
    private static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console(theme: AnsiConsoleTheme.Sixteen)
            .MinimumLevel.Verbose()
            .WriteTo.File("PhoneAssistant.log")
            .CreateLogger();
        Log.Information("Starting Phone Assistant");

        VelopackApp.Build().Run();
        
        try
        {
            MainAsync(args).GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            LogAndHandleException(ex);
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    private static async Task MainAsync(string[] args)
    {
        using IHost host = Host.CreateDefaultBuilder()
            .ConfigureApplicationServices()
            .Build();

        await host.StartAsync().ConfigureAwait(true);

        App app = new();
        app.InitializeComponent();

        var settings = host.Services.GetRequiredService<ISettingsMainViewModel>();
        await settings.LoadAsync();

        app.MainWindow = host.Services.GetRequiredService<MainWindow>();
        app.MainWindow.Visibility = Visibility.Visible;
        app.Run();

        DatabaseServices.BackupDatabase(host);

        await host.StopAsync().ConfigureAwait(true);
    }

    protected override void OnStartup(StartupEventArgs e)
    {
#if DEBUG
        var helper = new PaletteHelper();
        var theme = helper.GetTheme();
        theme.SetPrimaryColor(Colors.Orange);
        theme.SetSecondaryColor(Colors.Yellow);
        helper.SetTheme(theme);
#endif

        base.OnStartup(e);
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
