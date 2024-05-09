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
    private readonly ISimsRepository _simsRepository;

    public AddItemViewModel(IPhonesRepository phonesRepository,
                            ISimsRepository simsRepository)
    {
        _phonesRepository = phonesRepository ?? throw new ArgumentNullException(nameof(phonesRepository));
        _simsRepository = simsRepository ?? throw new ArgumentNullException(nameof(simsRepository));
        OEM = Application.Entities.OEMs.Samsung;
        ValidateAllProperties();
    }

    #region NewPhone
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(PhoneSaveCommand), nameof(PhoneWithSIMSaveCommand))]
    [NotifyDataErrorInfo]    
    [RegularExpression(@"MP\d{5}",ErrorMessage = "Asset Tag format MPnnnnn")]
    [CustomValidation(typeof(AddItemViewModel), nameof(ValidateAssetTag))]
    private string? _assetTag;

    public static ValidationResult ValidateAssetTag(string assetTag, ValidationContext context)
    {
#pragma warning disable CS8603 // Possible null reference return.
        if (assetTag is null) return ValidationResult.Success;
#pragma warning restore CS8603 // Possible null reference return.

        AddItemViewModel vm = (AddItemViewModel)context.ObjectInstance;

        bool unique = Task.Run(() => vm.IsAssetTagUniqueAsync()).GetAwaiter().GetResult();
                
#pragma warning disable CS8603 // Possible null reference return.
        if (unique) return ValidationResult.Success;
#pragma warning restore CS8603 // Possible null reference return.

        return new ValidationResult("Asset Tag must be unique");
    }
    private async Task<bool> IsAssetTagUniqueAsync() => await _phonesRepository.AssetTagUniqueAsync(AssetTag);

    public List<string> Conditions { get; } = ApplicationSettings.Conditions;

    [ObservableProperty]
    private string _condition = ApplicationSettings.Conditions[1].Substring(0,1);

    [ObservableProperty]
    private string? _formerUser;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(PhoneSaveCommand), nameof(PhoneWithSIMSaveCommand))]
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
    [NotifyCanExecuteChangedFor(nameof(PhoneSaveCommand), nameof(PhoneWithSIMSaveCommand))]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Model is required")]
    private string _model = string.Empty;

    [ObservableProperty]
    private string? _phoneNotes;

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
        AssetTag = null;
        Condition = ApplicationSettings.Conditions[1].Substring(0, 1);
        FormerUser = null;
        Imei = string.Empty;
        Model = string.Empty;
        PhoneNotes = null;
        OEM = Application.Entities.OEMs.Samsung;
        Status = ApplicationSettings.Statuses[1];

        ValidateAllProperties();
    }

    public bool CanSavePhone()
    {
        if (GetErrors(nameof(AssetTag)).Count() != 0) return false;
        if (GetErrors(nameof(Imei)).Count() != 0) return false;
        if (GetErrors(nameof(Model)).Count() != 0) return false;
        return true;
    }

    [RelayCommand(CanExecute = nameof(CanSavePhone))]
    private async Task PhoneSaveAsync()
    {
        Phone phone = new() { AssetTag = AssetTag, Condition = Condition, FormerUser = FormerUser, Imei = Imei, Model = Model, Notes = PhoneNotes, OEM = OEM, PhoneNumber = PhoneNumber, SimNumber = SimNumber, Status = Status };
        await _phonesRepository.CreateAsync(phone);

        PhoneClear();
    }
    #endregion

    #region NewSIM
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SIMSaveCommand), nameof(SIMDeleteCommand), nameof(PhoneWithSIMSaveCommand))]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Phone Number is required")]
    [RegularExpression(@"0\d{9,10}", ErrorMessage = "Phone Number must be 10 or 11 digits")]
    [CustomValidation(typeof(AddItemViewModel), nameof(ValidatePhoneNumber))]
    private string? _phoneNumber;

    public static ValidationResult ValidatePhoneNumber(string phoneNumber, ValidationContext context)
    {
        AddItemViewModel vm = (AddItemViewModel)context.ObjectInstance;

        bool unique  = Task.Run(() => vm.IsPhoneNumberUniqueAsync(phoneNumber)).GetAwaiter().GetResult();

#pragma warning disable CS8603 // Possible null reference return.
        if (unique) return ValidationResult.Success;
#pragma warning restore CS8603 // Possible null reference return.

        return new ValidationResult("Phone Number already linked to phone");
    }
    private async Task<bool> IsPhoneNumberUniqueAsync(string phoneNumber) => !await _phonesRepository.PhoneNumberExistsAsync(phoneNumber);
    
    async partial void OnPhoneNumberChanged(string? value)
    {
        _newSIM = true;
        if (string.IsNullOrWhiteSpace(value)) return;

        string? simNumber = await _simsRepository.GetSIMNumberAsync(value);

        if (simNumber is null) return;

        SimNumber = simNumber;
        _newSIM = false;
    }
    private bool _newSIM = true;

    [ObservableProperty]
    private string? _simNotes;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SIMSaveCommand), nameof(SIMDeleteCommand), nameof(PhoneWithSIMSaveCommand))]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "SIM Number is required")]
    [RegularExpression(@"8944\d{15}", ErrorMessage = "SIM Number must be 19 digits")]
    [CustomValidation(typeof(AddItemViewModel), nameof(ValidateSimNumber))]
    private string? _simNumber;

    public static ValidationResult ValidateSimNumber(string? simNumber, ValidationContext context)
    {
        if (string.IsNullOrWhiteSpace(simNumber)) return new ValidationResult("SIM Number is required");

        if (!LuhnValidator.IsValid(simNumber, 19)) return new ValidationResult("SIM Number check digit incorrect");

#pragma warning disable CS8603 // Possible null reference return.
        return ValidationResult.Success;
#pragma warning restore CS8603 // Possible null reference return.
    }


    [RelayCommand]
    private void SIMClear()
    {
        PhoneNumber = null;
        SimNotes = null;
        SimNumber = null;
    }

    public bool CanDeleteSIM()
    {
        if (_newSIM) return false;
        if (GetErrors(nameof(PhoneNumber)).Count() != 0) return false;
        if (GetErrors(nameof(SimNumber)).Count() != 0) return false;
        return true;
    }

    [RelayCommand(CanExecute = nameof(CanDeleteSIM))]
    private async Task SIMDeleteAsync()
    {
        _ = await _simsRepository.DeleteSIMAsync(PhoneNumber!);

        SIMClear();
    }

    public bool CanSaveSIM()
    {
        if (!_newSIM) return false;
        if (GetErrors(nameof(PhoneNumber)).Count() != 0) return false;
        if (GetErrors(nameof(SimNumber)).Count() != 0) return false;
        return true;
    }

    [RelayCommand(CanExecute = nameof(CanSaveSIM))]
    private async Task SIMSaveAsync()
    {
        Sim sim = new() { PhoneNumber = PhoneNumber!, SimNumber = SimNumber!, Notes = PhoneNotes };
        await _simsRepository.CreateAsync(sim);

        SIMClear();

    }
    #endregion

    public bool CanSavePhoneWithSIM()
    {
        if (CanSavePhone() && CanSaveSIM()) return true;
        return false; 
    }

    [RelayCommand(CanExecute =nameof(CanSavePhoneWithSIM))]
    private async Task PhoneWithSIMSaveAsync()
    {
        if (!_newSIM)
            await _simsRepository.DeleteSIMAsync(PhoneNumber!);

        await PhoneSaveAsync();
        SIMClear();
    }

    public Task LoadAsync()
    {
        return Task.CompletedTask;
    }
}
