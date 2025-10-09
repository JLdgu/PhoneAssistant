using CsvHelper;
using CsvHelper.Configuration.Attributes;
using NPOI.SS.UserModel;
using Serilog;
using System.Globalization;

namespace PhoneAssistant.Cli;

public  static class Knox
{
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
        using IWorkbook workbook = WorkbookFactory.Create(stream);
        ISheet ciSheet = workbook.GetSheetAt(0);

        IRow headerRow = ciSheet.GetRow(ciSheet.FirstRowNum);
        string? cellValue = headerRow.GetCell(3)?.ToString()?.Trim();
        if (cellValue != "Name")
        {
            Log.Error("Expected 'Name' in cell D1, found '{0}'.", cellValue);
            return;
        }

        int rows = ciSheet.LastRowNum - ciSheet.FirstRowNum;
        Log.Information("Processing {0} rows from the Excel file.",rows);
        Progress progress = new();

        List<SIM> matchedIMEIs = [];
        for (int i = (ciSheet.FirstRowNum + 1); i <= ciSheet.LastRowNum; i++)
        {
            if (i % 10 == 0 || i == rows)
            {
                progress.Draw(i, rows);                
            }

            IRow row = ciSheet.GetRow(i);
            if (row == null) continue;
            string? imei = row.GetCell(3).ToString()?.Trim();
            string? status = row.GetCell(7).ToString()?.Trim();

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
    [Name("Serial Number")]
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
    [Name("Enrollment Profile")]
    public string? EnrollmentProfile { get; set; }
    public string? Status { get; set; }
    [Name("Reseller ID")]
    public string? ResellerID { get; set; }
    [Name("Reseller Name")]
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