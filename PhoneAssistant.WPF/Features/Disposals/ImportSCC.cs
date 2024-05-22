using System.IO;

using CommunityToolkit.Mvvm.Messaging;

using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

using PhoneAssistant.WPF.Application.Entities;
using PhoneAssistant.WPF.Application.Repositories;

namespace PhoneAssistant.WPF.Features.Disposals;

public sealed class ImportSCC(string importFile,
                              IDisposalsRepository disposalsRepository,
                              IMessenger messenger)
{
    public async Task Execute()
    {
        using FileStream? stream = new FileStream(importFile, FileMode.Open, FileAccess.Read);
        try
        {
            using HSSFWorkbook workbook = new HSSFWorkbook(stream);

            ISheet sheet = workbook.GetSheetAt(0);
            messenger.Send(new LogMessage(MessageType.Default, $"Importing {importFile}"));
            messenger.Send(new LogMessage(MessageType.Default, $"Found sheet {sheet.SheetName}"));
            messenger.Send(new LogMessage(MessageType.Default, $"Processing {sheet.LastRowNum} rows"));

            IRow header = sheet.GetRow(1);
            ICell cell = header.GetCell(0);
            if (cell is null || cell.StringCellValue != "Units")
            {
                messenger.Send(new LogMessage(MessageType.Default, "Unable to find 'Units' in cell A2, check you are importing a SCC export."));
                return;
            }
            int added = 0;
            int unchanged = 0;
            int updated = 0;

            for (int i = (sheet.FirstRowNum + 4); i <= sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);
                if (row == null) continue;

                if (row.GetCell(3).CellType != CellType.Numeric)
                {
                    messenger.Send(new LogMessage(MessageType.Default, $"Ignored row {i} Serial Number is not numeric."));
                    continue;
                }
                string imei = row.GetCell(3).NumericCellValue.ToString();

                string status = row.GetCell(8).StringCellValue.ToLower();
                if (status.Contains("despatched"))
                    status = "Disposed";

                int? certificate = null;
                if (row.GetCell(2).CellType == CellType.Numeric)
                {
                    certificate = (int?)row.GetCell(2).NumericCellValue;
                }

                Result result = await disposalsRepository.AddOrUpdateSCCAsync(imei, status, certificate);
                switch (result)
                {
                    case Result.Added:
                        added++;
                        break;
                    case Result.Unchanged:
                        unchanged++;
                        break;
                    case Result.Updated:
                        updated++;
                        break;
                }
            }
            messenger.Send(new LogMessage(MessageType.Default, $"Added {added} disposals"));
            messenger.Send(new LogMessage(MessageType.Default, $"Updated {updated} disposals"));
            messenger.Send(new LogMessage(MessageType.Default, $"Unchanged {unchanged} disposals"));
            messenger.Send(new LogMessage(MessageType.Default, "Import complete"));
        }
        catch (IOException ex)
        {
            if (ex.Message.StartsWith("Duplicate"))
            {
                messenger.Send(new LogMessage(MessageType.Default, $"Cannot read SCC spreadsheet"));
                messenger.Send(new LogMessage(MessageType.Default, $"Try opening and saving a copy"));
            }
            else
                throw;
        }
    }
}
