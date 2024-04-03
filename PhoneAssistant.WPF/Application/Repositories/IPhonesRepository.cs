using PhoneAssistant.WPF.Application.Entities;

namespace PhoneAssistant.WPF.Application.Repositories;

public interface IPhonesRepository
{
    Task<Phone> Create(Phone phone);
    Task<bool> ExistsAsync(string imei);
    Task<IEnumerable<Phone>> GetActivePhonesAsync();
    Task<IEnumerable<Phone>> GetAllPhonesAsync();
    Task<Phone> RemoveSimFromPhone(Phone phone);
    Task<string> UpdateAsync(Phone phone);
    Task<string> UpdateKeyAsync(string oldImei, string newImei);
}