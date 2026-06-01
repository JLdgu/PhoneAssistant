using Microsoft.EntityFrameworkCore;
using ExcelDataReader;
using PhoneAssistant.Model;
using Serilog;
using System.CommandLine;

namespace PhoneAssistant.Cli;

internal static class Base
{
    internal static void Command(RootCommand rootCommand)
    {
        Command baseCommand = new("base", "Import EE base report");
        Option<FileInfo> baseFileOption = new("--file", "-f")
        {
            Description = "Full path to the EE base report file to import (*.xlsx)",
            Required = true,
            Validators =
            {
                result =>
                {
                    FileInfo? file = result.GetValueOrDefault<FileInfo>();
                    if (file == null || !file.Exists)
                    {
                        result.AddError("The specified file does not exist.");
                    }
                }
            }
        };

        baseCommand.Add(baseFileOption);
        baseCommand.SetAction(async parseResult =>
        {
            try
            {
                FileInfo baseFile = parseResult.GetValue(baseFileOption) ?? throw new ArgumentNullException(nameof(baseFileOption));
                Log.Information("Importing EE Base report");
                await ExecuteAsync(baseFile);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Unhandled exception:");
            }
        });

        rootCommand.Add(baseCommand);
    }

    internal static async Task ExecuteAsync(FileInfo baseFile)
    {
        Log.Information("Importing EE Base report from {0}", baseFile.FullName);

        using FileStream? stream = new(baseFile.FullName, FileMode.Open, FileAccess.Read);
        using var reader = ExcelReaderFactory.CreateReader(stream);

        // Convert the first sheet to DataTable
        var sheet = new System.Data.DataTable();
        var fieldCount = reader.FieldCount;

        for (int i = 0; i < fieldCount; i++)
        {
            sheet.Columns.Add($"Col{i}", typeof(object));
        }

        while (reader.Read())
        {
            var values = new object[fieldCount];
            reader.GetValues(values);
            sheet.Rows.Add(values);
        }

        if (sheet.Rows.Count == 0)
        {
            Log.Error("The Excel sheet is empty.");
            return;
        }

        var headerRow = sheet.Rows[0];
        var cell = headerRow[0]?.ToString()?.Trim();
        if (string.IsNullOrEmpty(cell) || cell != "Group")
        {
            Log.Error("Unable to find Group in cell A1, check you are importing the correct file.");
            return;
        }

        PhoneAssistantDbContext dbContext = ModelContext.Create();
        BaseReportRepository repository = new(dbContext);

        await repository.TruncateAsync();

        int added = 0;
        Progress progress = new();

        for (int i = 1; i < sheet.Rows.Count; i++)
        {
            var row = sheet.Rows[i];
            if (row == null) continue;
            if (row.ItemArray.Length == 4) break;

            var phoneNumber = row[11]?.ToString()?.Trim() ?? "";
            var userName = row[10]?.ToString()?.Trim() ?? "";
            var contractEndDate = row[15]?.ToString()?.Trim() ?? "";
            var talkPlan = row[6]?.ToString()?.Trim() ?? "";
            var handset = row[21]?.ToString()?.Trim() ?? "";
            var simNumber = row[17]?.ToString()?.Trim() ?? "";
            var lastUsedIMEI = row[18]?.ToString()?.Trim() ?? "";

            BaseReport item = new()
            {
                PhoneNumber = phoneNumber,
                UserName = userName,
                ContractEndDate = contractEndDate,
                TalkPlan = talkPlan,
                Handset = handset,
                SimNumber = simNumber,
                ConnectedIMEI = string.Empty,
                LastUsedIMEI = lastUsedIMEI
            };

            await repository.CreateAsync(item);

            progress.Draw(i, sheet.Rows.Count - 1);
            added++;
        }

        ImportHistoryRepository history = new(dbContext);
        _ = await history.CreateAsync(ImportType.BaseReport, baseFile.Name);

        Log.Information("Added {0} SIMs", added);
        Log.Information("Base Report imported successfully.");
    }    
}
