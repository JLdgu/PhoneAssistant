using System.Collections.ObjectModel;

using PhoneAssistant.WPF.Application.Entities;

namespace PhoneAssistant.WPF.Features.Dashboard;
public class DashboardMainViewModel : IDashboardMainViewModel
{

    public Task LoadAsync()
    {
        return Task.CompletedTask;
    }

    public Task WindowClosingAsync()
    {
        throw new NotImplementedException();
    }

}