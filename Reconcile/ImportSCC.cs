using FluentResults;

using NPOI.SS.UserModel;

using Serilog;

namespace Reconcile;

public record Disposal(string Name, int Certificate);

public sealed class ImportSCC(string importFile, List<Device> devices)
{
    private const string SheetName = "Units";
    private const string A2 = "A2";
    private const string CheckValue = "Units";
    const int TrackerId = 2;
    const int SerialNumber = 3;
    const int Status = 8;
    const int Manufacturer = 12;
    const int Model = 13;

    public Result Execute()
    {
        Log.Information("Importing {0}", importFile);

        List<Disposal> disposals = [];
        try
        {
            using FileStream stream = new(importFile, FileMode.Open, FileAccess.Read);
            using IWorkbook workbook = WorkbookFactory.Create(stream, readOnly: true);

            Result<ISheet> resultSheet = Import.IsValidSheet(workbook, SheetName, CheckValue, A2);
            if (resultSheet.IsFailed) return Result.Fail(resultSheet.Errors);

            ISheet sheet = resultSheet.Value;
            Log.Information("Processing {0} rows", sheet.LastRowNum - 3);

            for (int i = (sheet.FirstRowNum + 3); i <= sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);
                if (row is null) continue;
                //if (row.GetCell(0) is not null && row.GetCell(0).CellType == CellType.String && row.GetCell(0).StringCellValue == "D1024CT")
                    //disposals.Add();
            }
            Log.Information("{0} devices found", devices.Count);
            Log.Debug("First device {0}", devices.First());
            Log.Debug("Last device {0}", devices.Last());
        }
        catch (IOException)
        {
            return Result.Fail("Cannot access file, ensure it is closed and retry.");
        }

        return Result.Ok();
    }

    public static Result<Disposal> GetDisposal(IRow row)
    {
        int certificate = 0;
        if (row.GetCell(TrackerId).CellType == CellType.Numeric)
            certificate = (int)row.GetCell(TrackerId).NumericCellValue;

        string imei;
        switch (row.GetCell(SerialNumber).CellType)
        {
            case CellType.Numeric:
                {
                    imei = row.GetCell(SerialNumber).NumericCellValue.ToString("000000000000000");
                    break;
                }
            case CellType.String:
                {
                    imei = row.GetCell(SerialNumber).StringCellValue;
                    bool isNumeric = long.TryParse(imei, out _);
                    if (isNumeric)
                        imei = imei.PadLeft(15, '0');
                    break;
                }
            default:
                {
                    imei = row.GetCell(SerialNumber).CellFormula;
                    break;
                }
        }

        return Result.Ok();
    }
}
