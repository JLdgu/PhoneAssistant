﻿using Microsoft.EntityFrameworkCore;

using PhoneAssistant.WPF.Application.Entities;
using PhoneAssistant.WPF.Features.Application;

namespace PhoneAssistant.WPF.Features.ServiceRequests;

public sealed class ServiceRequestsRepository : IServiceRequestsRepository
{
    private readonly PhoneAssistantDbContext _dbContext;

    public ServiceRequestsRepository(PhoneAssistantDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<ServiceRequest>?> GetServiceRequestsAsync()
    {
        List<ServiceRequest> serviceRequests = await _dbContext.ServiceRequests.ToListAsync();
        return serviceRequests;
    }
}