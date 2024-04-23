using PhoneAssistant.WPF.Application.Entities;

namespace PhoneAssistant.WPF.Application.Repositories;
public interface ISimsRepository
{
    Task<string?> DeleteSIMAsync(string phoneNumber);
    Task<Sim?> GetSIMAsync(string phoneNumber);
    Task<IEnumerable<Sim>> GetSimsAsync();
}