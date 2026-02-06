using FluentResults;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using Microsoft.Data.Sqlite;
using System.CommandLine;

namespace DbUtil;

public sealed class Program
{
    private static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console(theme: AnsiConsoleTheme.Sixteen)
            .MinimumLevel.Debug()
            .WriteTo.File("dbutil.log")
            .CreateLogger();

        RootCommand rootCommand = new("Utility application to apply schema update scripts to a database");

        Command liveCommand = new("live", "Apply updates to LIVE database");
        Argument<string> liveArg = new("liveDb")
        {
            Description = "The path to the live PhoneAssistant database",
            DefaultValueFactory = _ => @"\\countyhall.ds2.devon.gov.uk\docs\Exeter, County Hall\FITProject\ICTS\Mobile Phones\PhoneAssistant\phoneassistant.db"
        };
        liveCommand.Add(liveArg);

        liveCommand.SetAction(parseResult =>
        {
            try
            {
                var live = parseResult.GetValue(liveArg);
                Log.Information("Applying update to LIVE database {0}", live);
                if (string.IsNullOrEmpty(live))
                    Log.Fatal("Database path is null when parsing parameter");                    
                else
                    Execute(live);
            }
            catch (Exception ex)
            {
                Log.Fatal(exception: ex, "Unhandled exception:");
            }
        });

        Command testCommand = new("test", "Apply updates to TEST database");
        Argument<string> testArg = new("testDb")
        {
            Description = "The path to the test PhoneAssistant database",
            DefaultValueFactory = _ => @"c:\dev\paTest.db"
        };
        testCommand.Add(testArg);

        testCommand.SetAction(parseResult =>
        {
            try
            {
                var test = parseResult.GetValue(testArg);
                Log.Information("Applying update to TEST database {0}", test);
                if (string.IsNullOrEmpty(test))
                {
                    Log.Fatal("Database path is null when parsing parameter");
                    return;
                }
                Execute(test.ToString());
            }
            catch (Exception ex)
            {

                Log.Fatal(exception: ex, "Unhandled exception:");
            }
        });

        rootCommand.Add(liveCommand);
        rootCommand.Add(testCommand);

        try
        {
            rootCommand.Parse(args).Invoke();
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    public static void Execute(string db)
    {
        SqlScripts sqlScripts = new();
        using var connection = new SqliteConnection($"Data Source={db}");
        connection.Open();

        CheckSchemaVersionsExists(connection);

        foreach (Script script in sqlScripts.Scripts)
        {
            Result<bool> result = ApplyScript(connection, script);
            if (result.IsSuccess)
            {
                if (result.Value == true)
                    Log.Information("{0} applied successfully", script.Name);
                else
                    Log.Information("{0} already applied to database", script.Name);
            }
            else
            {
                Log.Fatal("Error applying {0}", script.Name);
                Log.Fatal(result.Reasons.Select(reason => reason.Message).First());
                return;
            }
        }

        Log.Information("Update scripts applied successfully.");
    }

    public static Result<bool> ApplyScript(SqliteConnection connection, Script script)
    {
        var command = connection.CreateCommand();
        command.CommandText = $"SELECT count(*) FROM SchemaVersion WHERE ScriptName = '{script.Name}';";
        SqliteDataReader reader = command.ExecuteReader();
        reader.Read();
        var count = reader.GetInt32(0);
        reader.Close();
        if (count > 0) return Result.Ok(false);

        command.CommandText = $"INSERT INTO SchemaVersion (ScriptName) VALUES ('{script.Name}');";
        _ = command.ExecuteNonQuery();

        command.CommandText = script.Sql;
        try
        {
            _ = command.ExecuteNonQuery();
        }
        catch (Exception ex)
        {

            return Result.Fail(ex.Message);
        }

        return Result.Ok(true);
    }

    public static bool CheckSchemaVersionsExists(SqliteConnection connection)
    {
        var command = connection.CreateCommand();
        command.CommandText = "SELECT count(*) FROM sqlite_master WHERE type = 'table' and name = 'SchemaVersion';";
        SqliteDataReader reader = command.ExecuteReader();
        reader.Read();
        var count = reader.GetInt32(0);
        reader.Close();

        if (count > 0) return false;

        command.CommandText = "CREATE TABLE SchemaVersion (ScriptName TEXT NOT NULL CONSTRAINT PK_SchemaVersion PRIMARY KEY, Applied TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP);";
        _ = command.ExecuteNonQuery();
        return true;
    }
}
