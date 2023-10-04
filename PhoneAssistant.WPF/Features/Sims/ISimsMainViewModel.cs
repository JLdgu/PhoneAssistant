using System.Collections.ObjectModel;

using PhoneAssistant.WPF.Application.Entities;
using PhoneAssistant.WPF.Shared;

namespace PhoneAssistant.WPF.Features.Sims;

public interface ISimsMainViewModel : IViewModel
{
    ObservableCollection<v1Sim> Sims { get; }
}