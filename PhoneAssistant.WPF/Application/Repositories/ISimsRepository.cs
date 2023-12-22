using PhoneAssistant.WPF.Application.Entities;

namespace PhoneAssistant.WPF.Application.Repositories;
public interface ISimsRepository
{
    Task<IEnumerable<Sim>> GetSimsAsync();
}