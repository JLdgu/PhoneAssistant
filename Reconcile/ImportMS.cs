using FluentResults;

using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

using Serilog;

namespace Reconcile;

public sealed class ImportMS(string importFile) //ReconcileDbContext dbContext,
{
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
        List<Device> _devices = [];

        using FileStream stream = new(importFile, FileMode.Open, FileAccess.Read);
        using XSSFWorkbook workbook = new(stream);

        Result<ISheet> resultSheet = GetDataSheet(workbook);
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
                _devices.Add(resultDevice.Value);                
            }
        }
        Log.Information("{0} devices found",_devices.Count);

        return Result.Ok<List<Device>>(_devices);
    }

    public static Result<ISheet> GetDataSheet(XSSFWorkbook xssWorkbook)
    {
        ISheet sheet = xssWorkbook.GetSheetAt(0);
        if (sheet.SheetName is not "Data") return Result.Fail<ISheet>("Unable to find sheet named Data in workbook");
        Log.Debug("Sheet named Data found in workbook");
        
        IRow header = sheet.GetRow(0);
        if (header is null) return Result.Fail<ISheet>("No rows found in sheet Data");

        Result<string> cat = IsValidColumn(header, Category, "Category");
        return cat.IsFailed ? Result.Fail<ISheet>(cat.Errors) : Result.Ok<ISheet>(sheet);
    }

    private static Result<string> IsValidColumn(IRow header, int cellnum, string columnName)
    {
        if (header.GetCell(cellnum) is not ICell cell || cell.StringCellValue != columnName) return  Result.Fail<string>($"Unable to find '{columnName}' column, check you are using a myScomis export");

        return Result.Ok<string>($"Cell {cell.Address.FormatAsString()} is {columnName}");
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
                name: row.GetCell(Name).StringCellValue,
                assetTag: row.GetCell(AssetTag).StringCellValue,
                serialNumber: row.GetCell(SerialNumber).StringCellValue,
                status: row.GetCell(Status).StringCellValue
                );

        //Log.Debug("Item Type: {0} | OEM: {1} | Model: {2} | Name: {3} | Asset Tag: {4} | Serial Number: {5} | Status: {6}",
        //    row.GetCell(ItemType).StringCellValue, row.GetCell(OEM).StringCellValue, row.GetCell(Model).StringCellValue, 
        //    device.Name, device.AssetTag, device.SerialNumber, device.Status);

        return Result.Ok<Device>(device);
    }
}
