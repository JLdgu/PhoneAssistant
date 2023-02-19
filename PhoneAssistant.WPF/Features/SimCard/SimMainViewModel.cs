using PhoneAssistant.WPF.Shared;
using System.Threading.Tasks;

namespace PhoneAssistant.WPF.Features.SimCard;

internal sealed partial class SimMainViewModel : IViewModel
{
    //public string ViewPackIcon => "SimOutline";

    //public string ViewName => "SIM Cards";

    public async Task LoadAsync()
    {
        await Task.CompletedTask;
    }
}
