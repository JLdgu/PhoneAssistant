using PhoneAssistant.WPF.Application.Entities;

namespace PhoneAssistant.WPF.Features.Sims;
public interface ISimsRepository
{
    Task<IEnumerable<v1Sim>> GetSimsAsync();
}