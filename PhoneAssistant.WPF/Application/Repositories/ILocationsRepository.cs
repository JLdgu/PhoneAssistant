using PhoneAssistant.WPF.Application.Entities;

namespace PhoneAssistant.WPF.Application.Repositories;
public interface ILocationsRepository
{
    Task<IEnumerable<Location>> GetAllLocationsAsync();
}