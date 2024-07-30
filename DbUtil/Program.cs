using System.CommandLine;
using System.Reflection;

using DbUp;
using DbUp.Engine;

namespace DbUtil;

public sealed class Program
{
    private static Task<int> Main(string[] args)
    {
        
        CliConfiguration configuration = GetConfiguration();        
        return configuration.InvokeAsync(args);
    }

    private static CliConfiguration GetConfiguration()
    {
        CliArgument<string> testDb = new("testDB")
        {
            Description = "The path to the test PhoneAssistant database",
            DefaultValueFactory = _ => @"c:\dev\paTest.db"
        };
        CliArgument<string> liveDb = new("liveDB")
        {
            Description = "The path to the live PhoneAssistant database",
            DefaultValueFactory = _ => @"K:\FITProject\ICTS\Mobile Phones\PhoneAssistant\phoneassistant.db"
        };
        CliCommand liveUpdateCommand = new("live", "Apply updates to live database")
            {
                liveDb
            };
        liveUpdateCommand.SetAction((ParseResult parseResult) =>
        {
            string? db = parseResult.CommandResult.GetValue(testDb);
            ApplyUpdates(db, parseResult);
        });

        CliRootCommand rootCommand = new("Utility application to update PhoneAssistant database")
            {
                testDb,            
                liveUpdateCommand,
            };
        rootCommand.SetAction((ParseResult parseResult) =>
        {
            string? db = parseResult.CommandResult.GetValue(testDb);
            ApplyUpdates(db, parseResult);
        });
        return new CliConfiguration(rootCommand);
    }

    private static void ApplyUpdates(string? db, ParseResult parseResult)
    {
        parseResult.Configuration.Output.WriteLine($"Applying updates to {db}");
        if (db is null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Database path not supplied");
            Console.ResetColor();
            return;
        }

        if (!File.Exists(db))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Database not found");
            Console.ResetColor();
            return;
        }

        string connectionString = $@"DataSource={db};";
        UpgradeEngine updater = DeployChanges.To
            .SQLiteDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
            .LogToConsole()
            .Build();

        List<SqlScript> scripts = updater.GetDiscoveredScripts();
        foreach (SqlScript script in scripts)
        {
            Console.WriteLine(script.Name);
        }

        DatabaseUpgradeResult result = updater.PerformUpgrade();

        if (!result.Successful)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(result.Error);
            Console.ResetColor();
            return;
        }

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Update scripts applied successfully.");
        Console.ResetColor();
    }
}