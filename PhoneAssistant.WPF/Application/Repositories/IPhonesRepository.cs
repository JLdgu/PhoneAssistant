using PhoneAssistant.WPF.Application.Entities;

namespace PhoneAssistant.WPF.Application.Repositories;

public interface IPhonesRepository
{
    Task<IEnumerable<v1Phone>> GetPhonesAsync();
    Task<v1Phone> RemoveSimFromPhone(v1Phone phone);
    Task<string> UpdateAsync(v1Phone phone);
    Task<string> UpdateKeyAsync(string oldImei, string newImei);
}