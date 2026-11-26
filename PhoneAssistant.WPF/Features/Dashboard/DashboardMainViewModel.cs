using PhoneAssistant.WPF.Shared;

namespace PhoneAssistant.WPF.Features.Dashboard;

public interface IDashboardMainViewModel : IViewModel
{
}

public class DashboardMainViewModel : IDashboardMainViewModel
{

    public Task LoadAsync()
    {
        return Task.CompletedTask;
    }

}