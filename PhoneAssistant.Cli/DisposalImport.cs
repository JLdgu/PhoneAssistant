using ExcelDataReader;
using FluentResults;
using System.Data;

namespace PhoneAssistant.Cli;

public record ExcelSheetData(string SheetName, int RowCount, Dictionary<int, Dictionary<int, string>> Rows);

public static class DisposalImport
{
    public static Result<ExcelSheetData> IsValidSheet(string filePath, string sheetName, string cellValue, string cellReference)
    {
        try
        {
            // Parse cell reference (e.g., "A1" or "A2")
            if (cellReference.Length < 2)
                return Result.Fail<ExcelSheetData>($"Invalid cell reference: {cellReference}");

            string col = cellReference.TakeWhile(char.IsLetter).Aggregate("", (acc, c) => acc + c);
            string rowStr = cellReference.Substring(col.Length);

            if (!int.TryParse(rowStr, out int row) || row < 1)
                return Result.Fail<ExcelSheetData>($"Invalid cell reference: {cellReference}");

            // Convert column letters to zero-based index (A=0, B=1, etc.)
            int colIndex = 0;
            for (int i = 0; i < col.Length; i++)
            {
                colIndex = colIndex * 26 + (col[i] - 'A' + 1);
            }
            colIndex--; // Convert to zero-based

            using var stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
            using var reader = ExcelReaderFactory.CreateReader(stream);

            // Find the target sheet
            DataTable? sheet = null;
            do
            {
                if (reader.Name == sheetName)
                {
                    // Convert reader to DataTable
                    sheet = new DataTable(sheetName);
                    var fieldCount = reader.FieldCount;

                    for (int i = 0; i < fieldCount; i++)
                    {
                        sheet.Columns.Add($"Col{i}", typeof(object));
                    }

                    if (reader.Read())  // First row (header row)
                    {
                        var headerValues = new object[fieldCount];
                        reader.GetValues(headerValues);
                        sheet.Rows.Add(headerValues);
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
                return Result.Fail<ExcelSheetData>($"Sheet '{sheetName}' not found");

            if (sheet.Rows.Count < row)
                return Result.Fail<ExcelSheetData>("No rows found in sheet");

            var headerRow = sheet.Rows[row - 1]; // Convert to zero-based index
            if (colIndex >= headerRow.ItemArray.Length)
                return Result.Fail<ExcelSheetData>($"Column {col} not found in row {row}");

            var cellVal = headerRow[colIndex]?.ToString()?.Trim();
            if (cellVal != cellValue)
                return Result.Fail<ExcelSheetData>($"Unable to find '{cellValue}' in cell {cellReference}");

            // Convert DataTable to our format
            var rows = new Dictionary<int, Dictionary<int, string>>();
            for (int i = 0; i < sheet.Rows.Count; i++)
            {
                var rowData = new Dictionary<int, string>();
                for (int j = 0; j < sheet.Columns.Count; j++)
                {
                    rowData[j] = sheet.Rows[i][j]?.ToString() ?? "";
                }
                rows[i] = rowData;
            }

            return Result.Ok(new ExcelSheetData(sheetName, sheet.Rows.Count, rows));
        }
        catch (Exception ex)
        {
            return Result.Fail<ExcelSheetData>($"Error reading Excel file: {ex.Message}");
        }
    }
}
