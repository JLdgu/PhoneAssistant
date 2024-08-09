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
    public const int TrackerId = 2; 
    public const int SerialNumber = 3;
    public const int Status = 8;
    public const int Manufacturer = 12;
    public const int Model = 13;


    public async Task Execute()
    {
        using FileStream? stream = new(importFile, FileMode.Open, FileAccess.Read);
        using HSSFWorkbook workbook = new(stream);

        ISheet sheet = workbook.GetSheetAt(0);
        messenger.Send(new LogMessage(MessageType.Default, $"Importing {importFile}"));
        messenger.Send(new LogMessage(MessageType.Default, $"Found sheet {sheet.SheetName}"));
        messenger.Send(new LogMessage(MessageType.MaxProgress, "", sheet.LastRowNum));
        IRow header = sheet.GetRow(1);
        ICell cell = header.GetCell(0);
        if (cell is null || cell.StringCellValue != "Units")
        {
            messenger.Send(new LogMessage(MessageType.Default, "Unable to find 'Units' in cell A2, check you are importing a SCC export."));
            return;
        }
        int added = 0;
        TrackProgress progress = new(sheet.LastRowNum);

        await Task.Run(async delegate
        {
            for (int i = (sheet.FirstRowNum + 4); i <= sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);
                Disposal disposal = await ParseRowAsync(row);
                if (disposal.Action == ReconciliationConstants.ImeiInvalid)
                {
                    messenger.Send(new LogMessage(MessageType.Default, $"Ignored row {row.RowNum} Serial Number is invalid"));
                    continue;
                }

                await disposalsRepository.AddAsync(disposal);
                added++;

                if (progress.Milestone(added))
                {
                    messenger.Send(new LogMessage(MessageType.Progress, "", added));
                }
            }
        });

        messenger.Send(new LogMessage(MessageType.Default, $"Added {added} disposals"));
        messenger.Send(new LogMessage(MessageType.Default, "Import complete"));
    }

    public async Task<Disposal> ParseRowAsync(IRow row)
    {
        Disposal disposal = new() { Imei = string.Empty, Manufacturer = string.Empty, Model = string.Empty, TrackedSKU = false, Action = ReconciliationConstants.ImeiInvalid };

        if (row == null) return disposal;

        if (row.GetCell(2).CellType == CellType.Numeric)
        {
            disposal.Certificate = (int)row.GetCell(2).NumericCellValue;
        }

        switch (row.GetCell(SerialNumber).CellType)
        {
            case CellType.Numeric:
                {
                    disposal.Imei = row.GetCell(SerialNumber).NumericCellValue.ToString("000000000000000");
                    break;
                }
            case CellType.String:
                {
                    disposal.Imei = row.GetCell(SerialNumber).StringCellValue;
                    bool isNumeric = long.TryParse(disposal.Imei, out _);
                    if (isNumeric)
                        disposal.Imei = disposal.Imei.PadLeft(15, '0');
                    else
                    {
                        disposal.Action = ReconciliationConstants.ImeiInvalid;
                        return disposal;
                    }
                    break;
                }
            default:
                {
                    disposal.Imei = row.GetCell(SerialNumber).CellFormula;
                    disposal.Action = ReconciliationConstants.ImeiInvalid; 
                    return disposal;                    
                }
        }

        if (!row.GetCell(Status).StringCellValue.Contains("despatched", StringComparison.InvariantCultureIgnoreCase))
        {
            disposal.Action = ReconciliationConstants.CheckSCCStatus;
            return disposal;
        }

        disposal.Manufacturer = row.GetCell(Manufacturer).StringCellValue;
        if (row.GetCell(Model).CellType == CellType.Numeric)
        {
            disposal.Model = row.GetCell(Model).NumericCellValue == disposal.Certificate ? "Unknown" : row.GetCell(Model).NumericCellValue.ToString();
        }
        else
        {
            disposal.Model = row.GetCell(Model).StringCellValue;
        }

        disposal.Action = ReconciliationConstants.UnknownSKU;
        StockKeepingUnit? sku = await disposalsRepository.GetSKUAsync(disposal.Manufacturer, disposal.Model);
        if (sku is not null)
        {
            disposal.Manufacturer = sku.Manufacturer;
            disposal.Model = sku.Model;
            disposal.TrackedSKU = true;
            disposal.Action = null;
        }

        return disposal;

    }
}
