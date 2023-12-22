using PhoneAssistant.WPF.Application.Entities;

namespace PhoneAssistant.WPF.Application.Repositories;
public interface IStateRepository
{
    Task<IEnumerable<State>?> GetStatesAsync();
}