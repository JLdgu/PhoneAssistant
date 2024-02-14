using PhoneAssistant.WPF.Application.Entities;

namespace PhoneAssistant.WPF.Application.Repositories;
public interface IDisposalsRepository
{
    Task AddAsync(Disposal disposal);
    Task<Disposal?> GetDisposalAsync(string imei);
    Task UpdateAsync(Disposal disposal);
}