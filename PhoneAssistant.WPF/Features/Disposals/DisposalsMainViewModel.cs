using PhoneAssistant.WPF.Shared;

namespace PhoneAssistant.WPF.Features.Disposals;

public interface IDisposalsMainViewModel : IViewModel
{
}

public partial class DisposalsMainViewModel : IDisposalsMainViewModel
{

    public async Task LoadAsync()
    {
        await Task.CompletedTask;
    }
}