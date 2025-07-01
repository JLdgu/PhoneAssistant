using PhoneAssistant.Model;

namespace PhoneAssistant.WPF.Application.Repositories;
public interface IImportHistoryRepository
{
    Task<ImportHistory> CreateAsync(ImportType importType, string file);
    Task<ImportHistory?> GetLatestImportAsync(ImportType importType);
}