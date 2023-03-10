using PhoneAssistant.WPF.Models;
using System.Collections.ObjectModel;

using PhoneAssistant.WPF.Shared;

namespace PhoneAssistant.WPF.Features.Phones
{
    public interface IPhonesMainViewModel : IViewModel
    {
        ObservableCollection<Phone> Phones { get; }
    }
}
