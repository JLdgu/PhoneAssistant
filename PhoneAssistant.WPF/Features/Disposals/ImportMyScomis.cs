using CommunityToolkit.Mvvm.Messaging;

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
        using FileStream? stream = new FileStream(importFile, FileMode.Open, FileAccess.Read);
        using XSSFWorkbook xssWorkbook = new XSSFWorkbook(stream);

        ISheet sheet = xssWorkbook.GetSheetAt(0);
        messenger.Send(new LogMessage($"Found sheet {sheet.SheetName}"));
        messenger.Send(new LogMessage($"Processing {sheet.LastRowNum} rows"));

        IRow header = sheet.GetRow(0);
        ICell cell = header.GetCell(0);
        if (cell is null || cell.StringCellValue != "Category")
        {
            messenger.Send(new LogMessage("Unable to find Category is cell A1, check you are importing a myScomis export."));
            return;
        }
        int added = 0;
        int updated = 0;
        int unchanged = 0;

        Reconciliation reconciliation = new(disposalsRepository);

        for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
        {
            IRow row = sheet.GetRow(i);
            if (row == null) continue;

            string name = row.GetCell(3).StringCellValue;
            string ciStatus = row.GetCell(7).StringCellValue;

            Result result = await reconciliation.DisposalAsync(Import.DCC, name, ciStatus);
            switch (result)
            {
                case Result.Added:
                    added++; break;
                case Result.Updated:
                    updated++; break;
                case Result.Unchanged: 
                    unchanged++; break;
            }
        }
        messenger.Send(new LogMessage($"Added {added} disposals"));
        messenger.Send(new LogMessage($"Unchanged {unchanged} disposals"));
        messenger.Send(new LogMessage($"Updated {updated} disposals"));
        messenger.Send(new LogMessage("Import complete"));
    }
}
