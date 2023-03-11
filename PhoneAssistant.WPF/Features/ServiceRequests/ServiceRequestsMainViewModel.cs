using PhoneAssistant.WPF.Shared;

namespace PhoneAssistant.WPF.Features.ServiceRequests;

public sealed class ServiceRequestsMainViewModel : IServiceRequestsMainViewModel
{
    public Task LoadAsync()
    {
        return Task.CompletedTask;
    }

    public Task WindowClosingAsync()
    {
        // TODO : Check outstanding edits have been saved
        return Task.CompletedTask;
    }
}
