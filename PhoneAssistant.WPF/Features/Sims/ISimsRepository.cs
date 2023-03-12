using PhoneAssistant.WPF.Models;

namespace PhoneAssistant.WPF.Features.Sims;
public interface ISimsRepository
{
    Task<IEnumerable<Sim>?> GetSimsAsync();
}