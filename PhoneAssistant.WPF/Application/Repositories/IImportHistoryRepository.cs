using PhoneAssistant.WPF.Application.Entities;

namespace PhoneAssistant.WPF.Application.Repositories;
public interface IImportHistoryRepository
{
    Task<ImportHistory> CreateAsync(ImportType importType, string file);
    Task<ImportHistory?> GetLatestImportAsync(ImportType importType);
}