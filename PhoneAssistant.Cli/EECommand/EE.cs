using FluentResults;
using PhoneAssistant.Model;
using Serilog;
using System.CommandLine;
using System.IO.Compression;
using System.Text;

namespace PhoneAssistant.Cli.EECommand;

internal static class EE
{
    internal static void Command(RootCommand rootCommand)
    {
        StringBuilder description = new();
        description.AppendLine("Import EE long phone summary");
        description.AppendLine();
        description.AppendLine("Inputs");
        description.AppendLine("CMP-G01916_LONG_PHONE_SUMMARY_ccyymm.ZIP");
        description.AppendLine("Manage SIMs.csv");
        Command eeCommand = new("ee", description.ToString());

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
                        result.AddError($"Directory {dir?.FullName ?? "null"} does not exist.");
                        return;
                    }
                }
            }
        };
        eeCommand.Add(folderOption);

        eeCommand.SetAction(parseResult =>
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

        rootCommand.Add(eeCommand);
    }

    private static void Execute(DirectoryInfo? directory)
    {
        Log.Information("Looking for import files in folder: {Folder}", directory?.FullName);

        FileInfo? manageSIMs = directory?.GetFiles("Manage SIMs.csv").FirstOrDefault();
        if (manageSIMs is null)
        {
            Log.Error("No 'Manage SIMs.csv' was found in the specified folder.");
            return;
        }
        Log.Information("Reading file {0}", manageSIMs.FullName);

        Result<Dictionary<string, SIMDetail>> simDetailsResult = SIMDetailParser.LoadSimDetails(manageSIMs.FullName);
        if (simDetailsResult.IsFailed)
        {
            Log.Error(simDetailsResult.Errors[0].Message);
            return;
        }

        FileInfo? summary = directory?.GetFiles("CMP-G01916_LONG_PHONE_SUMMARY_*.zip")
                    .OrderByDescending(f => f.Name)
                    .FirstOrDefault();
        if (summary is null)
        {
            Log.Error("No 'CMP-G01916_LONG_PHONE_SUMMARY_*.zip' was found in the specified folder.");
            return;
        }

        string billingPeriod = summary.Name.Replace(".zip", "", StringComparison.OrdinalIgnoreCase).Split('_').Last();
        Log.Information("Billing period identified as: {BillingPeriod}", billingPeriod);

        PhoneAssistantDbContext dbContext = ModelContext.Create();
        SimRepository repository = new(dbContext);
        string latestBillingPeriod = repository.GetLatestBillingPeriod().GetAwaiter().GetResult();
        if (latestBillingPeriod == billingPeriod)
        {
            Log.Error("A run for this billing period already exists.");
            return;
        }        

        using FileStream zipToOpen = new(summary.FullName, FileMode.Open, FileAccess.Read);
        using ZipArchive archive = new(zipToOpen, ZipArchiveMode.Read);

        if (archive.Entries.Count == 0)
        {
            Log.Error("The ZIP file is empty.");
            return;
        }
        var csvName = archive.Entries[0].Name;

        ZipArchiveEntry? entry = archive.GetEntry(csvName);
        if (entry is null)
        {
            Log.Error("File {0} not found in ZIP.", csvName);
            return;
        }
        Log.Information("Reading file {0} from ZIP", csvName);

        Result<Dictionary<string, PhoneSummary>> phoneSummariesResult = PhoneSummaryParser.LoadPhoneSummaries(entry);
        if (phoneSummariesResult.IsFailed)
        {
            Log.Error(phoneSummariesResult.Errors[0].Message);
            return;
        }

        int lineCount = 0;
        foreach (var phoneSummary in phoneSummariesResult.Value)
        {
            string simNumber = "Unknown";
            if (simDetailsResult.Value.TryGetValue(phoneSummary.Key, out var simDetail))
                simNumber = simDetail.SimNumber;

            var sim = new Sim
            {
                PhoneNumber = phoneSummary.Key,
                BillingPeriod = billingPeriod,
                SIMNumber = simNumber,
                BroadbandData = phoneSummary.Value.BroadbandData,
                TextMessages = phoneSummary.Value.TextMessageCount,
                UserName = phoneSummary.Value.UserName,
                VoiceCalls = phoneSummary.Value.VoiceCallCount
            };

            lineCount++;
            dbContext.Sims.Add(sim);
        }

        dbContext.SaveChanges();
        Log.Information("Import completed. {LineCount} records processed.", lineCount);
    }
}
