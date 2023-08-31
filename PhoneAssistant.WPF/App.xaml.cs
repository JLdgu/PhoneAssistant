using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

using PhoneAssistant.WPF.Application;
using PhoneAssistant.WPF.Application.Entities;
using PhoneAssistant.WPF.Features.MainWindow;

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
            Current.Shutdown();

        ConfigureDatabase();

        MainWindow = _host.Services.GetRequiredService<MainWindow>();
        MainWindow.Show();

        base.OnStartup(e);
    }

    private void ConfigureDatabase()
    {
        v1PhoneAssistantDbContext dbContext = _host.Services.GetRequiredService<v1PhoneAssistantDbContext>();

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
        using StreamWriter writer = new("PhoneAssistant.ErrorLog.txt", true);
        writer.WriteLine(DateTime.Now.ToString());

        Exception? ex = e.Exception;

        while (ex is not null)
        {
            writer.WriteLine(ex.GetType().FullName);
            writer.WriteLine("HResult=" + ex.HResult);
            writer.WriteLine("Message=" + ex.Message);
            writer.WriteLine("StackTrace:");
            writer.WriteLine(ex.StackTrace);

            ex = ex.InnerException;
        }
        writer.WriteLine("-----------------------------------------------------------------------------");
        writer.WriteLine();

        writer.Close();

        e.Handled = true;

        MessageBox.Show($"An unexpected exception has occured {Environment.NewLine}See PhoneAssistant.ErrorLog.txt for more details.",
            "Phone Assistant", MessageBoxButton.OK, MessageBoxImage.Stop);

        App.Current.Shutdown();
    }
}
