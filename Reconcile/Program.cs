using FluentResults;

using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

using System.CommandLine;

namespace Reconcile;

public sealed class Program
{
    private static void Main(string[] args)
    {
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
        var scc = new CliOption<FileInfo>("--scc") { Description = "Path to the SCC Excel spreadsheet", Required = true }.AcceptExistingOnly();
        scc.Aliases.Add("-s");
        CliRootCommand rootCommand = new("Utility application to reconcile phone disposals")
        {
            myScomis,
            scc
        };
        rootCommand.SetAction((parseResult) =>
        {
            try
            {
                Execute(
                    msExcel: parseResult.CommandResult.GetValue<FileInfo>(myScomis),
                    scc: parseResult.CommandResult.GetValue<FileInfo>(scc)
                    );
            }
            catch (Exception ex)
            {
                Log.Fatal(exception: ex, "Unhandled exception:");
            }
        });

        try
        {
            rootCommand.Parse(args).Invoke();
        }
        finally
        {
            Log.Debug("Closing log");
            Log.CloseAndFlush();
        }
    }

    private static void Execute(FileInfo? msExcel, FileInfo? scc)
    {
        Log.Information("Reconcile Starting");

        ImportMS importMS = new(msExcel!.FullName);
        Result<List<Device>> msResult = importMS.Execute();
        if (msResult.IsFailed)
        {
            Log.Error(msResult.Errors.First().Message);
            return;
        }

        ImportSCC importSCC = new(scc!.FullName, msResult.Value);
        Result sccResult = importSCC.Execute();
        if (sccResult.IsFailed)
        {
            Log.Error(sccResult.Errors.First().Message);
            return;
        }
    }
}