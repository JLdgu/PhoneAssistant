using FluentResults;
using NPOI.SS.UserModel;
using Serilog;

namespace Reconcile;

public record Device(string Name, string AssetTag, string SerialNumber, string Status, int Certificate = 0);

public sealed class ImportMS(string importFile)
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
        Log.Information("Importing {0}", importFile);

        List<Device> devices = [];

        try
        {
            using FileStream stream = new(importFile, FileMode.Open, FileAccess.Read);
            using IWorkbook workbook = WorkbookFactory.Create(stream);

            Result<ISheet> resultSheet = Import.IsValidSheet(workbook, SheetName, CheckValue, A1);
            if (resultSheet.IsFailed)
            {
                return Result.Fail<List<Device>>(resultSheet.Errors);
            }
            ISheet sheet = resultSheet.Value;
            Log.Information("Processing {0} rows", sheet.LastRowNum - sheet.FirstRowNum);

            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);
                Result<Device> resultDevice = GetDevice(row);
                if (resultDevice.IsSuccess)
                {
                    devices.Add(resultDevice.Value);
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

    public static Result<Device> GetDevice(IRow row)
    {
        if (row.GetCell(ItemType).StringCellValue != "Computer" && row.GetCell(ItemType).StringCellValue != "Phone") return Result.Fail<Device>("Ignore: Item Type");

        switch (row.GetCell(ItemType).StringCellValue)
        {
            case "Computer":
                if (row.GetCell(OEM).StringCellValue != "Apple")
                    return Result.Fail<Device>("Ignore: Manufacturer");
                break;
            case "Phone":
                if (row.GetCell(Model).StringCellValue == "SIM Card")
                    return Result.Fail<Device>("Ignore: Model");
                break;
        }

        Device device = new(
                Name: row.GetCell(Name).StringCellValue,
                AssetTag: row.GetCell(AssetTag).StringCellValue,
                SerialNumber: row.GetCell(SerialNumber).StringCellValue,
                Status: row.GetCell(Status).StringCellValue
                );
        
        //Log.Debug("Item Type: {0} | OEM: {1} | Model: {2} | Name: {3} | Asset Tag: {4} | Serial Number: {5} | Status: {6}",
        //    row.GetCell(ItemType).StringCellValue, row.GetCell(OEM).StringCellValue, row.GetCell(Model).StringCellValue, 
        //    device.Name, device.AssetTag, device.SerialNumber, device.Status);

        return Result.Ok<Device>(device);
    }
}
