namespace PhoneAssistant.Model;

public interface IImportHistoryRepository
{
    Task<ImportHistory> CreateAsync(ImportType importType, string file);
    Task<ImportHistory?> GetLatestImportAsync(ImportType importType);
}