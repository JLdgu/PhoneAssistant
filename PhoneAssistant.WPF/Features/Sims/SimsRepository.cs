using PhoneAssistant.WPF.Application.Entities;

namespace PhoneAssistant.WPF.Features.Sims;

public sealed class SimsRepository : ISimsRepository
{
    public async Task<IEnumerable<Sim>?> GetSimsAsync()
    {
        await Task.CompletedTask;
        return AllSimCards();
    }

    private static IEnumerable<Sim> AllSimCards()
    {
        return new List<Sim>()
        {
            new Sim () {Id = 11, PhoneNumber = "07967691765", SimNumber = "8944125605559639023"},
            new Sim () {Id = 21, PhoneNumber = "07967691764", SimNumber = "8944125605559639133"},
            new Sim () {Id = 31, PhoneNumber = "07967691763", SimNumber = "8944125605559639323"},
            new Sim () {Id = 41, PhoneNumber = "07967691762", SimNumber = "8944125605559639453"},
            new Sim () {Id = 51, PhoneNumber = "07967691761", SimNumber = "8944125605559639893"},
            new Sim () {Id = 61, PhoneNumber = "07967691760", SimNumber = "8944125605559639999"}
        };
    }    
}
