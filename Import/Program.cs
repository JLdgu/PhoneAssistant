using System.CommandLine;

using Serilog;

namespace Import;

public sealed class Program
{
    private const string LiveRun = "live";
    private const string LiveArgument = "liveDb";
    private const string TestRun = "test";
    private const string TestArgument = "testDb";
    private const string MsOption = "--myScomis";
    private const string MsAlias = "-m";

    static async Task Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
#if DEBUG
            .WriteTo.File(@"c:\dev\import.txt")
#else
            .WriteTo.File("import.txt", rollingInterval: RollingInterval.Day)
#endif
            .CreateLogger();

        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

        try
        {
            CliConfiguration configuration = GetConfiguration();
            await configuration.InvokeAsync(args);
            return;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Something went wrong");
            return;
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }

    private static CliConfiguration GetConfiguration()
    {
        CliOption<string> myScomis = new(MsOption)
        {
            Description = "The path to the myScomis Excel spreadsheet",
            Required = true,
        };
        myScomis.Aliases.Add(MsAlias);

        CliArgument<string> testDb = new(TestArgument)
        {
            Description = "The path to the test PhoneAssistant database",
            DefaultValueFactory = _ => @"c:\dev\paTest.db"
        };
        CliCommand testUpdateCommand = new(TestRun, "Apply import to TEST database")
        {
            testDb,
            myScomis
        };
        testUpdateCommand.SetAction((ParseResult parseResult) =>
        {
            string? db = parseResult.CommandResult.GetValue<string>(testDb);
            string? ms = parseResult.CommandResult.GetValue<string>(myScomis);
            Execute(parseResult, db, ms);
        });

        CliArgument<string> liveDb = new(LiveArgument)
        {
            Description = "The path to the live PhoneAssistant database",
            DefaultValueFactory = _ => @"\\countyhall.ds2.devon.gov.uk\docs\exeter, county hall\FITProject\ICTS\Mobile Phones\PhoneAssistant\PhoneAssistant.db"
        };
        CliCommand liveUpdateCommand = new(LiveRun, "Apply import to LIVE database")
        {
            liveDb,
            myScomis
        };
        liveUpdateCommand.SetAction((ParseResult parseResult) =>
        {
            string? db = parseResult.CommandResult.GetValue(liveDb);
            string? ms = parseResult.CommandResult.GetValue<string>(myScomis);
            Execute(parseResult, db, ms);
        });

        CliRootCommand rootCommand = new("Utility application to update PhoneAssistant database")
            {
                liveUpdateCommand,
                testUpdateCommand
            };
        return new CliConfiguration(rootCommand);
    }

    private static void Execute(ParseResult parseResult, string? db, string? ms)
    {
        ArgumentNullException.ThrowIfNull(parseResult);

        string runType = parseResult.CommandResult.Command.Name;
        Log.Information("Starting Import - {0} run", runType);

        if (!CheckFileExists(db)) return;
        if (!CheckFileExists(ms)) return;

        Log.Information("Applying import to {0}", db);
        Log.Information("Importing {0}", ms);
    }

    private static bool CheckFileExists(string? argument)
    {
        if (!File.Exists(argument))
        {
            Log.Error("{0} not found", argument);
            return false;
        }
        return true;
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