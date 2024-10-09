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

        var sr = new CliOption<int>("--sr") { Description = "Service Request of disposals", Required = true };
        var myScomis = new CliOption<FileInfo>("--myScomis") { Description = "Path to the myScomis Excel spreadsheet", Required = true }.AcceptExistingOnly();
        myScomis.Aliases.Add("-ms");
        var scc = new CliOption<FileInfo>("--scc") { Description = "Path to the SCC Excel spreadsheet", Required = true }.AcceptExistingOnly();
        scc.Aliases.Add("-s");
        var export = new CliOption<DirectoryInfo>("--export") { Description = "Path to the folder where output files will be created", Required = true }.AcceptExistingOnly();
        export.Aliases.Add("-e");
        CliRootCommand rootCommand = new("Utility application to reconcile phone disposals")
        {
            sr,
            myScomis,
            scc, 
            export
        };
        rootCommand.SetAction((parseResult) =>
        {
            try
            {
                Execute(
                    sr: parseResult.CommandResult.GetValue<int>(sr),
                    msExcel: parseResult.CommandResult.GetValue<FileInfo>(myScomis),
                    scc: parseResult.CommandResult.GetValue<FileInfo>(scc),
                    exportDirectory: parseResult.CommandResult.GetValue(export)
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

    private static void Execute(int sr, FileInfo? msExcel, FileInfo? scc, DirectoryInfo? exportDirectory)
    {
        Log.Information("Reconcile starting");

        ImportMS importMS = new(msExcel!.FullName);
        Result<List<Device>> msResult = importMS.Execute();
        if (msResult.IsFailed)
        {
            Log.Error(msResult.Errors.First().Message);
            return;
        }

        ImportSCC importSCC = new(scc!.FullName);
        Result<List<Disposal>> sccResult = importSCC.Execute();
        if (sccResult.IsFailed)
        {
            Log.Error(sccResult.Errors.First().Message);
            return;
        }

        Export export = new(sr: sr, disposals: sccResult.Value, devices: msResult.Value, exportDirectory: exportDirectory!);
        Result exportResult = export.Execute();
        if (exportResult.IsFailed)
        {
            Log.Error(sccResult.Errors.First().Message);
            return;
        }

        Log.Information("Reconcile finished");
    }
}