using Microsoft.EntityFrameworkCore;

using PhoneAssistant.WPF.Application.Entities;
using PhoneAssistant.WPF.Application;

namespace PhoneAssistant.WPF.Application;
public sealed class StateRepository : IStateRepository
{
    private readonly PhoneAssistantDbContext _dbContext;

    public StateRepository(PhoneAssistantDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<State>?> GetStatesAsync()
    {
        List<State> states = await _dbContext.States.ToListAsync();
        return states;
    }
}
