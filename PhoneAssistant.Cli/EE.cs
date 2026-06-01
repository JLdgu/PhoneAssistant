using FluentResults;
using Microsoft.VisualBasic.FileIO;
using PhoneAssistant.Model;
using Serilog;
using System.CommandLine;
using System.Globalization;
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
                        result.AddError( "The specified folder does not exist.");
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

            PhoneAssistantDbContext dbContext = ModelContext.Create();
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
                    /*
                     * TODO: Monthly recurring charges
                     * Other costs
                     * Number of voice calls
                     * Number of text messages
                     * Quantity of GPRS data (bytes)
                    */

                    PhoneNumber = csvSim.Value.PhoneNumber,
                    BillingPeriod = billingPeriod,

                    MonthlyRecurringCharge = "£0.00",
                    OtherCosts = "£0.00",
                    VoiceCalls = 0,
                    TextMessages = 0,
                    BroadbandData = 0,
                    UserName = csvSim.Value.UserName
                };

                dbContext.Sims.Add(sim);
            }
            dbContext.SaveChanges();
            Log.Information("{LineCount} records processed", lineCount);

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

internal class CsvSim(string phoneNumber, string userName, decimal recurringCharge, long dataVolume)
{
    internal string PhoneNumber { get; init; } = phoneNumber ?? throw new ArgumentNullException(nameof(phoneNumber));
    internal long DataVolume { get; init; } = dataVolume;
    internal decimal RecurringCharge { get; init; } = recurringCharge;
    internal string UserName { get; init; } = userName;

    private static readonly string[] ExpectedHeader =
    [
        "Cost centre name",
        "Cost centre code",
        "Phone number",
        "User name",
        "Monthly recurring charges",
        "Other costs",
        "Call costs (airtime)",
        "Credits",
        "Total costs (excluding VAT)",
        "Cost of Voice",
        "Cost of SMS",
        "Cost of MMS",
        "Cost of Data",
        "Cost of GPRS",
        "Cost of Fax",
        "Cost of Email",
        "Number of voice calls",
        "Duration of voice calls",
        "Number of SMS",
        "Number of MMS (photo and video)",
        "Duration of data calls (CSD/HSCSD)",
        "Quantity of data (bytes)",
        "Quantity of GPRS data (bytes)",
        "Number of faxes",
        "Number of emails",
        "Duration of landline",
        "Duration of answerphone",
        "Number of text messages",
        "Duration of calls to EE mobiles (EE to EE)",
        "Duration of calls to other mobiles (other mobile network)",
        "Duration of calls to other",
        "Duration of calls to roaming",
        "Duration of calls to international",
        "Duration of calls to premium rate numbers",
        "Duration of calls to mobile voice VPN",
        "Invoice number",
        "Account/Group",
        "VAT exempt call costs (airtime)"
    ];

    internal static Result<string> ValidateHeader(string headerLine)
    {
        using var parser = new TextFieldParser(new StringReader(headerLine));
        parser.HasFieldsEnclosedInQuotes = true;
        parser.SetDelimiters(",");

        string[]? fields = parser.ReadFields();

        if (fields is null || fields.Length != ExpectedHeader.Length)
        {
            return Result.Fail<string>($"Header line has {fields?.Length ?? 0} columns, expected {ExpectedHeader.Length}");
        }

        for (int i = 0; i < ExpectedHeader.Length; i++)
        {
            if (fields[i].Trim('"') != ExpectedHeader[i])
            {
                return Result.Fail<string>($"Column {i} header mismatch. Expected '{ExpectedHeader[i]}', but got '{fields[i].Trim('"')}'");
            }
        }

        return Result.Ok(headerLine);
    }

    internal static Result<CsvSim> Parse(string csvLine)
    {
        using var parser = new TextFieldParser(new StringReader(csvLine));
        parser.HasFieldsEnclosedInQuotes = true;
        parser.SetDelimiters(",");

        string[]? fields = parser.ReadFields();

        if (fields is null || fields.Length < 5)
            throw new FormatException("CSV line does not contain enough columns.");

        string phoneNumber = fields[2];
        string userName = fields[3];

        string recurringChargeText = fields[4].Trim('"');
        if (!decimal.TryParse(recurringChargeText, NumberStyles.Currency, CultureInfo.GetCultureInfo("en-GB"), out var recurringChargeValue))
        {
            return Result.Fail<CsvSim>("Unable to parse recurring charge value");
        }

        string dataVolumeText = fields[21].Trim('"');
        if (!long.TryParse(dataVolumeText, NumberStyles.AllowThousands, CultureInfo.GetCultureInfo("en-GB"), out var dataVolumeValue))
        {
            return Result.Fail<CsvSim>("Unable to parse data volume value");
        }

        return new CsvSim(phoneNumber, userName, recurringChargeValue, dataVolumeValue);
    }
}
