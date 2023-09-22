using Microsoft.EntityFrameworkCore;

using PhoneAssistant.WPF.Application;
using PhoneAssistant.WPF.Application.Entities;

namespace PhoneAssistant.WPF.Features.Sims;

public sealed class SimsRepository : ISimsRepository
{
    private readonly v1PhoneAssistantDbContext _dbContext;

    public SimsRepository(v1PhoneAssistantDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<v1Sim>> GetSimsAsync()
    {
        List<v1Sim> sims = await _dbContext.Sims.ToListAsync();
        return sims;
    }

}
