using FluentResults;
using Serilog;
using System.CommandLine;
using System.Text;

namespace PhoneAssistant.Cli;

internal static class Disposal
{
    internal static void Command(RootCommand rootCommand)
    {
        StringBuilder description = new();
        description.AppendLine("Reconcile phone disposals");
        description.AppendLine("An export from myScomis 'All Telephony CIs");
        description.AppendLine("CI Listccyy_mm_dd_hh_mm_ss.xlsx");
        description.AppendLine("Units D1024CT ccyy-mm-dd.xls SCC export");
        Command disposalCommand = new("disposal", description.ToString());

        Option<int> srOption = new("--serviceRequest", "-sr")
        {
            Description = "Service Request of disposals",
            Required = true
        };
        disposalCommand.Add(srOption);

        Option<int> crOption = new("--collectionRequest", "-cr")
        {
            Description = "Collection Request of disposals",
            Required = true
        };
        disposalCommand.Add(crOption);

        Option<DirectoryInfo> folderOption = new("--folder", "-f")
        {
            Description = "Path to the folder where import files exist",
            Required = true,
            Validators =
            {
                result =>
                {
                    var dir = result.GetValueOrDefault<DirectoryInfo>();
                    if (dir == null || !dir.Exists)
                    {
                        result.AddError( "The specified folder does not exist.");
                    }
                }
            }
        };
        disposalCommand.Add(folderOption);

        disposalCommand.SetAction(parseResult =>
        {
            try
            {
                var sr = parseResult.GetValue(srOption);
                var cr = parseResult.GetValue(crOption);
                var folder = parseResult.GetValue(folderOption);
                Execute(sr: sr, scc: cr, directory: folder);
            }
            catch (Exception ex)
            {
                Log.Fatal(exception: ex, "Unhandled exception:");
            }
        });

        rootCommand.Add(disposalCommand);
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

        DisposalImportMS importMS = new(msImport);
        Result<List<Device>> msResult = importMS.Execute();
        if (msResult.IsFailed)
        {
            Log.Error(msResult.Errors.First().Message);
            return;
        }

        DisposalImportSCC importSCC = new(sccImport);
        Result<List<SccDisposal>> sccResult = importSCC.Execute();
        if (sccResult.IsFailed)
        {
            Log.Error(sccResult.Errors.First().Message);
            return;
        }

        DisposalExport export = new(sr: sr, disposals: sccResult.Value, devices: msResult.Value, exportDirectory: directory!);
        FluentResults.Result exportResult = export.Execute();
        if (exportResult.IsFailed)
        {
            Log.Error(sccResult.Errors.First().Message);
            return;
        }

        Log.Information("Reconcile finished");
    }

}
