using FluentResults;

using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

using System.CommandLine;
using System.Text;

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

        StringBuilder sb = new();
        sb.AppendLine("Utility application to reconcile phone disposals");
        sb.AppendLine("File name expected formats are:");
        sb.AppendLine("CI List.xlsx for myScomis import");
        sb.AppendLine("SR[sr] CR[cr] Units.xlsx for SCC import");
        RootCommand rootCommand = new(sb.ToString());

        var srOption = new Option<int>("--serviceRequest") { Description = "Service Request of disposals" };
        srOption.AddAlias("-sr");
        srOption.IsRequired = true;
        rootCommand.AddOption(srOption);

        var crOption = new Option<int>("--collectionRequest") { Description = "Service Request of disposals" };
        crOption.AddAlias("-cr");
        crOption.IsRequired = true;
        rootCommand.AddOption(crOption);

        var folderOption = new Option<DirectoryInfo>("--folder") { Description = "Path to the folder where import files exist" };
        folderOption.AddAlias("-f");
        folderOption.ExistingOnly();
        folderOption.IsRequired = true;
        rootCommand.AddOption(folderOption);

        rootCommand.SetHandler((sr, cr, folder) =>
        {
            try
            {
                Execute(sr: sr, scc: cr, directory: folder);
            }
            catch (Exception ex)
            {
                Log.Fatal(exception: ex, "Unhandled exception:");
            }
        },srOption, crOption, folderOption);

        try
        {
            rootCommand.Invoke(args);
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    private static void Execute(int sr, int scc, DirectoryInfo? directory)
    {
        Log.Information("Reconcile starting");

        string msImport = Path.Combine(directory!.FullName, "CI List.xlsx");
        if (!File.Exists(msImport))
        {
            Log.Error("Unable to find {0}", msImport);
            return;
        }

        string sccImport = Path.Combine(directory!.FullName, $"SR{sr} CR{scc} Units.xls");
        if (!File.Exists(sccImport))
        {
            Log.Error("Unable to find {0}", sccImport);
            return;
        }

        ImportMS importMS = new(msImport);
        Result<List<Device>> msResult = importMS.Execute();
        if (msResult.IsFailed)
        {
            Log.Error(msResult.Errors.First().Message);
            return;
        }

        ImportSCC importSCC = new(sccImport);
        Result<List<Disposal>> sccResult = importSCC.Execute();
        if (sccResult.IsFailed)
        {
            Log.Error(sccResult.Errors.First().Message);
            return;
        }

        Export export = new(sr: sr, disposals: sccResult.Value, devices: msResult.Value, exportDirectory: directory!);
        Result exportResult = export.Execute();
        if (exportResult.IsFailed)
        {
            Log.Error(sccResult.Errors.First().Message);
            return;
        }

        Log.Information("Reconcile finished");
    }
}