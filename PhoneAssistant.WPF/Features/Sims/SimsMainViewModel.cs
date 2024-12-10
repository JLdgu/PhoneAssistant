namespace PhoneAssistant.WPF.Features.Sims;

public sealed partial class SimsMainViewModel : ISimsMainViewModel
{
    public Task LoadAsync()
    {
        return Task.CompletedTask;
    }
}
