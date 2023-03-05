using PhoneAssistant.WPF.Models;

namespace PhoneAssistant.WPF.Features.Phones;
public interface IPhonesRepository
{
    Task<IEnumerable<Phone>> AllAsync();
    Task<IEnumerable<Phone>> SearchAsync(string search);
    void SaveChanges(Phone changedPhone);
}