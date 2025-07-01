using PhoneAssistant.WPF.Shared;
using System.Collections.ObjectModel;

namespace PhoneAssistant.WPF.Features.Phones;

public interface IPhonesMainViewModel : IViewModel
{
    ObservableCollection<PhonesItemViewModel> PhoneItems { get; }
}
