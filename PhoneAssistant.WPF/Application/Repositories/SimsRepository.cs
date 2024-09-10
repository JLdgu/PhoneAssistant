using Microsoft.EntityFrameworkCore;

using PhoneAssistant.Model;
using PhoneAssistant.WPF.Application.Entities;

namespace PhoneAssistant.WPF.Application.Repositories;

public sealed class SimsRepository : ISimsRepository
{
    private readonly PhoneAssistantDbContext _dbContext;

    public SimsRepository(PhoneAssistantDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task CreateAsync(Sim sim)
    {
        _dbContext.Sims.Add(sim);
        await _dbContext.SaveChangesAsync();

    }

    public async Task<IEnumerable<Sim>> GetSimsAsync()
    {
        List<Sim> sims = await _dbContext.Sims.ToListAsync();
        return sims;
    }
}
