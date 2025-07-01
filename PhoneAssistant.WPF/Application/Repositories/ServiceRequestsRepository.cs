namespace PhoneAssistant.Model;

public sealed class ServiceRequestsRepository : IServiceRequestsRepository
{
    private readonly PhoneAssistantDbContext _dbContext;

    public ServiceRequestsRepository(PhoneAssistantDbContext dbContext) => _dbContext = dbContext;
}