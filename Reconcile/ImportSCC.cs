using FluentResults;

using NPOI.SS.UserModel;

using Serilog;

namespace Reconcile;

public record Disposal(string PrimaryKey, string? SecondaryKey, int Certificate);

public sealed class ImportSCC(string importFile)
{
    private const string SheetName = "Units";
    private const string A2 = "A2";
    private const string CheckValue = "Units";
    public const int Account = 0;
    public const int TrackerId = 2;
    public const int SerialNumber = 3;
    public const int AssetNumber = 4;

    public Result<List<Disposal>> Execute()
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

            for (int i = sheet.FirstRowNum; i <= sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);
                Result<Disposal> disposal = GetDisposal(row);
                if (disposal.IsSuccess) 
                    disposals.Add(disposal.Value);
            }
            Log.Information("{0} devices found", disposals.Count);
            Log.Debug("First device {0}", disposals.First());
            Log.Debug("Last device {0}", disposals.Last());
        }
        catch (IOException)
        {
            return Result.Fail("Cannot access file, ensure it is closed and retry.");
        }

        return Result.Ok();
    }

    public static Result<Disposal> GetDisposal(IRow row)
    {
        if (row is null) return Result.Fail("Ignore: Null row");
        if (row.GetCell(Account) is null) return Result.Fail("Ignore: Account not D1024CT");
        if (row.GetCell(Account).CellType is not CellType.String) return Result.Fail("Ignore: Account not D1024CT");
        if (row.GetCell(Account).StringCellValue != "D1024CT") return Result.Fail("Ignore: Account not D1024CT");

        if (row.GetCell(SerialNumber).CellType is CellType.String
            && (row.GetCell(SerialNumber).StringCellValue == "NONE" || row.GetCell(SerialNumber).StringCellValue == "UNREADABLE")) return Result.Fail("Ignore: Unidentifiable");

        string primary;
        if (row.GetCell(SerialNumber).CellType is CellType.Numeric)
        {
            primary = row.GetCell(SerialNumber).NumericCellValue.ToString("000000000000000");
        }
        else
        {
            primary = row.GetCell(SerialNumber).StringCellValue;
            bool isNumeric = long.TryParse(primary, out _);
            if (isNumeric)
                primary = primary.PadLeft(15, '0');
        }
        string? secondary = null;
        if (row.GetCell(AssetNumber).CellType is CellType.String && row.GetCell(AssetNumber).StringCellValue != "NONE")
            secondary = row.GetCell(AssetNumber).StringCellValue;

        int certificate = (int)row.GetCell(TrackerId).NumericCellValue;

        return Result.Ok(new Disposal(primary, secondary, certificate));
    }
}
