using PhoneAssistant.WPF.Application.Entities;

namespace PhoneAssistant.WPF.Features.ServiceRequests;
public interface IServiceRequestsRepository
{
    Task<IEnumerable<ServiceRequest>?> GetServiceRequestsAsync();
}