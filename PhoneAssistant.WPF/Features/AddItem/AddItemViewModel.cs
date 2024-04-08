using System.ComponentModel.DataAnnotations;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using PhoneAssistant.WPF.Application;
using PhoneAssistant.WPF.Application.Repositories;
using PhoneAssistant.WPF.Shared;

namespace PhoneAssistant.WPF.Features.AddItem;
public partial class AddItemViewModel : ObservableValidator, IViewModel
{
    private readonly IPhonesRepository _phonesRepository;

    public AddItemViewModel(IPhonesRepository phonesRepository)
    {
        _phonesRepository = phonesRepository ?? throw new ArgumentNullException(nameof(phonesRepository));
    }

    public List<string> Conditions { get; } = ApplicationSettings.Conditions;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(PhoneSaveCommand))]
    private string _phoneCondition = string.Empty;

    public List<string> Statuses { get; } = ApplicationSettings.Statuses;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(PhoneSaveCommand))]
    private string _phoneStatus = string.Empty;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [CustomValidation(typeof(AddItemViewModel), nameof(ValidateImeiAsync))]
    private string _phoneImei = string.Empty;

    partial void OnPhoneImeiChanged(string value)
    {

    }

    public static ValidationResult ValidateImeiAsync(string imei, ValidationContext context)
    {   
        if (string.IsNullOrWhiteSpace(imei)) return new ValidationResult("IMEI is required");

        if(!LuhnValidator.IsValid(imei, 15)) return new ValidationResult("IMEI must be 15 digits");

        AddItemViewModel vm = (AddItemViewModel)context.ObjectInstance;

        bool unique = Task.Run(() => vm.IsIMEIUniqueAsync(imei)).GetAwaiter().GetResult();
        //var task = vm.IsIMEIUniqueAsync(imei);
        //task.Wait();
        //bool unique = task.Result;

#pragma warning disable CS8603 // Possible null reference return.
        if (unique) return ValidationResult.Success;
#pragma warning restore CS8603 // Possible null reference return.
        
        return new ValidationResult("IMEI must be unique");
    }

    private async Task<bool> IsIMEIUniqueAsync(string imei) => !await _phonesRepository.ExistsAsync(imei);

    [RelayCommand]
    private void PhoneClear()
    {
        PhoneStatus = string.Empty;
        PhoneImei = string.Empty;
    }

    public bool CanSavePhone()
    {
        if (HasErrors) return false;
        if (PhoneCondition == string.Empty) return false;
        //if (PhoneStatus == string.Empty) return false;

        return true;
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
