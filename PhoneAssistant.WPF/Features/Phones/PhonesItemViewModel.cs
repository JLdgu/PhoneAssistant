using System.Windows;

using CommunityToolkit.Mvvm.Input;

using PhoneAssistant.WPF.Application.Entities;

namespace PhoneAssistant.WPF.Features.Phones;

public sealed partial class PhonesItemViewModel
{
    private readonly IPhonesRepository _repository;

    public v1Phone Phone { get; set; }

    public PhonesItemViewModel(IPhonesRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    [RelayCommand(CanExecute = nameof(CanRemoveSim))]
    public void RemoveSim()
    {
        _repository.RemoveSimFromPhone(Phone);
        Phone.PhoneNumber = null;
        Phone.SimNumber = null;
    }

    private bool CanRemoveSim() => !string.IsNullOrEmpty(Phone.PhoneNumber);
}
