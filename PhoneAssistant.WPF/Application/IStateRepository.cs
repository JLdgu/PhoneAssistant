using PhoneAssistant.WPF.Models;

namespace PhoneAssistant.WPF.Application;
public interface IStateRepository
{
    Task<IEnumerable<State>?> GetStatesAsync();
}