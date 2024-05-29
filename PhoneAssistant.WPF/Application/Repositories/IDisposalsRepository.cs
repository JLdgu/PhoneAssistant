using Microsoft.EntityFrameworkCore;

using PhoneAssistant.WPF.Application.Entities;

namespace PhoneAssistant.WPF.Application.Repositories;
public interface IDisposalsRepository
{
    Task AddAsync(Disposal disposal);
    Task<Result> AddOrUpdateMSAsync(string imei, string status);
    Task<Result> AddOrUpdatePAAsync(string imei, string status, int? SR);
    Task<Result> AddOrUpdateSCCAsync(string imei, string status, int? certificate);
    Task<IEnumerable<Disposal>> GetAllDisposalsAsync();
    Task<Disposal?> GetDisposalAsync(string imei);
    Task TruncateAsync();
    Task UpdateAsync(Disposal disposal);
}