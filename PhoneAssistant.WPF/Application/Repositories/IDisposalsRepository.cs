using Microsoft.EntityFrameworkCore;

using PhoneAssistant.WPF.Application.Entities;

namespace PhoneAssistant.WPF.Application.Repositories;
public interface IDisposalsRepository
{
    Task AddAsync(Disposal disposal);
    Task<IEnumerable<Disposal>> GetAllDisposalsAsync();
    Task<Disposal?> GetDisposalAsync(string imei);
    Task<StockKeepingUnit?> GetSKUAsync(string manufacturer, string model);
    Task TruncateAsync();
    Task UpdateAsync(Disposal disposal);
    Task<Result> UpdateMSAsync(string imei, string status);
    Task<Result> UpdatePAAsync(string imei, string status, int SR);
}