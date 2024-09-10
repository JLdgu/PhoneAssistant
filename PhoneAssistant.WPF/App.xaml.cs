using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

using MaterialDesignThemes.Wpf;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using PhoneAssistant.Model;
using PhoneAssistant.WPF.Application;

using Velopack;

namespace PhoneAssistant.WPF;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : System.Windows.Application
{
    [STAThread]
    private static void Main(string[] args)
    {
        Trace.Listeners.Add(new TextWriterTraceListener("PhoneAssistant.log", "logListener"));
        Trace.AutoFlush = true;

        if (!UserSettings.DatabaseFullPathRetrieved())
        {
            Trace.Close();
            return;
        }
        
        VelopackApp.Build().Run();
        
        MainAsync(args).GetAwaiter().GetResult();
    }

    private static async Task MainAsync(string[] args)
    {
        using IHost host = Host.CreateDefaultBuilder()
            .ConfigureApplicationServices()
            .Build();        
        await host.StartAsync().ConfigureAwait(true);

        App app = new();
        app.InitializeComponent();
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
        theme.SetBaseTheme(BaseTheme.Inherit);
        theme.SetPrimaryColor(Colors.Orange);
        theme.SetSecondaryColor(Colors.Yellow);
        helper.SetTheme(theme);
#endif

        base.OnStartup(e);
    }
    private void OnExit(object sender, ExitEventArgs e)
    {
        Trace.Close();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        base.OnExit(e);
    }

    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        Trace.TraceError(DateTime.Now.ToString());

        Exception? ex = e.Exception;

        while (ex is not null)
        {

            Trace.TraceError(ex.GetType().FullName);
            Trace.TraceError("HResult=" + ex.HResult);
            Trace.TraceError("Message=" + ex.Message);
            Trace.TraceError("StackTrace:");
            Trace.TraceError(ex.StackTrace);

            ex = ex.InnerException;
        }
        Trace.TraceError("-----------------------------------------------------------------------------");
        Trace.TraceError("");

        Trace.Close();

        e.Handled = true;

        MessageBox.Show($"An unexpected exception has occured {Environment.NewLine}See PhoneAssistant.log for more details.",
            "Phone Assistant", MessageBoxButton.OK, MessageBoxImage.Stop);

        App.Current.Shutdown();
    }
}
