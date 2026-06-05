using ExcelDataReader;
using FluentResults;
using Serilog;
using System.Data;

namespace PhoneAssistant.Cli;

public record Device(string Name, string AssetTag, string SerialNumber, string Status, int Certificate = 0);

public sealed class DisposalImportMS(string importFile)
{
    private const string SheetName = "Data";
    private const string A1 = "A1";
    private const string CheckValue = "Category";
    public const int Category = 0;
    public const int ItemType = 1;
    public const int Name = 3;
    public const int AssetTag = 4;
    public const int Status = 7;
    public const int OEM = 8;
    public const int Model = 9;
    public const int SerialNumber = 10;

    public Result<List<Device>> Execute()
    {
        List<Device> devices = [];

        try
        {
            using FileStream stream = new(importFile, FileMode.Open, FileAccess.Read);
            using var reader = ExcelReaderFactory.CreateReader(stream);

            // Find the Data sheet
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
                return Result.Fail<List<Device>>($"Sheet '{SheetName}' not found");

            // Validate header
            if (sheet.Rows.Count == 0 || sheet.Columns.Count <= Category)
                return Result.Fail<List<Device>>("No rows found in sheet Data");

            var headerCell = sheet.Rows[0][Category]?.ToString()?.Trim();
            if (headerCell != CheckValue)
                return Result.Fail<List<Device>>($"Unable to find '{CheckValue}' in cell {A1}");

            Log.Information("Processing {0} rows from myScomis", sheet.Rows.Count - 1);
            Progress progress = new();

            for (int i = 1; i < sheet.Rows.Count; i++)
            {
                var row = sheet.Rows[i];
                Result<Device> resultDevice = GetDevice(row);
                if (resultDevice.IsSuccess)
                {
                    devices.Add(resultDevice.Value);
                }

                if (i % 10 == 0 || i == sheet.Rows.Count - 1)
                {
                    progress.Draw(i, sheet.Rows.Count - 1);
                }
            }
            Log.Information("{0} devices found", devices.Count);
        }
        catch (IOException)
        {
            return Result.Fail<List<Device>>("Cannot access file, ensure it is closed and retry.");
        }

        return Result.Ok<List<Device>>(devices);
    }

    public static Result<Device> GetDevice(System.Data.DataRow row)
    {
        try
        {
            var itemType = row[ItemType]?.ToString()?.Trim();
            if (itemType != "Computer" && itemType != "Phone")
                return Result.Fail<Device>("Ignore: Item Type");

            switch (itemType)
            {
                case "Computer":
                    var oem = row[OEM]?.ToString()?.Trim();
                    if (oem != "Apple")
                        return Result.Fail<Device>("Ignore: Manufacturer");
                    break;
                case "Phone":
                    var model = row[Model]?.ToString()?.Trim();
                    if (model == "SIM Card")
                        return Result.Fail<Device>("Ignore: Model");
                    break;
            }

            Device device = new(
                    Name: row[Name]?.ToString()?.Trim() ?? "",
                    AssetTag: row[AssetTag]?.ToString()?.Trim() ?? "",
                    SerialNumber: row[SerialNumber]?.ToString()?.Trim() ?? "",
                    Status: row[Status]?.ToString()?.Trim() ?? ""
                    );

            return Result.Ok<Device>(device);
        }
        catch (Exception)
        {
            return Result.Fail<Device>("Error reading row");
        }
    }
}
