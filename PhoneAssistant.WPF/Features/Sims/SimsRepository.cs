using PhoneAssistant.WPF.Models;

namespace PhoneAssistant.WPF.Features.Sims;

public sealed class SimsRepository
{
    public async Task<IEnumerable<Sim>?> AllAsync()
    {
        await Task.CompletedTask;
        return AllSimCards();
    }

    private static IEnumerable<Sim> AllSimCards()
    {
        return new List<Sim>()
        {
            new Sim () { PhoneNumber = "07967691765", SimNumber = "8944125605559639023", Status="In Stock", Note=null},
            new Sim () { PhoneNumber = "07967691764", SimNumber = "8944125605559639133", Status="Production", Note="Replacement" },
            new Sim () { PhoneNumber = "07967691763" ,SimNumber = "8944125605559639323", Status="In Stock", Note=null },
            new Sim () { PhoneNumber = "07967691762" ,SimNumber = "8944125605559639453", Status="In Stock", Note=null },
            new Sim () { PhoneNumber = "07967691761" ,SimNumber = "8944125605559639893", Status="In Stock", Note=null },
            new Sim () { PhoneNumber = "07967691760" ,SimNumber = "8944125605559639999", Status="Production", Note="Replacement" }
        };
    }

}
