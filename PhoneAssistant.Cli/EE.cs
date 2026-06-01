using FluentResults;

using PhoneAssistant.Model;

using Serilog;

using System.CommandLine;
using System.IO.Compression;
using System.Text;

namespace PhoneAssistant.Cli;

internal static class EE
{
    internal static void Command(RootCommand rootCommand)
    {
        StringBuilder description = new();
        description.AppendLine("Import EE long phone summary");
        description.AppendLine();
        description.AppendLine("Inputs");
        description.AppendLine("CMP-G01916_LONG_PHONE_SUMMARY_ccyymm.ZIP");
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
        Log.Information("EE Long Phone Summary import starting");
        Log.Information("Looking for import files in folder: {Folder}", directory?.FullName);

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
        ImportHistoryRepository importRepository = new(dbContext);
        bool runExists = importRepository.RunExistsAsync(ImportType.BaseReport, billingPeriod).GetAwaiter().GetResult();
        if (runExists)
        {
            Log.Error("A run for this billing period already exists.");
            return;
        }

        try
        {
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
            Log.Information("Reading file {FileName} from ZIP", csvName);

            using StreamReader reader = new(entry.Open(), Encoding.Latin1);
            int lineCount = 0;

            string? csvHeader = reader.ReadLine();
            if (csvHeader is null || !csvHeader.StartsWith("Cost centre name")) //, StringComparison.OrdinalIgnoreCase))
                throw new Exception("Unexpected CSV format. Header line is missing or does not start with 'Cost centre name'.");
                        
            HashSet<(string, string)> sims = [];

            while (!reader.EndOfStream)
            {
                string? csvLine = reader.ReadLine();
                lineCount++;

                if (string.IsNullOrWhiteSpace(csvLine))
                    continue;

                Result<CsvSim> csvSim = CsvSim.Parse(csvLine);
                if (csvSim.IsFailed)
                    throw new Exception($"Line {lineCount + 1}: {csvSim.Errors[0].Message}");

                if (!sims.Add((csvSim.Value.PhoneNumber, billingPeriod)))
                {
                    Log.Warning("Duplicate record for phone number {PhoneNumber} and billing period {BillingPeriod} found at line {LineNumber}. Skipping.", csvSim.Value.PhoneNumber, billingPeriod, lineCount + 1);
                    continue;
                }
                var sim = new Sim
                {
                    PhoneNumber = csvSim.Value.PhoneNumber,
                    BillingPeriod = billingPeriod,

                    BroadbandData = csvSim.Value.BroadbandData,
                    TextMessages = csvSim.Value.TextMessages,
                    UserName = csvSim.Value.UserName,
                    VoiceCalls = csvSim.Value.VoiceCalls
                };

                dbContext.Sims.Add(sim);
            }
            dbContext.SaveChanges();
            Log.Information("{LineCount} records processed", lineCount);

            importRepository.CreateAsync(ImportType.BaseReport, billingPeriod).GetAwaiter().GetResult();
            dbContext.SaveChanges();
        }
        catch (FileNotFoundException)
        {
            Log.Error("ZIP file not found.");
        }
        catch (InvalidDataException)
        {
            Log.Error("Invalid ZIP file format.");
        }
        catch (Exception ex)
        {
            Log.Error($"Error: {ex.Message}");
        }
        Log.Information("EE Long Phone Summary import finished");
    }
}
