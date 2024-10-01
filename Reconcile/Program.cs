using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

using System.CommandLine;

namespace Reconcile;

public sealed class Program
{
    private static void Main(string[] args)
    {
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console(theme: AnsiConsoleTheme.Sixteen)
#if DEBUG
            .MinimumLevel.Debug()
            .WriteTo.File(@"c:\dev\reconcile.log")
#else
            .MinimumLevel.Warning()
            .WriteTo.File("reconcile.log", rollingInterval: RollingInterval.Day)
#endif
            .CreateLogger();

        var myScomis = new CliOption<FileInfo>("--myScomis") { Description = "Path to the myScomis Excel spreadsheet", Required = true }.AcceptExistingOnly();
        myScomis.Aliases.Add("-ms");
        CliRootCommand rootCommand = new("Utility application to reconcile phone disposals")
        {
            myScomis
        };
        rootCommand.SetAction((parseResult) =>
        {
            Execute(
                msExcel: parseResult.CommandResult.GetValue<FileInfo>(myScomis)
                );
        });

        try
        {
            rootCommand.Parse(args).Invoke();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Something went wrong");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    private static void Execute(FileInfo? msExcel) 
    {
        Log.Information("Reconcile Starting");
        Log.Information("Importing {0}", msExcel);

        ImportMS importMS = new( msExcel!.FullName); //dbContext,
        FluentResults.Result<List<Device>> result = importMS.Execute();
        if (result.IsFailed)
        {
            Log.Error(result.Errors.First().Message);
            return; 
        }
    }

    private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is Exception ex)
        {
            Log.Fatal(exception: ex, "Unhandled exception:");
            return;
        }

        Log.Fatal("Unhandled non-exception object:", e.ExceptionObject);
    }
}