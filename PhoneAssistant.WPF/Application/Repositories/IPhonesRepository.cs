using PhoneAssistant.WPF.Application.Entities;

namespace PhoneAssistant.WPF.Application.Repositories;

public interface IPhonesRepository
{
    Task<Phone> CreateAsync(Phone phone);
    Task<bool> ExistsAsync(string imei);
    Task<IEnumerable<Phone>> GetActivePhonesAsync();
    Task<IEnumerable<Phone>> GetAllPhonesAsync();
    Task RemoveSimFromPhoneAsync(Phone phone);
    Task UpdateAsync(Phone phone);
    //Task UpdateKeyAsync(string oldImei, string newImei);
}