using FluentResults;

using NPOI.SS.UserModel;
using NPOI.SS.Util;

namespace Reconcile;
public static class Import
{
    public static Result<ISheet> IsValidSheet(IWorkbook workbook, string sheetName, string cellValue, string cellReference)
    {
        CellReference cr = new (cellReference);        

        ISheet sheet = workbook.GetSheetAt(0);
        if (sheet.SheetName != sheetName) return Result.Fail<ISheet>($"First sheet in workbook not named {sheetName}");

        IRow header = sheet.GetRow(cr.Row);
        if (header is null) return Result.Fail<ISheet>("No rows found in sheet Data");

        return header.GetCell(cr.Col) is not ICell cell || cell.StringCellValue != cellValue
            ? Result.Fail<ISheet>($"Unable to find '{cellValue}' in cell {cellReference}")
            : Result.Ok<ISheet>(sheet);
    }
}
