using Microsoft.EntityFrameworkCore;

using PhoneAssistant.WPF.Application.Entities;
using PhoneAssistant.WPF.Application;

namespace PhoneAssistant.WPF.Application.Repositories;

public sealed class ServiceRequestsRepository : IServiceRequestsRepository
{
    private readonly PhoneAssistantDbContext _dbContext;

    public ServiceRequestsRepository(PhoneAssistantDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task AddAsync(ServiceRequest newSR)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<ServiceRequest>?> GetServiceRequestsAsync()
    {
        List<ServiceRequest> serviceRequests = await _dbContext.ServiceRequests.ToListAsync();
        return serviceRequests;
    }

    public Task UpdateAsync(ServiceRequest srToUpdate)
    {
        throw new NotImplementedException();
    }
}