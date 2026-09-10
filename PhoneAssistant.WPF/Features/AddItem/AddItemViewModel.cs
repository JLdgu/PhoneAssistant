using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using PhoneAssistant.Model;
using PhoneAssistant.WPF.Shared;
using System.Collections.ObjectModel;

namespace PhoneAssistant.WPF.Features.AddItem;

public interface IAddItemViewModel : IViewModel
{
}

public sealed partial class AddItemViewModel : ValidatableViewModel<AddItemViewModel>, IAddItemViewModel
{
    private readonly IApplicationSettingsRepository _appSettings;
    private readonly IMessenger _messenger;
    private readonly IPhonesRepository _phonesRepository;
    private readonly ISimRepository _simRepository;

    public ObservableCollection<string> LogItems { get; } = [];

    public AddItemViewModel(IPhonesRepository phonesRepository,
                            IApplicationSettingsRepository appSettings,
                            ISimRepository simRepository,
                            IMessenger messenger,
                            IServiceProvider serviceProvider) : base(serviceProvider)
    {
        _phonesRepository = phonesRepository ?? throw new ArgumentNullException(nameof(phonesRepository));
        _simRepository = simRepository ?? throw new ArgumentNullException(nameof(simRepository));
        _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
        _messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(PhoneSaveCommand))]
    [NotifyPropertyChangedFor(nameof(Status))]
    public partial string? AssetTag { get; set; }

    async partial void OnAssetTagChanged(string? value) => await ValidateAllPropertiesAsync();    

    public List<string> Conditions { get; } = ApplicationConstants.Conditions;

    [ObservableProperty]
    public partial string Condition { get; set; } = ApplicationConstants.Conditions[0][..1];

    [ObservableProperty]
    public partial bool Esim { get; set; }

    [ObservableProperty]
    public partial string? FormerUser { get; set; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(PhoneSaveCommand))]
    public partial string Imei { get; set; } = string.Empty;

    async partial void OnImeiChanged(string value) => await ValidateAllPropertiesAsync();

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(PhoneSaveCommand))]
    public partial string Model { get; set; } = "iPad A16";

    async partial void OnModelChanged(string value) => await ValidateAllPropertiesAsync();

    [ObservableProperty]
    public partial string? PhoneNotes { get; set; }

    public static IEnumerable<Manufacturer> OEMs => Enum.GetValues<Manufacturer>();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SerialNumber))]
    public partial Manufacturer OEM { get; set; } = Manufacturer.Apple;

    async partial void OnOEMChanged(Manufacturer value)
    {
        switch (value)
        {
            case Manufacturer.Apple:
                Model = "iPad A16";
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

        await ValidateAllPropertiesAsync();
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(PhoneSaveCommand))]
    public partial string? SerialNumber { get; set; }

    async partial void OnSerialNumberChanged(string? value) => await ValidateAllPropertiesAsync();

    public List<string> Statuses { get; } = ApplicationConstants.Statuses;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(AssetTag))]
    [NotifyPropertyChangedFor(nameof(Ticket))]
    public partial string Status { get; set; } = ApplicationConstants.StatusInStock;

    async partial void OnStatusChanged(string value)
    {
        if ((value == "Decommissioned" || value == "Disposed") && string.IsNullOrEmpty(Ticket))
            Ticket = _appSettings.ApplicationSettings.DefaultDecommissionedTicket.ToString();

        await ValidateAllPropertiesAsync();
    }

    [ObservableProperty]
    public partial string? Ticket { get; set; }

    async partial void OnTicketChanged(string? value) => await ValidateAllPropertiesAsync();

    [RelayCommand]
    private async Task PhoneClearAsync()
    {
        AssetTag = null;
        Condition = ApplicationConstants.Conditions[0][..1];
        Esim = false;
        FormerUser = null;
        Imei = string.Empty;
        PhoneNotes = null;
        PhoneNumber = null;
        OEM = Manufacturer.Apple;
        SerialNumber = null;
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
        Phone phone = new() { 
            AssetTag = AssetTag, Condition = Condition, Esim = Esim, FormerUser = FormerUser, Imei = Imei, Model = Model, 
            Notes = PhoneNotes, OEM = OEM, PhoneNumber = PhoneNumber, SerialNumber = SerialNumber, SimNumber = SimNumber, Ticket = sr, Status = Status };
        string conditionDesc = ApplicationConstants.ConditionRepurposed;
        if (Condition == ApplicationConstants.ConditionNew[..1])
            conditionDesc = ApplicationConstants.ConditionNew;
        
        string simDetails = string.Empty;
        if (PhoneNumber is not null)
        {
            if (Esim == true)
                simDetails += $"{PhoneNumber} eSIM {SimNumber}";
            else
                simDetails = $"SIM Card {PhoneNumber} SIM Card {SimNumber}";
        }

        await _phonesRepository.CreateAsync(phone);
        LogItems.Add($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} Phone added - {AssetTag} IMEI: {Imei} Status: {Status} Condition: {conditionDesc} {OEM} {Model} {FormerUser} {simDetails}");
        _messenger.Send(phone);

        await PhoneClearAsync();
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(PhoneSaveCommand))]
    public partial string? PhoneNumber { get; set; }

    async partial void OnPhoneNumberChanged(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return;
        
        await ValidateAllPropertiesAsync();

        string? simNumber = await _simRepository.GetSimNumber(value);

        if (simNumber is null) return;

        SimNumber = simNumber;
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(PhoneSaveCommand))]
    public partial string? SimNumber { get; set; }

    async partial void OnSimNumberChanged(string? value) => await ValidateAllPropertiesAsync();
}
