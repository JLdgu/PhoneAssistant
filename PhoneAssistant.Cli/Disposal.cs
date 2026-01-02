using System.CommandLine;
using System.Text;

using FluentResults;

using PhoneAssistant.Model;

using Serilog;

namespace PhoneAssistant.Cli;

internal static class Disposal
{
    internal static void Command(RootCommand rootCommand)
    {
        StringBuilder description = new();
        description.AppendLine("Reconcile phone/tablet disposals");
        description.AppendLine();
        description.AppendLine("Inputs");
        description.AppendLine("CI Listccyy_mm_dd_hh_mm_ss.xlsx - an export from myScomis 'All Telephony CIs'");
        description.AppendLine("Units D1024CT ccyy-mm-dd.xls - an export from SCC for a collection request");
        description.AppendLine();
        description.AppendLine("Outputs");
        description.AppendLine("Disposal_ccyymmdd_hhmmss.xlsx - a file to be imported to myScomis to update CI status");
        description.AppendLine("DisposalNotes_ccyymmdd_hhmmss.xlsx - a file to be imported to myScomis to update CI notes");
        Command disposalCommand = new("disposal", description.ToString());

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
                        return;
                    }
                }
            }
        };
        disposalCommand.Add(folderOption);

        disposalCommand.SetAction(parseResult =>
        {
            try
            {
                DirectoryInfo? folder = parseResult.GetValue(folderOption);
                Execute(directory: folder);
            }
            catch (Exception ex)
            {
                Log.Fatal(exception: ex, "Unhandled exception:");
            }
        });

        rootCommand.Add(disposalCommand);
    }

    private static void Execute(DirectoryInfo? directory)
    {
        Log.Information("Disposal reconciliation starting");
        Log.Information("Looking for import files in folder: {Folder}", directory?.FullName);

        FileInfo? msImport = directory?.GetFiles("CI List*.xlsx")
                    .OrderByDescending(f => f.Name)
                    .FirstOrDefault();
        if (msImport is null)
        {
            Log.Error("No 'CI List*.xlsx' file was found in the specified folder.");
            return;
        }
        Log.Information("Using myScomis file: {FileName}", msImport.Name);

        FileInfo? sccImport = directory?.GetFiles("Units D1024CT *.xls")
                    .OrderByDescending(f => f.Name)
                    .FirstOrDefault();
        if (sccImport is null)
        {
            Log.Error("No 'Units D1024CT *.xlsx' file was found in the specified folder.");
            return;
        }
        Log.Information("Using SCC file: {FileName}", sccImport.Name);

        DisposalImportMS importMS = new(msImport.FullName);
        Result<List<Device>> msResult = importMS.Execute();
        if (msResult.IsFailed)
        {
            Log.Error(msResult.Errors[0].Message);
            return;
        }

        DisposalImportSCC importSCC = new(sccImport.FullName);
        Result<List<SccDisposal>> sccResult = importSCC.Execute();
        if (sccResult.IsFailed)
        {
            Log.Error(sccResult.Errors[0].Message);
            return;
        }

        DisposalExport export = new(disposals: sccResult.Value, devices: msResult.Value, exportDirectory: directory!);
        FluentResults.Result exportResult = export.Execute();
        if (exportResult.IsFailed)
        {
            Log.Error(sccResult.Errors[0].Message);
            return;
        }

        Log.Information("Disposal reconciliation finished");
    }

}
