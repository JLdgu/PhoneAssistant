using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

using System.Data;
using System.IO;

namespace PhoneAssistant.WPF.Features.Disposals;
public class ExportMyScomis
{
    public static void WriteExcel()
    {
        var a = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        using var fs = new FileStream(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "myScomisImport.xlsx"), FileMode.Create, FileAccess.Write);
        IWorkbook workbook = new XSSFWorkbook();
        ISheet excelSheet = workbook.CreateSheet("Sheet1");

        List<String> columns = new List<string>();
        IRow row = excelSheet.CreateRow(0);
        int columnIndex = 0;

        columns.Add("Name");
        row.CreateCell(columnIndex).SetCellValue("Name");
        columnIndex++;

        columns.Add("Status");
        row.CreateCell(columnIndex).SetCellValue("Status");
        columnIndex++;

        int rowIndex = 1;
        row = excelSheet.CreateRow(rowIndex);
        int cellIndex = 0;
        row.CreateCell(cellIndex).SetCellValue("123456789012347");
        cellIndex++;
        row.CreateCell(cellIndex).SetCellValue("Disposed");

        rowIndex++;
        workbook.Write(fs);
    }
}
