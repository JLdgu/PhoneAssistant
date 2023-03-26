using PhoneAssistant.WPF.Application.Entities;

namespace PhoneAssistant.WPF.Application;
public interface IStateRepository
{
    Task<IEnumerable<State>?> GetStatesAsync();
}