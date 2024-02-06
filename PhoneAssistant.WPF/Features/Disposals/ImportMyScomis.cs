using CommunityToolkit.Mvvm.Messaging;

using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

using PhoneAssistant.WPF.Application.Entities;
using PhoneAssistant.WPF.Application.Repositories;

using System.IO;

namespace PhoneAssistant.WPF.Features.Disposals;
public sealed class ImportMyScomis(string importFile, 
                                   DisposalsRepository disposalsRepository,
                                   IMessenger messenger)
{
    private readonly DisposalsRepository _disposalsRepository = disposalsRepository;

    public async void Execute()
    {
        using FileStream? stream = new FileStream(importFile, FileMode.Open, FileAccess.Read);
        using XSSFWorkbook xssWorkbook = new XSSFWorkbook(stream);

        ISheet sheet = xssWorkbook.GetSheetAt(0);        
        messenger.Send(new LogMessage($"Found sheet {sheet.SheetName}"));
        messenger.Send(new LogMessage($"Processing {sheet.LastRowNum} rows"));

        IRow header = sheet.GetRow(0);
        ICell cell = header.GetCell(0);
        if (cell is null ||  cell.StringCellValue != "Category")
        {
            messenger.Send(new LogMessage("Unable to find Category is cell A1, check you are importing a myScomis export."));
            return;
        }
        int added = 0;
        int deleted = 0;
        int updated = 0;
        int unchanged = 0;

        for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
        {
            IRow row = sheet.GetRow(i);
            if (row == null) continue;

            string name = row.GetCell(3).StringCellValue;

            Disposal? disposal = await _disposalsRepository.GetDisposal(name);
            if (disposal is null)
            {
                disposal = new() { Imei = name, StatusDCC = row.GetCell(7).StringCellValue };                
                _disposalsRepository.Add(disposal);
                added++;
            }
            else
            {
                unchanged++;
            }
        }
        await _disposalsRepository.Save();
        messenger.Send(new LogMessage($"Added {added} disposals"));
        messenger.Send(new LogMessage($"Unchanged {unchanged} disposals"));
        messenger.Send(new LogMessage("Import complete"));
    }
}

public sealed record class LogMessage(string text);