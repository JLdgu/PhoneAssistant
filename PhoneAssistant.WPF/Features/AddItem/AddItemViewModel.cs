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
        ValidateAllProperties();
    }

    public List<string> Conditions { get; } = ApplicationSettings.Conditions;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(PhoneSaveCommand))]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Asset Tag is required")]
    [RegularExpression(@"MP\d{5}",ErrorMessage = "Asset Tag format MPnnnnn")]
    private string _assetTag = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(PhoneSaveCommand))]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Condition is required")]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    private string? _condition = string.Empty;

    [ObservableProperty]
    private string _formerUser = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(PhoneSaveCommand))]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "IMEI is required")]
    [CustomValidation(typeof(AddItemViewModel), nameof(ValidateImeiAsync))]
    private string _imei = string.Empty;

    public static ValidationResult ValidateImeiAsync(string imei, ValidationContext context)
    {   
        if (string.IsNullOrWhiteSpace(imei)) return new ValidationResult("IMEI is required");

        if(!LuhnValidator.IsValid(imei, 15)) return new ValidationResult("IMEI check digit incorrect");

        AddItemViewModel vm = (AddItemViewModel)context.ObjectInstance;

        bool unique = Task.Run(() => vm.IsIMEIUniqueAsync(imei)).GetAwaiter().GetResult();

#pragma warning disable CS8603 // Possible null reference return.
        if (unique) return ValidationResult.Success;
#pragma warning restore CS8603 // Possible null reference return.
        
        return new ValidationResult("IMEI must be unique");
    }

    private async Task<bool> IsIMEIUniqueAsync(string imei) => !await _phonesRepository.ExistsAsync(imei);

    public List<string> Statuses { get; } = ApplicationSettings.Statuses;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(PhoneSaveCommand))]
    [NotifyDataErrorInfo]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Status is required")]
    private string? _status = string.Empty;

    [RelayCommand]
    private void PhoneClear()
    {
        AssetTag = string.Empty;
        Condition = null;
        FormerUser = string.Empty;
        Imei = string.Empty;
        Status = null;

        ValidateAllProperties();
    }

    public bool CanSavePhone() => !HasErrors;

    [RelayCommand(CanExecute = nameof(CanSavePhone))]
    private void PhoneSave()
    {
        PhoneClear();
    }

    public Task LoadAsync()
    {
        return Task.CompletedTask;
    }
}
