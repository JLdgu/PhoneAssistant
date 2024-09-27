using Microsoft.EntityFrameworkCore;
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
            .WriteTo.File(@"c:\dev\reconcile.txt")
#else
            .MinimumLevel.Warning()
            .WriteTo.File("reconcile.txt", rollingInterval: RollingInterval.Day)
#endif
            .CreateLogger();

        var database = new CliOption<FileInfo>("--database") { Description = "Path to the PhoneAssistant database", Required = true }.AcceptExistingOnly();
        database.Aliases.Add("-db");
        var myScomis = new CliOption<FileInfo>("--myScomis") { Description = "Path to the myScomis Excel spreadsheet", Required = true }.AcceptExistingOnly();
        myScomis.Aliases.Add("-ms");
        CliRootCommand rootCommand = new("Utility application to reconcile phone disposals")
        {
            database,
            myScomis
        };
        rootCommand.SetAction((parseResult) =>
        {
            Execute(
                database: parseResult.CommandResult.GetValue<FileInfo>(database),
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

    private static void Execute(FileInfo? database, FileInfo? msExcel)
    {
        Log.Logger.Information("Reconcile Starting");

        Log.Information("Applying import to {0}", database);
        Log.Information("Importing {0}", msExcel);

        string connectionString = $"DataSource={database};";
        DbContextOptionsBuilder<ReconcileDbContext> optionsBuilder = new();
        optionsBuilder.UseSqlite(connectionString);
        ReconcileDbContext dbContext = new(optionsBuilder.Options);

        ImportMS importMS = new(dbContext, msExcel!.FullName);
        var result = importMS.Execute();
    }

    private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is Exception ex)
        {
            Log.Fatal(ex, "Unhandled exception:");
            return;
        }

        Log.Fatal("Unhandled non-exception object:", e.ExceptionObject);
    }
}