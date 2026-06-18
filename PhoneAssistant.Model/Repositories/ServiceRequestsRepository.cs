namespace PhoneAssistant.Model;

public interface IServiceRequestsRepository
{
}

public sealed class ServiceRequestsRepository(PhoneAssistantDbContext dbContext) : IServiceRequestsRepository
{
    private readonly PhoneAssistantDbContext _dbContext = dbContext;
}