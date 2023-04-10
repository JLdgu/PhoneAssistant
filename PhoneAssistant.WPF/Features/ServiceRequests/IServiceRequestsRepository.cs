using PhoneAssistant.WPF.Application.Entities;

namespace PhoneAssistant.WPF.Features.ServiceRequests;
public interface IServiceRequestsRepository
{
    Task AddAsync(ServiceRequest newSR);

    Task<IEnumerable<ServiceRequest>?> GetServiceRequestsAsync();

    Task UpdateAsync(ServiceRequest srToUpdate);
}