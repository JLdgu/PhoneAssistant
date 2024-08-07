using System.CommandLine;
using System.Reflection;
using System.Xml.Linq;

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
        CliOption<bool> dryRun = new("--dryRun", "-d");

        CliCommand liveUpdateCommand = new("live", "Apply updates to LIVE database")
        {
            liveDb,
            dryRun
        };
        liveUpdateCommand.SetAction((ParseResult parseResult) =>
        {
            string? db = parseResult.CommandResult.GetValue(liveDb);
            bool upgrade = !parseResult.CommandResult.GetValue(dryRun);
            ApplyUpdates(db, parseResult, upgrade);
        });

        CliCommand testUpdateCommand = new("test", "Apply updates to TEST database")
        {
            testDb,
            dryRun
        };
        testUpdateCommand.SetAction((ParseResult parseResult) =>
        {
            string? db = parseResult.CommandResult.GetValue(testDb);
            bool upgrade = !parseResult.CommandResult.GetValue(dryRun);
            ApplyUpdates(db, parseResult, upgrade);
        });

        CliRootCommand rootCommand = new("Utility application to update PhoneAssistant database")
            {
                liveUpdateCommand,
                testUpdateCommand

            };
        return new CliConfiguration(rootCommand);
    }

    private static void ApplyUpdates(string? db, ParseResult parseResult, bool upgrade)
    {
        string dryRun = "DRY RUN: ";
        if (upgrade) dryRun = string.Empty;

        parseResult.Configuration.Output.WriteLine("{0}Applying updates to {1}", dryRun, db);
        if (db is null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("{0}Database path not supplied", dryRun);
            Console.ResetColor();
            return;
        }

        if (!File.Exists(db))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("{0}Database not found", dryRun);
            Console.ResetColor();
            return;
        }
        if (upgrade)
        {
            DirectoryInfo dbPath = new(Path.Combine(Path.GetDirectoryName(db)!, "Backup"));
            string dbName = new FileInfo(db).Name;
            string backup = Path.Combine(dbPath.FullName, dbName.Replace(".", $"{DateTime.Now:yyy-MM-dd}_PreMigration."));
            int x = 0;
            while (File.Exists(backup))
            {
                backup = Path.Combine(dbPath.FullName, dbName.Replace(".", $"{DateTime.Now:yyy-MM-dd}_PreMigration{x}."));
                x++;
            }
            File.Copy(db, backup);
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
            Console.WriteLine("{0}{1}", dryRun, script.Name);
        }

        if (upgrade)
        {
            DatabaseUpgradeResult result = updater.PerformUpgrade();

            if (!result.Successful)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(result.Error);
                Console.ResetColor();
                return;
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("{0}Update scripts applied successfully.", dryRun);
            Console.ResetColor();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("{0}No update scripts were applied", dryRun);
            Console.ResetColor();
        }
    }
}