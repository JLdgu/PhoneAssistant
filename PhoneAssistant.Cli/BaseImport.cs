using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using NPOI.SS.UserModel;

using PhoneAssistant.Model;

using Serilog;

namespace PhoneAssistant.Cli;

public static class BaseImport
{
    public static async Task ExecuteAsync(FileInfo baseFile)
    {
        ApplicationSettingsRepository settingsRepository = new();
        Log.Information("Applying update to {0}", settingsRepository.ApplicationSettings.Database);
        Log.Information("Importing EE Base report from {0}", baseFile.FullName);

        using FileStream? stream = new(baseFile.FullName, FileMode.Open, FileAccess.Read);
        using IWorkbook workbook = WorkbookFactory.Create(stream, readOnly: true);
        ISheet sheet = workbook.GetSheetAt(0);
        IRow header = sheet.GetRow(0);
        ICell cell = header.GetCell(0);
        if (cell is null || cell.StringCellValue != "Group")
        {
            Log.Error("Unable to find Group in cell A1, check you are importing the correct file.");
            return;
        }

        string connectionString = $@"DataSource={settingsRepository.ApplicationSettings.Database};";
        var options = new DbContextOptionsBuilder<PhoneAssistantDbContext>()
            .UseSqlite(connectionString)
            .Options;
        PhoneAssistantDbContext dbContext = new(options);
        BaseReportRepository _repository = new(dbContext);

        await _repository.TruncateAsync();

        int added = 0;
        Progress progress = new();

        for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
        {
            IRow row = sheet.GetRow(i);
            if (row == null) continue;
            if (row.Cells.Count == 4) break;

            _ = row.GetCell(11).DateCellValue.ToString() ?? string.Empty;

            var PhoneNumber = row.GetCell(6).StringCellValue;
            var UserName = row.GetCell(5).StringCellValue;
            var ContractEndDate = row.GetCell(15).DateCellValue.ToString() ?? string.Empty;
            var TalkPlan = row.GetCell(8).StringCellValue.ToString();
            var Handset = row.GetCell(21).StringCellValue;
            var SimNumber = row.GetCell(17).StringCellValue;
            var ConnectedIMEI = string.Empty;
            var LastUsedIMEI = row.GetCell(18).StringCellValue;

            BaseReport item = new()
            {
                PhoneNumber = row.GetCell(6).StringCellValue,
                UserName = row.GetCell(5).StringCellValue,
                ContractEndDate = row.GetCell(15).DateCellValue.ToString() ?? string.Empty,
                TalkPlan = TalkPlan = row.GetCell(8).StringCellValue.ToString(),
                Handset = row.GetCell(21).StringCellValue,
                SimNumber = row.GetCell(17).StringCellValue,
                ConnectedIMEI = string.Empty,
                LastUsedIMEI = row.GetCell(18).StringCellValue
            };

            await _repository.CreateAsync(item);

            progress.Draw(i, sheet.LastRowNum);
            added++;
        }

        ImportHistoryRepository history = new(dbContext);
        _ = await history.CreateAsync(ImportType.BaseReport, baseFile.Name);

        Log.Information("Added {0} SIMs",added);
        Log.Information("Base Report imported successfully.");
    }

}
