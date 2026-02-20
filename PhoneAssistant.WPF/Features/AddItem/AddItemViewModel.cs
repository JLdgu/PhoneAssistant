using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using PhoneAssistant.Model;
using PhoneAssistant.WPF.Shared;

namespace PhoneAssistant.WPF.Features.AddItem;

public interface IAddItemViewModel : IViewModel
{
}

public sealed partial class AddItemViewModel : ValidatableViewModel<AddItemViewModel>, IAddItemViewModel
{
    private readonly IApplicationSettingsRepository _appSettings;
    private readonly IBaseReportRepository _baseReportRepository;
    private readonly IMessenger _messenger;
    private readonly IPhonesRepository _phonesRepository;

    public ObservableCollection<string> LogItems { get; } = [];

    public AddItemViewModel(IPhonesRepository phonesRepository,
                            IBaseReportRepository baseReportRepository,
                            IApplicationSettingsRepository appSettings,
                            IMessenger messenger,
                            IServiceProvider serviceProvider) : base(serviceProvider)
    {
        _phonesRepository = phonesRepository ?? throw new ArgumentNullException(nameof(phonesRepository));
        _baseReportRepository = baseReportRepository ?? throw new ArgumentNullException(nameof(baseReportRepository));
        _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
        _messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
        OEM = Manufacturer.Apple;
        Model = "iPhone SE 2022";
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(PhoneSaveCommand))]
    [NotifyPropertyChangedFor(nameof(Status))]
    private string? _assetTag;

    async partial void OnAssetTagChanged(string? value)
    {
        await ValidatePropertyAsync(nameof(AssetTag));
        await ValidatePropertyAsync(nameof(Status)); 
    }

    public List<string> Conditions { get; } = ApplicationConstants.Conditions;

    [ObservableProperty]
    private string _condition = ApplicationConstants.Conditions[1].Substring(0,1);

    [ObservableProperty]
    private string? _formerUser;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(PhoneSaveCommand))]
    private string _imei = string.Empty;
    async partial void OnImeiChanged(string value) => await ValidatePropertyAsync(nameof(Imei));    

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(PhoneSaveCommand))]
    private string _model = string.Empty;
    async partial void OnModelChanged(string value) => await ValidatePropertyAsync(nameof(Model));

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
                Model = "iPhone 16E";
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
    private string _status = ApplicationConstants.StatusInStock;

    async partial void OnStatusChanged(string value)
    {
        if ((value == "Decommissioned" || value == "Disposed") && string.IsNullOrEmpty(Ticket))
            Ticket = _appSettings.ApplicationSettings.DefaultDecommissionedTicket.ToString();

        await ValidatePropertyAsync(nameof(AssetTag));
        await ValidatePropertyAsync(nameof(Ticket)); 
    }

    [ObservableProperty]
    private string? _ticket;
    async partial void OnTicketChanged(string? value) => await ValidatePropertyAsync(nameof(Ticket));

    [RelayCommand]
    private async Task PhoneClearAsync()
    {
        AssetTag = null;
        Condition = ApplicationConstants.Conditions[1][..1];
        FormerUser = null;
        Imei = string.Empty;
        PhoneNotes = null;
        PhoneNumber = null;
        OEM = Manufacturer.Apple;
        SimNumber = null;
        Status = ApplicationConstants.Statuses[1];
        SimNumber = null;
        Ticket = null;

        await ValidateAllPropertiesAsync();        
    }

    public bool CanSavePhone() => HasErrors == false;

    [RelayCommand(CanExecute = nameof(CanSavePhone))]
    private async Task PhoneSaveAsync()
    {
        int? sr = null;
        if (Ticket is not null)
            sr = int.Parse(Ticket);
        Phone phone = new() { AssetTag = AssetTag, Condition = Condition, FormerUser = FormerUser, Imei = Imei, Model = Model, Notes = PhoneNotes, OEM = OEM, PhoneNumber = PhoneNumber, SimNumber = SimNumber, Ticket = sr, Status = Status };
        string conditionDesc = ApplicationConstants.ConditionRepurposed;
        if (Condition == ApplicationConstants.ConditionNew.Substring(0,1))
            conditionDesc = ApplicationConstants.ConditionNew;
        
        string simDetails = string.Empty;
        if (PhoneNumber is not null)
            simDetails = $"SIM Card {PhoneNumber} {SimNumber}";

        await _phonesRepository.CreateAsync(phone);
        LogItems.Add($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} Phone added - {AssetTag} IMEI: {Imei} Status: {Status} Condition: {conditionDesc} {OEM} {Model} {FormerUser} {simDetails}");
        _messenger.Send(phone);

        await PhoneClearAsync();
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(PhoneSaveCommand))]
    private string? _phoneNumber;
    
    async partial void OnPhoneNumberChanged(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return;
        
        await ValidatePropertyAsync(nameof(PhoneNumber));

        string? simNumber = await _baseReportRepository.GetSimNumberAsync(value);

        if (simNumber is null) return;

        SimNumber = simNumber;
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(PhoneSaveCommand))]
    private string? _simNumber;

    async partial void OnSimNumberChanged(string? value) => await ValidatePropertyAsync(nameof(SimNumber));
}
