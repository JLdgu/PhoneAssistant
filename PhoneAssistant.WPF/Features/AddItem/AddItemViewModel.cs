using System.ComponentModel.DataAnnotations;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using PhoneAssistant.WPF.Application;
using PhoneAssistant.WPF.Application.Entities;
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

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(PhoneSaveCommand))]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Asset Tag is required")]
    [RegularExpression(@"MP\d{5}",ErrorMessage = "Asset Tag format MPnnnnn")]
    private string _assetTag = string.Empty;

    public List<string> Conditions { get; } = ApplicationSettings.Conditions;

    [ObservableProperty]
    private string _condition = ApplicationSettings.Conditions[0];

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

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(PhoneSaveCommand))]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Model is required")]
    private string _model = string.Empty;

    [ObservableProperty]
    private string _notes = string.Empty;

    public IEnumerable<OEMs> OEMs
    {
        get { return Enum.GetValues(typeof(OEMs)).Cast<OEMs>(); }
    }

    [ObservableProperty]
    private OEMs _oEM;

    public List<string> Statuses { get; } = ApplicationSettings.Statuses;

    [ObservableProperty]
    private string _status = ApplicationSettings.Statuses[1];

    [RelayCommand]
    private void PhoneClear()
    {
        AssetTag = string.Empty;
        Condition = ApplicationSettings.Conditions[0];
        FormerUser = string.Empty;
        Imei = string.Empty;
        Model = string.Empty;
        Notes = string.Empty;
        OEM = Application.Entities.OEMs.Apple;
        Status = ApplicationSettings.Statuses[1];

        ValidateAllProperties();
    }

    public bool CanSavePhone() => !HasErrors;

    [RelayCommand(CanExecute = nameof(CanSavePhone))]
    private async Task PhoneSaveAsync()
    {
        Phone phone = new() { AssetTag = AssetTag, Condition = Condition, FormerUser = FormerUser, Imei = Imei, Model = Model, OEM = OEM, Status = Status };
        await _phonesRepository.CreateAsync(phone);

        PhoneClear();
    }

    public Task LoadAsync()
    {
        return Task.CompletedTask;
    }
}
