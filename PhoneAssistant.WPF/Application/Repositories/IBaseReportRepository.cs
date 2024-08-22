using PhoneAssistant.WPF.Application.Entities;

namespace PhoneAssistant.WPF.Application.Repositories;
public interface IBaseReportRepository
{
    Task CreateAsync(BaseReport report);
    Task<IEnumerable<BaseReport>> GetBaseReportAsync();
    Task<string?> GetSimNumberAsync(string phoneNumber);
    Task TruncateAsync();
}