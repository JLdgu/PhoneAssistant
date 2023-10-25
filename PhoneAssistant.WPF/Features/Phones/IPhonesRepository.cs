using PhoneAssistant.WPF.Application.Entities;

namespace PhoneAssistant.WPF.Features.Phones;

public interface IPhonesRepository
{
    Task<IEnumerable<v1Phone>> GetPhonesAsync();
    Task<string> RemoveSimFromPhone(v1Phone phone);
    Task<string> UpdateAsync(v1Phone phone);
    Task<string> UpdateKeyAsync(string oldImei, string newImei);
}