using PhoneAssistant.WPF.Shared;

namespace PhoneAssistant.WPF.Features.Dymo;

public class DymoViewModel : IViewModel
{
    public Task LoadAsync()
    {
        return Task.CompletedTask;
    }
}
