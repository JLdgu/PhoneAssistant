using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Hosting;

using Serilog;

using System.CommandLine;

namespace Import;

public sealed class Program
{
    private const string LiveRun = "live";
    private const string LiveArgument = "liveDb";
    private const string TestRun = "test";
    private const string TestArgument = "testDb";
    private const string MsOption = "--myScomis";
    private const string MsAlias = "-m";

    private static async Task Main(string[] args)
    {
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

        var database = new CliOption<FileInfo>("--database") { Description = "Path to the PhoneAssistant database", Required = true }.AcceptExistingOnly();
        database.Aliases.Add("-db");
        var myScomis = new CliOption<FileInfo>("--myScomis") { Description = "Path to the myScomis Excel spreadsheet", Required = true }.AcceptExistingOnly();
        myScomis.Aliases.Add("-ms");
        CliRootCommand rootCommand = new("Utility application to update PhoneAssistant database")
        { 
            database,
            myScomis
        };
        rootCommand.SetAction((parseResult, cancellationToken) =>
        {
            return ExecuteAsync(
                database: parseResult.CommandResult.GetValue<FileInfo>(database),
                msExcel: parseResult.CommandResult.GetValue<FileInfo>(myScomis));
        });

        try
        {
            await rootCommand.Parse(args).InvokeAsync(CancellationToken.None);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Something went wrong");
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }

    private static async Task ExecuteAsync(FileInfo? database, FileInfo? msExcel)
    {
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console()
#if DEBUG
            .MinimumLevel.Information()
            .WriteTo.File(@"c:\dev\import.txt")
#else
            .MinimumLevel.Warning()
            .WriteTo.File("import.txt", rollingInterval: RollingInterval.Day)
#endif
            .CreateLogger();

        Log.Logger.Information("Import Application Starting");

        Log.Information("Applying import to {0}", database);
        Log.Information("Importing {0}", msExcel);

        string connectionString = $"DataSource={database};";
        DbContextOptionsBuilder<ImportDbContext> optionsBuilder = new();
        optionsBuilder.UseSqlite(connectionString);
        ImportDbContext dbContext = new(optionsBuilder.Options);

        ImportMS? importMS = new(dbContext);

        Log.Information("Record count = {0}", importMS.Count());
        await Task.CompletedTask;
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