using FluentResults;

using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

using Serilog;

namespace Reconcile;

public sealed class ImportMS(ReconcileDbContext dbContext, string importFile)
{
    public Result<bool> Execute()
    {
        using FileStream stream = new(importFile, FileMode.Open, FileAccess.Read);
        using XSSFWorkbook workbook = new(stream);

        Result<ISheet> result = GetDataSheet(workbook);
        if (result.IsFailed)
        {
            Log.Error(result.Errors.First().Message);
            return false;
        }
        ISheet sheet = result.Value;
        Log.Information("Processing {0} rows", sheet.LastRowNum - sheet.FirstRowNum);

        for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
        {
            IRow row = sheet.GetRow(i);
            Result<Device> device = GetDevice(row);

            if (i > 10) return true;
        }

        return true;
    }

    public static Result<ISheet> GetDataSheet(XSSFWorkbook xssWorkbook)
    {
        ISheet sheet = xssWorkbook.GetSheetAt(0);
        if (sheet.SheetName is not "Data") return Result.Fail<ISheet>("Unable to find sheet named Data in workbook");
        Log.Debug("Sheet named Data found in workbook");
        
        IRow header = sheet.GetRow(0);
        if (header is null) return Result.Fail<ISheet>("No rows found in sheet Data");

        Result<string> cat = IsValidColumn(header, 0, "Category");
        return cat.IsFailed ? Result.Fail<ISheet>(cat.Errors) : Result.Ok<ISheet>(sheet);
    }

    private static Result<string> IsValidColumn(IRow header, int cellnum, string columnName)
    {
        if (header.GetCell(cellnum) is not ICell cell || cell.StringCellValue != columnName) return  Result.Fail<string>($"Unable to find '{columnName}' column, check you are using a myScomis export");

        return Result.Ok<string>($"Cell {cell.Address.FormatAsString()} is {columnName}");
    }

    public static Result<Device> GetDevice(IRow row)
    {
        if (row.GetCell(1).StringCellValue != "Computer" && row.GetCell(1).StringCellValue != "Phone") return Result.Fail<Device>("Invalid Item Type");

        if (row.GetCell(1).StringCellValue == "Computer" && row.GetCell(8).StringCellValue != "Apple") return Result.Fail<Device>("Invalid Manufacturer");

        return Result.Ok<Device>(
            new Device(
                row.GetCell(3).StringCellValue, 
                row.GetCell(4).StringCellValue,
                row.GetCell(10).StringCellValue,
                row.GetCell(7).StringCellValue
                ));
    }

}
