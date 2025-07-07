using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using PhoneAssistant.Model;
using PhoneAssistant.WPF.Shared;

namespace PhoneAssistant.WPF.Features.AddItem;
public partial class AddItemViewModel : ObservableValidator, IViewModel
{
    private readonly IApplicationSettingsRepository _appSettings;
    private readonly IBaseReportRepository _baseReportRepository;
    private readonly IMessenger _messenger;
    private readonly IPhonesRepository _phonesRepository;

    public ObservableCollection<string> LogItems { get; } = [];

    public AddItemViewModel(IPhonesRepository phonesRepository,
                            IBaseReportRepository baseReportRepository,
                            IApplicationSettingsRepository appSettings,
                            IMessenger messenger)
    {
        _phonesRepository = phonesRepository ?? throw new ArgumentNullException(nameof(phonesRepository));
        _baseReportRepository = baseReportRepository ?? throw new ArgumentNullException(nameof(baseReportRepository));
        _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
        _messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
        OEM = Manufacturer.Apple;
        Model = "iPhone SE 2022";
        ValidateAllProperties();
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(PhoneSaveCommand))]
    [NotifyPropertyChangedFor(nameof(Status))]
    [NotifyDataErrorInfo]    
    [RegularExpression(@"(MP|PC)\d{5}",ErrorMessage = "Invalid format")]
    [CustomValidation(typeof(AddItemViewModel), nameof(ValidateAssetTag))]
    private string? _assetTag;

    partial void OnAssetTagChanged(string? value)
    {
        ValidateProperty(Status, "Status");
    }

    [return: MaybeNull]
    public static ValidationResult ValidateAssetTag(string assetTag, ValidationContext context)
    {
        if (assetTag is null) return ValidationResult.Success;

        AddItemViewModel vm = (AddItemViewModel)context.ObjectInstance;

        bool unique = Task.Run(() => vm.IsAssetTagUniqueAsync()).GetAwaiter().GetResult();
                
        if (unique) return ValidationResult.Success;
        return new ValidationResult("Asset Tag must be unique");
    }
    private async Task<bool> IsAssetTagUniqueAsync() => await _phonesRepository.AssetTagUniqueAsync(AssetTag);

    public List<string> Conditions { get; } = ApplicationConstants.Conditions;

    [ObservableProperty]
    private string _condition = ApplicationConstants.Conditions[1].Substring(0,1);

    [ObservableProperty]
    private string? _formerUser;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(PhoneSaveCommand))]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "IMEI is required")]
    [CustomValidation(typeof(AddItemViewModel), nameof(ValidateImeiAsync))]
    private string _imei = string.Empty;

    [return: MaybeNull]
    public static ValidationResult ValidateImeiAsync(string imei, ValidationContext context)
    {   
        if (string.IsNullOrWhiteSpace(imei)) return new ValidationResult("IMEI is required");

        if(!LuhnValidator.IsValid(imei, 15)) return new ValidationResult("IMEI check digit incorrect");

        AddItemViewModel vm = (AddItemViewModel)context.ObjectInstance;

        bool unique = Task.Run(() => vm.IsIMEIUniqueAsync(imei)).GetAwaiter().GetResult();
        if (unique) return ValidationResult.Success;
        return new ValidationResult("IMEI must be unique");
    }

    private async Task<bool> IsIMEIUniqueAsync(string imei) => !await _phonesRepository.ExistsAsync(imei);

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(PhoneSaveCommand))]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Model is required")]
    private string _model = string.Empty;

    [ObservableProperty]
    private string? _phoneNotes;

    public static IEnumerable<Manufacturer> OEMs => Enum.GetValues(typeof(Manufacturer)).Cast<Manufacturer>();

    [ObservableProperty]
    private Manufacturer _oEM;

    partial void OnOEMChanged(Manufacturer value)
    {
        switch (value)
        {
            case Manufacturer.Apple:
                Model = "iPhone SE 2022";
                break;
            case Manufacturer.Nokia:
                Model = "110 4G";
                break;
            case Manufacturer.Samsung:
               Model = "A32";
                break;
            case Manufacturer.Other:
                Model = "";
                break;
            default:
                break;
        }
    }

    public List<string> Statuses { get; } = ApplicationConstants.Statuses;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(AssetTag))]
    [NotifyPropertyChangedFor(nameof(Ticket))]
    [NotifyDataErrorInfo]
    [CustomValidation(typeof(AddItemViewModel), nameof(ValidateStatus))]
    private string _status = ApplicationConstants.StatusInStock;

    [return: MaybeNull]
    public static ValidationResult ValidateStatus(string status, ValidationContext context)
    {
        if (status != ApplicationConstants.StatusInStock)
            return ValidationResult.Success;
        AddItemViewModel vm = (AddItemViewModel)context.ObjectInstance;
        if (string.IsNullOrEmpty(vm.AssetTag))
            return new ValidationResult("Asset Tag required");
        return ValidationResult.Success;
    }

    partial void OnStatusChanged(string value)
    {
        if ((value == "Decommissioned" || value == "Disposed") && string.IsNullOrEmpty(Ticket))
            Ticket = _appSettings.ApplicationSettings.DefaultDecommissionedTicket.ToString();
        else
            ValidateProperty(Ticket, "Ticket");
    }

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [CustomValidation(typeof(Validation), nameof(Validation.ValidateTicket))]
    private string? _ticket;

    [RelayCommand]
    private void PhoneClear()
    {
        AssetTag = null;
        Condition = ApplicationConstants.Conditions[1].Substring(0, 1);
        FormerUser = null;
        Imei = string.Empty;
        PhoneNotes = null;
        PhoneNumber = null;
        OEM = Manufacturer.Apple;
        SimNumber = null;
        Status = ApplicationConstants.Statuses[1];
        SimNumber = null;
        Ticket = null;

        ValidateAllProperties();
    }

    public bool CanSavePhone()
    {
        if (GetErrors(nameof(AssetTag)).Any()) return false;
        if (GetErrors(nameof(Imei)).Any()) return false;
        if (GetErrors(nameof(Model)).Any()) return false;
        return true;
    }

    [RelayCommand(CanExecute = nameof(CanSavePhone))]
    private async Task PhoneSaveAsync()
    {
        int? sr = null;
        if (Ticket is not null)
            sr = int.Parse(Ticket);
        Phone phone = new() { AssetTag = AssetTag, Condition = Condition, FormerUser = FormerUser, Imei = Imei, Model = Model, Notes = PhoneNotes, OEM = OEM, PhoneNumber = PhoneNumber, SimNumber = SimNumber, SR = sr, Status = Status };
        string conditionDesc = ApplicationConstants.ConditionRepurposed;
        if (Condition == ApplicationConstants.ConditionNew.Substring(0,1))
            conditionDesc = ApplicationConstants.ConditionNew;
        
        string simDetails = string.Empty;
        if (PhoneNumber is not null)
            simDetails = $"SIM Card {PhoneNumber} {SimNumber}";

        await _phonesRepository.CreateAsync(phone);
        LogItems.Add($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} Phone added - {AssetTag} IMEI: {Imei} Status: {Status} Condition: {conditionDesc} {OEM} {Model} {FormerUser} {simDetails}");
        _messenger.Send(phone);
        PhoneClear();
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(PhoneSaveCommand))]
    [NotifyDataErrorInfo]    
    [RegularExpression(@"0\d{9,10}", ErrorMessage = "Phone Number must be 10 or 11 digits")]
    [CustomValidation(typeof(AddItemViewModel), nameof(ValidatePhoneNumber))]
    private string? _phoneNumber;

    public static ValidationResult? ValidatePhoneNumber(string phoneNumber, ValidationContext context)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber)) return ValidationResult.Success;

        AddItemViewModel vm = (AddItemViewModel)context.ObjectInstance;

        bool unique  = Task.Run(() => vm.IsPhoneNumberUniqueAsync(phoneNumber)).GetAwaiter().GetResult();

        if (unique) return ValidationResult.Success!;

        return new ValidationResult("Phone Number already linked to phone");
    }

    private async Task<bool> IsPhoneNumberUniqueAsync(string phoneNumber) => !await _phonesRepository.PhoneNumberExistsAsync(phoneNumber);
    
    async partial void OnPhoneNumberChanged(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return;

        string? simNumber = await _baseReportRepository.GetSimNumberAsync(value);

        if (simNumber is null) return;

        SimNumber = simNumber;
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(PhoneSaveCommand))]
    [NotifyDataErrorInfo]
    [RegularExpression(@"8944\d{15}", ErrorMessage = "SIM Number must be 19 digits")]
    [CustomValidation(typeof(Validation), nameof(Validation.ValidateSimNumber))]
    private string? _simNumber;

    [GeneratedRegex(@"8944\d{15}", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-GB")]
    private static partial Regex ImeiFormat();

    public Task LoadAsync()
    {
        return Task.CompletedTask;
    }
}
