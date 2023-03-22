using PhoneAssistant.WPF.Application.Entities;

namespace PhoneAssistant.WPF.Models;
public sealed class State
{
    public string Status { get; init; }

    public State(string status)
    {
        Status = status;
    }
    
    public static State ToState(StateEntity entity) => new State(entity.Status);    

    public static implicit operator StateEntity(State state) => new StateEntity(state.Status);
}
