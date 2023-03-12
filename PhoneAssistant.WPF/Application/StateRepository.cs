using Microsoft.EntityFrameworkCore;

using PhoneAssistant.WPF.Application.Entities;
using PhoneAssistant.WPF.Features.Application;
using PhoneAssistant.WPF.Models;

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
        List<StateEntity> allStates = await _dbContext.States.ToListAsync();
        List<State> states = new List<State>();
        foreach (StateEntity entity in allStates)
        {
            states.Add(State.ToState(entity));
        }
        return states;
    }
}
