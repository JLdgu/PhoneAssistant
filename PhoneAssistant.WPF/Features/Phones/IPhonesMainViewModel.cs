using System.Collections.ObjectModel;

using PhoneAssistant.WPF.Shared;
using PhoneAssistant.WPF.Application.Entities;

namespace PhoneAssistant.WPF.Features.Phones;

public interface IPhonesMainViewModel : IViewModel
{
    ObservableCollection<PhonesItemViewModel> PhoneItems { get; }
    //ObservableCollection<v1Phone> Phones { get; }
}
