using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using PhoneAssistant.WPF.Application;
using PhoneAssistant.WPF.Application.Repositories;
using PhoneAssistant.WPF.Shared;

namespace PhoneAssistant.WPF.Features.AddItem;
public partial class AddItemViewModel : ObservableObject, IViewModel
{
    private readonly IPhonesRepository _phonesRepository;

    public List<string> Statuses { get; } = ApplicationSettings.Statuses;

    public AddItemViewModel(IPhonesRepository phonesRepository)
    {
        _phonesRepository = phonesRepository ?? throw new ArgumentNullException(nameof(phonesRepository));
    }

    [ObservableProperty]
    private bool _conditionNew;

    [ObservableProperty]
    private bool _conditionRepurposed;

    [RelayCommand]
    private void PhoneClear()
    {

    }

    private bool CanSavePhone() => false;

    [RelayCommand(CanExecute = nameof(CanSavePhone))]
    private void PhoneSave()
    {

    }

    private bool CanPrintEnvelope() => false;
    [RelayCommand(CanExecute = nameof(CanPrintEnvelope))]
    private void PrintEnvelope()
    {
    }


    public Task LoadAsync()
    {
        return Task.CompletedTask;
    }
}
