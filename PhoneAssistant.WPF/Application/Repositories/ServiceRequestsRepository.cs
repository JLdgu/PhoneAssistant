using Microsoft.EntityFrameworkCore;

using PhoneAssistant.WPF.Application.Entities;
using PhoneAssistant.WPF.Application;
using PhoneAssistant.Model;

namespace PhoneAssistant.WPF.Application.Repositories;

public sealed class ServiceRequestsRepository : IServiceRequestsRepository
{
    private readonly PhoneAssistantDbContext _dbContext;

    public ServiceRequestsRepository(PhoneAssistantDbContext dbContext)
    {
        _dbContext = dbContext;
    }
}