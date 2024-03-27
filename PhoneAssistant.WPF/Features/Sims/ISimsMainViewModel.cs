using System.Collections.ObjectModel;

using PhoneAssistant.WPF.Shared;

namespace PhoneAssistant.WPF.Features.Sims;

public interface ISimsMainViewModel : IViewModel
{
    ObservableCollection<SimsItemViewModel> SimItems { get; }
}