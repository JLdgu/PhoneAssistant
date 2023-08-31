using PhoneAssistant.WPF.Application.Entities;
using PhoneAssistant.WPF.Shared;

using System.Collections.ObjectModel;

namespace PhoneAssistant.WPF.Features.Dashboard;
public interface IDashboardMainViewModel : IViewModel
{
    ObservableCollection<v1Phone> Phones { get; }
}
