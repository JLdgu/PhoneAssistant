using PhoneAssistant.WPF.Application.Entities;

namespace PhoneAssistant.WPF.Application.Repositories;

public interface IPhonesRepository
{
    Task<IEnumerable<Phone>> GetPhonesAsync();
    Task<Phone> RemoveSimFromPhone(Phone phone);
    Task<string> UpdateAsync(Phone phone);
    Task<string> UpdateKeyAsync(string oldImei, string newImei);
}