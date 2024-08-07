using System.Diagnostics.Eventing.Reader;
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
                if (row == null) continue;

                string imei;
                if (row.GetCell(3).CellType == CellType.Numeric)
                {
                    imei = row.GetCell(3).NumericCellValue.ToString("000000000000000");
                }
                else
                {
                    if (row.GetCell(3).CellType == CellType.String)
                    {
                        imei = row.GetCell(3).StringCellValue.PadLeft(15, '0');

                        bool isNumeric = long.TryParse(imei, out _);
                        if (!isNumeric)
                        {
                            messenger.Send(new LogMessage(MessageType.Default, $"Ignored row {i} Serial Number is not numeric."));
                            continue;
                        }
                    }
                    else
                    {
                        messenger.Send(new LogMessage(MessageType.Default, $"Ignored row {i} Serial Number is not numeric."));
                        continue;
                    }
                }

                string status = row.GetCell(8).StringCellValue;
                if (status.Contains("despatched", StringComparison.InvariantCultureIgnoreCase))
                    status = "Disposed";

                string manufacturer = row.GetCell(12).StringCellValue;
                string model = row.GetCell(13).CellType == CellType.Numeric ? row.GetCell(2).NumericCellValue.ToString() : row.GetCell(13).StringCellValue;
                bool tracked = false;
                string? action = ReconciliationConstants.UnknownSKU;

                StockKeepingUnit? sku = await disposalsRepository.GetSKUAsync(manufacturer, model);
                if (sku is not null)
                { 
                    manufacturer = sku.Manufacturer;
                    model = sku.Model;
                    tracked = true;
                    action = null;
                }

                int certificate = 0;
                if (row.GetCell(2).CellType == CellType.Numeric)
                {
                    certificate = (int)row.GetCell(2).NumericCellValue;
                }

                Disposal disposal = new() { Imei = imei, Manufacturer = manufacturer, Model = model, TrackedSKU = tracked, Certificate = certificate, Action = action};
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
        //}
        //catch (IOException ex)
        //{
        //    if (ex.Message.StartsWith("Duplicate"))
        //    {
        //        messenger.Send(new LogMessage(MessageType.Default, $"Cannot read SCC spreadsheet"));
        //        messenger.Send(new LogMessage(MessageType.Default, $"Try opening and saving a copy"));
        //    }
        //    else
        //        throw;
        //}
    }
}
