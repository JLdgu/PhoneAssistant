﻿using CommunityToolkit.Mvvm.Messaging;

using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

using PhoneAssistant.WPF.Application.Repositories;

using System.IO;

namespace PhoneAssistant.WPF.Features.Disposals;

public sealed class ImportMyScomis(string importFile,
                                   IDisposalsRepository disposalsRepository,
                                   IMessenger messenger)
{
    public async Task Execute()
    {
        using FileStream? stream = new(importFile, FileMode.Open, FileAccess.Read);
        using XSSFWorkbook xssWorkbook = new(stream);

        ISheet sheet = xssWorkbook.GetSheetAt(0);
        messenger.Send(new LogMessage(MessageType.Default, $"Importing {importFile}"));
        messenger.Send(new LogMessage(MessageType.Default, $"Found sheet {sheet.SheetName}"));
        messenger.Send(new LogMessage(MessageType.MaxProgress, "", sheet.LastRowNum));

        IRow header = sheet.GetRow(0);
        ICell cell = header.GetCell(0);
        if (cell is null || cell.StringCellValue != "Category")
        {
            messenger.Send(new LogMessage(MessageType.Default, "Unable to find 'Category' in cell A1, check you are importing a myScomis export."));
            return;
        }
        int added = 0;
        int unchanged = 0;
        int updated = 0;
        TrackProgress progress = new(sheet.LastRowNum);

        await Task.Run(async delegate
        {
            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);
                if (row == null) continue;

                string imei = row.GetCell(3).StringCellValue;
                string status = row.GetCell(7).StringCellValue;

                Result result = await disposalsRepository.UpdateMSAsync(imei, status);
                switch (result)
                {
                    case Result.Added:
                        added++;
                        break;
                    case Result.Ignored:
                        messenger.Send(new LogMessage(MessageType.Default, $"Ignored row {row.RowNum + 1} IMEI ({imei}) not found"));
                        break;
                    case Result.Unchanged:
                        unchanged++;
                        break;
                    case Result.Updated:
                        updated++;
                        break;
                }
                if (progress.Milestone(i))
                {
                    messenger.Send(new LogMessage(MessageType.Progress, "", i));
                }
            }
        });

        messenger.Send(new LogMessage(MessageType.Default, $"Added {added} disposals"));
        messenger.Send(new LogMessage(MessageType.Default, $"Updated {updated} disposals"));
        messenger.Send(new LogMessage(MessageType.Default, $"Unchanged {unchanged} disposals"));
        messenger.Send(new LogMessage(MessageType.Default, "Import complete"));
    }
}
