using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using PhoneAssistant.WPF.Application;
using PhoneAssistant.WPF.Application.Repositories;
using PhoneAssistant.WPF.Shared;

namespace PhoneAssistant.WPF.Features.AddItem;
public partial class AddItemViewModel : ObservableValidator, IViewModel
{
    private readonly IPhonesRepository _phonesRepository;
    private bool _isValidated = false;

    public AddItemViewModel(IPhonesRepository phonesRepository)
    {
        _phonesRepository = phonesRepository ?? throw new ArgumentNullException(nameof(phonesRepository));
    }

    public List<string> Conditions { get; } = ApplicationSettings.Conditions;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(PhoneSaveCommand))]
    private string _phoneCondition = string.Empty;

    partial void OnPhoneConditionChanged(string value)
    {
        _isValidated = true;
    }

    public List<string> Statuses { get; } = ApplicationSettings.Statuses;

    [ObservableProperty]
    private string _phoneStatus = string.Empty;

    [ObservableProperty]
    private string _phoneImei = string.Empty;

    [RelayCommand]
    private void PhoneClear()
    {
        PhoneStatus = string.Empty;
        PhoneImei = string.Empty;
    }

    public bool CanSavePhone()
    {
        return !HasErrors && _isValidated;
    }


    [RelayCommand(CanExecute = nameof(CanSavePhone))]
    private void PhoneSave()
    {

    }   

    public Task LoadAsync()
    {
        return Task.CompletedTask;
    }
}
