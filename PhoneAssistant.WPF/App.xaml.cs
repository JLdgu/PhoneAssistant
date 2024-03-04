using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

using MaterialDesignThemes.Wpf;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using PhoneAssistant.WPF.Application;

namespace PhoneAssistant.WPF;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : System.Windows.Application
{
    private readonly IHost _host;

    public App()
    {
        _host = Host.CreateDefaultBuilder()
            .ConfigureApplicationServices()
            .Build();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        _host.Start();

        if (ApplicationUpdate.FirstRun())
        {
            Trace.Close();
            Current.Shutdown();
        }

        ConfigureDatabase();

#if DEBUG
        var helper = new PaletteHelper();
        var theme = helper.GetTheme();
        theme.SetPrimaryColor(Colors.Orange);
        theme.SetSecondaryColor(Colors.DarkOrange);
        helper.SetTheme(theme);
#endif


        MainWindow = _host.Services.GetRequiredService<MainWindow>();
        MainWindow.Show();

        base.OnStartup(e);
    }
    private void OnExit(object sender, ExitEventArgs e)
    {
        Trace.Close();
    }

    private void ConfigureDatabase()
    {
        PhoneAssistantDbContext dbContext = _host.Services.GetRequiredService<PhoneAssistantDbContext>();

        dbContext.Database.EnsureCreated();
        //dbContext.Database.Migrate();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _host.Dispose();

        base.OnExit(e);
    }

    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        Trace.Listeners.Add(new TextWriterTraceListener("PhoneAssistant.log", "logListener"));//
        Trace.AutoFlush = true;

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
