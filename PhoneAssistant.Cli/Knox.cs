using CsvHelper;
using CsvHelper.Configuration.Attributes;
using ExcelDataReader;
using Serilog;
using System.CommandLine;
using System.Data;
using System.Globalization;
using System.Text;

namespace PhoneAssistant.Cli;

internal static class Knox
{    
    internal static void Command(RootCommand rootCommand)
    {
        StringBuilder sb = new();
        sb.AppendLine("Create a csv file containing decommissioned/disposed IMEIs that can be bulk imported to Samsung Knox.");
        sb.AppendLine();
        sb.AppendLine("The output file (knox_import.csv) will contain IMEIs that are marked as decommissioned and disposed on myScomis.");
        sb.AppendLine("Input file name expected formats are:");
        sb.AppendLine("CI List*.xlsx for myScomis import - most recent will be used");
        sb.AppendLine("kme_devices.csv");
        sb.AppendLine();
        sb.AppendLine("All files should be placed in the folder specified. Defaults to users Downloads folder");

        Command knoxCommand = new("knox", sb.ToString());
        Option<DirectoryInfo> workFolderOption = new("--folder", "-f")
        {
            Description = "Path to the folder where the output csv file should be created",
            Validators =
            {
                result =>
                {
                    var dir = result.GetValueOrDefault<DirectoryInfo>();
                    if (dir == null || !dir.Exists)
                    {
                        result.AddError("The specified folder does not exist.");
                    }
                }
            }
        };
        knoxCommand.Add(workFolderOption);

        knoxCommand.SetAction(parseResult =>
        {
            try
            {
                var outputFolder = parseResult.GetValue(workFolderOption);
                if (outputFolder is null)
                    Log.Fatal("Output folder is required");
                else
                {
                    Log.Information("Creating Knox import file");
                    Knox.Execute(outputFolder);
                }
            }
            catch (Exception ex)
            {
                Log.Fatal(exception: ex, "Unhandled exception:");
            }
        });

        rootCommand.Add(knoxCommand);
    }

    public static void Execute(DirectoryInfo workFolder)
    {
        workFolder ??= new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads"));
        if (!workFolder.Exists)
        {
            Log.Error("The folder {0} does not exist.", workFolder.FullName);
            return;
        }

        // Find the latest "CI List *.xlsx" file in the workFolder directory
        var ciFiles = workFolder.GetFiles("CI List*.xlsx");
        if (ciFiles.Length == 0)
        {
            Log.Error("No 'CI List *.xlsx' file found in {0}.", workFolder.FullName);
            return;
        }
        FileInfo ciFile = ciFiles.OrderByDescending(f => f.LastWriteTime).First();

        FileInfo knoxFile = new(Path.Combine(workFolder.FullName, "kme_devices.csv"));
        if (!knoxFile.Exists)
        {
            Log.Error("No 'kme_devices.csv' file found in {0}.", workFolder.FullName);
            return;

        }

        using var reader = new StreamReader(knoxFile.FullName);
        using var csvFile = new CsvReader(reader, CultureInfo.InvariantCulture);

        List<ActiveKnoxSIM> activeSIMs = [.. csvFile.GetRecords<ActiveKnoxSIM>()];
        HashSet<string> imeiSet = [.. activeSIMs.Select(sim => sim.IMEI_MEID)];

        using FileStream stream = new(ciFile.FullName, FileMode.Open, FileAccess.Read);
        using var excelReader = ExcelReaderFactory.CreateReader(stream);

        // Convert the first sheet to DataTable
        var ciSheet = new System.Data.DataTable();
        var fieldCount = excelReader.FieldCount;

        for (int i = 0; i < fieldCount; i++)
        {
            ciSheet.Columns.Add($"Col{i}", typeof(object));
        }

        while (excelReader.Read())
        {
            var values = new object[fieldCount];
            excelReader.GetValues(values);
            ciSheet.Rows.Add(values);
        }

        if (ciSheet.Rows.Count == 0 || ciSheet.Columns.Count <= 3)
        {
            Log.Error("Expected 'Name' in cell D1, found empty sheet.");
            return;
        }

        string? cellValue = ciSheet.Rows[0][3]?.ToString()?.Trim();
        if (cellValue != "Name")
        {
            Log.Error("Expected 'Name' in cell D1, found '{0}'.", cellValue);
            return;
        }

        int rows = ciSheet.Rows.Count - 1;
        Log.Information("Processing {0} rows from the Excel file.", rows);
        Progress progress = new();

        List<SIM> matchedIMEIs = [];
        for (int i = 1; i < ciSheet.Rows.Count; i++)
        {
            if (i % 10 == 0 || i == rows)
            {
                progress.Draw(i, rows);                
            }

            var row = ciSheet.Rows[i];
            if (row == null) continue;
            string? imei = row[3]?.ToString()?.Trim();
            string? status = row[7]?.ToString()?.Trim();

            if (string.IsNullOrEmpty(imei)) continue;
            if (string.IsNullOrEmpty(status)) continue;

            if (!status.Equals("Decommissioned", StringComparison.OrdinalIgnoreCase) &&
                !status.Equals("Disposed", StringComparison.OrdinalIgnoreCase)) continue;
            if (!imeiSet.Contains(imei)) continue;

            matchedIMEIs.Add(new SIM(imei));

        }
        using var writer = new StreamWriter(Path.Combine(workFolder.FullName,"knox_bulk_delete.csv"));
        using var csvOut = new CsvWriter(writer, CultureInfo.InvariantCulture);
        csvOut.WriteRecords(matchedIMEIs);

        Log.Information("Matched {0} IMEIs.", matchedIMEIs.Count);
    }
}


public class ActiveKnoxSIM
{
    [Name("IMEI/MEID")]
    public required string IMEI_MEID { get; set; }
    public string? IMEI2 { get; set; }
    [Name("Serial number")]
    public string? SerialNumber { get; set; }
    [Name("Wi-Fi: Device MAC Address")]
    public string? WifiDeviceMacAddress { get; set; }
    [Name("Order Date")]
    public string? OrderDate { get; set; }
    [Name("Order Number")]
    public string? OrderNumber { get; set; }
    public string? Model { get; set; }
    [Name("User ID")]
    public string? UserID { get; set; }
    public string? Tags { get; set; }
    public string? Submitted { get; set; }
    [Name("Enrollment profile")]
    public string? EnrollmentProfile { get; set; }
    public string? Status { get; set; }
    [Name("Reseller ID")]
    public string? ResellerID { get; set; }
    [Name("Reseller name")]
    public string? ResellerName { get; set; }
    [Name("Last Modified Time")]
    public string? LastModifiedTime { get; set; }
    [Name("Last Seen")]
    public string? LastSeen { get; set; }
}

public class SIM(string imei)
{
    public string IMEI { get; init; } = imei;
}