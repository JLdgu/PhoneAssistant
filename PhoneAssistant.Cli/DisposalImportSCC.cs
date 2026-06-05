using ExcelDataReader;
using FluentResults;
using Serilog;
using System.Data;

namespace PhoneAssistant.Cli;

public record SccDisposal(string PrimaryKey, string? SecondaryKey, int Certificate);

public sealed class DisposalImportSCC(string importFile)
{
    private const string SheetName = "Units";
    private const string A2 = "A2";
    private const string CheckValue = "Units";
    public const int Account = 0;
    public const int TrackerId = 2;
    public const int SerialNumber = 3;
    public const int AssetNumber = 4;
    public const int ProductType = 5;
    public const int Status = 8;

    public Result<List<SccDisposal>> Execute()
    {
        List<SccDisposal> disposals = [];
        try
        {
            using FileStream stream = new(importFile, FileMode.Open, FileAccess.Read);
            using var reader = ExcelReaderFactory.CreateReader(stream);

            // Find the Units sheet
            DataTable? sheet = null;
            do
            {
                if (reader.Name == SheetName)
                {
                    sheet = new System.Data.DataTable(SheetName);
                    var fieldCount = reader.FieldCount;

                    for (int i = 0; i < fieldCount; i++)
                    {
                        sheet.Columns.Add($"Col{i}", typeof(object));
                    }

                    while (reader.Read())
                    {
                        var values = new object[fieldCount];
                        reader.GetValues(values);
                        sheet.Rows.Add(values);
                    }
                    break;
                }
            } while (reader.NextResult());

            if (sheet is null)
                return Result.Fail($"Sheet '{SheetName}' not found");

            // Validate header (A2 is row 2, zero-based index 1)
            if (sheet.Rows.Count < 2 || sheet.Columns.Count <= Account)
                return Result.Fail("No rows found in sheet");

            var headerCell = sheet.Rows[1][Account]?.ToString()?.Trim();
            if (headerCell != CheckValue)
                return Result.Fail($"Unable to find '{CheckValue}' in cell {A2}");

            Log.Information("Processing {0} rows from SCC", sheet.Rows.Count - 1);
            for (int i = 1; i < sheet.Rows.Count; i++)
            {
                var row = sheet.Rows[i];
                Result<SccDisposal> disposal = GetDisposal(row);
                if (disposal.IsSuccess) 
                    disposals.Add(disposal.Value);
            }
            Log.Information("{0} devices found", disposals.Count);
        }
        catch (IOException)
        {
            return Result.Fail("Cannot access file, ensure it is closed and retry.");
        }

        return Result.Ok(disposals);
    }

    public static Result<SccDisposal> GetDisposal(System.Data.DataRow row)
    {
        try
        {
            if (row is null) 
                return Result.Fail("Ignore: Null row");

            var accountValue = row[Account]?.ToString()?.Trim();
            if (string.IsNullOrEmpty(accountValue) || accountValue != "D1024CT")
                return Result.Fail("Ignore: Account not D1024CT");

            var serialValue = row[SerialNumber]?.ToString()?.Trim();
            if (string.IsNullOrEmpty(serialValue) || serialValue == "NONE" || serialValue == "UNREADABLE")
                return Result.Fail("Ignore: Unidentifiable");

            var productTypeValue = row[ProductType]?.ToString()?.Trim();
            if (productTypeValue == "MONITORS")
                return Result.Fail("Ignore: Invalid product type");

            var statusValue = row[Status]?.ToString()?.Trim();
            if (string.IsNullOrEmpty(statusValue) || !statusValue.StartsWith("Despatched", StringComparison.OrdinalIgnoreCase))
                return Result.Fail("Ignore: Status not despatched");

            // Parse serial number as primary key
            string primary;
            if (long.TryParse(serialValue, out long numericSerial))
            {
                primary = numericSerial.ToString("000000000000000");
            }
            else
            {
                primary = serialValue;
            }

            // Parse asset number as secondary key
            string? secondary = null;
            var assetValue = row[AssetNumber]?.ToString()?.Trim();
            if (!string.IsNullOrEmpty(assetValue) && assetValue != "NONE")
                secondary = assetValue;

            // Parse tracker ID as certificate
            int certificate = 0;
            var trackerIdValue = row[TrackerId]?.ToString()?.Trim();
            if (!string.IsNullOrEmpty(trackerIdValue) && int.TryParse(trackerIdValue, out int cert))
                certificate = cert;

            return Result.Ok(new SccDisposal(primary, secondary, certificate));
        }
        catch (Exception)
        {
            return Result.Fail("Ignore: Error reading row");
        }
    }
}
