using PhoneAssistant.WPF.Application.Entities;

namespace PhoneAssistant.WPF.Models;
public sealed class State
{
    public string Status { get; init; }
    
    public static State ToState(StateEntity entity)
    {
        return new State()
        {
            Status = entity.Status
        };
    }

    public static implicit operator StateEntity(State state)
    {
        return new StateEntity
        {
            Status = state.Status
        };
    }
}
