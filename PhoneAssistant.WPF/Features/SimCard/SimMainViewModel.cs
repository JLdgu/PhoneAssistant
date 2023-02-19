using PhoneAssistant.WPF.Shared;
using System.Threading.Tasks;

namespace PhoneAssistant.WPF.Features.SimCard;

internal sealed partial class SimMainViewModel : IViewModel
{
    public async Task LoadAsync()
    {
        await Task.CompletedTask;
    }
}
