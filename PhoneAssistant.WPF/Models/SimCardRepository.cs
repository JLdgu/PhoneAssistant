using PhoneAssistant.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PhoneAssistant.WPF.Models;
internal class SimCardRepository
{
    public async Task<IEnumerable<SimCard>?> AllAsync()
    {
        await Task.CompletedTask;
        return AllSimCards();
    }

    private static IEnumerable<SimCard> AllSimCards()
    {
        return new List<SimCard>()
        {
            new SimCard () { PhoneNumber = "07967691765", SimNumber = "8944125605559639023", Status="In Stock", Note=null},
            new SimCard () { PhoneNumber = "07967691764", SimNumber = "8944125605559639133", Status="Production", Note="Replacement" },
            new SimCard () { PhoneNumber = "07967691763" ,SimNumber = "8944125605559639323", Status="In Stock", Note=null },
            new SimCard () { PhoneNumber = "07967691762" ,SimNumber = "8944125605559639453", Status="In Stock", Note=null },
            new SimCard () { PhoneNumber = "07967691761" ,SimNumber = "8944125605559639893", Status="In Stock", Note=null },
            new SimCard () { PhoneNumber = "07967691760" ,SimNumber = "8944125605559639999", Status="Production", Note="Replacement" }
        };
    }

}
