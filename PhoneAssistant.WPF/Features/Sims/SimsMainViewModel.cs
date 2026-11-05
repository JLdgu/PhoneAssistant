using System.Collections.ObjectModel;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using PhoneAssistant.Model;
using PhoneAssistant.WPF.Shared;

namespace PhoneAssistant.WPF.Features.Sims;

public enum SimCardType
{
    Data,
    VoiceTextData,
    VoiceText
}

public interface ISimsMainViewModel : IViewModel { }

public sealed partial class SimsMainViewModel : ValidatableViewModel<SimsMainViewModel>, ISimsMainViewModel
{
    private readonly ILocationsRepository _locationsRepository;
    private readonly IPrintDymoLabel _dymoLabel;
    private readonly IPrintEnvelope _printEnvelope;
    private readonly ISimRepository _simRepository;

    private readonly Phone _orderPhone = new()
    {
        Condition = "norr",
        DespatchDetails = string.Empty,
        Imei = "imei",
        Model = "SIM",
        NewUser = string.Empty,
        OEM = Manufacturer.EE,
        PhoneNumber = string.Empty,
        SimNumber = string.Empty,
        Ticket = 0,
        Status = "Production"
    };

    private DeliveryAddressModel? _deliveryAddressModel;

    [RelayCommand]
    private void Clear()
    {
        DeliveryAddress = string.Empty;
        SelectedLocation = null;

        Esim = false;
        NewUser = null;
        PhoneNumber = null;
        SimNumber = null;
        Ticket = null;
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(PrintDymoLabelCommand))]
    public partial string DeliveryAddress { get; set; } = string.Empty;
    async partial void OnDeliveryAddressChanged(string value) => await GenerateOutputAsync(null);

    [ObservableProperty]
    public partial string EmailHtml { get; set; } = "<p>Control will update when all mandatory are present</p>";

    [ObservableProperty]
    public partial bool Esim { get; set; }

    public ObservableCollection<Location> Locations => _deliveryAddressModel?.Locations ?? [];

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(PrintEnvelopeCommand))]
    public partial string? NewUser { get; set; }
    async partial void OnNewUserChanged(string? value) => await GenerateOutputAsync(null);

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(PrintEnvelopeCommand))]
    public partial string? PhoneNumber { get; set; }
    async partial void OnPhoneNumberChanged(string? value) => await GenerateOutputAsync(value);

    [ObservableProperty]
    public partial SimCardType SimType { get; set; } = SimCardType.Data;
    //partial void OnSimTypeChanged(SimCardType value)
    //{
    //    _orderPhone.Model = value switch
    //    {
    //        SimCardType.Data => "Data SIM",
    //        SimCardType.VoiceTextData => "Voice & Text + Data SIM",
    //        SimCardType.VoiceText => "Voice & Text SIM",
    //        _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
    //    };
    //}

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(PrintEnvelopeCommand))]
    public partial string? SimNumber { get; set; }
    async partial void OnSimNumberChanged(string? value) => await GenerateOutputAsync(null);

    private bool CanPrintDymoLabel() => SelectedLocation is not null || !string.IsNullOrWhiteSpace(DeliveryAddress);

    [RelayCommand(CanExecute = nameof(CanPrintDymoLabel))]
    private async Task PrintDymoLabel()
    {
        bool includeDate = false;
        if (SelectedLocation is not null)
        {
            includeDate = SelectedLocation.Collection;
        }
        await Task.Run(() => _dymoLabel.Execute(DeliveryAddress, includeDate));
        Clipboard.SetText(DeliveryAddress);
    }

    private bool CanPrintEnvelope() => HasErrors == false;

    [RelayCommand(CanExecute = nameof(CanPrintEnvelope))]
    private async Task PrintEnvelope()
    {
        OrderDetails orderDetails = new(_orderPhone)
        {
            OrderType = OrderType.New
        };
        orderDetails.Execute(null);

        await Task.Run(() => _printEnvelope.Execute(orderDetails.DocumentName, orderDetails.EnvelopeInsertText));
    }

    private void DeliveryAddressModel_SelectedLocationChanged(object? sender, Location? value)
    {
        if (value is null) return;

        string deliveryAddress = value.Address;
        deliveryAddress = deliveryAddress.Replace("{NewUser}", NewUser);
        deliveryAddress = deliveryAddress.Replace("{SR}", Ticket);
        deliveryAddress = deliveryAddress.Replace("{PhoneNumber}", PhoneNumber);

        DeliveryAddress = deliveryAddress;
        GenerateOutput();

        OnPropertyChanged(nameof(SelectedLocation));
    }

    public Location? SelectedLocation
    {
        get => _deliveryAddressModel?.SelectedLocation;
        set
        {
            _deliveryAddressModel?.SelectedLocation = value;
        }
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(PrintEnvelopeCommand))]
    public partial string? Ticket { get; set; }
    async partial void OnTicketChanged(string? value) => await GenerateOutputAsync(null);

    public async Task GenerateOutputAsync(string? phoneNumber)
    {
        await ValidateAllPropertiesAsync();
        if (phoneNumber is not null && !GetErrors(nameof(PhoneNumber)).Cast<string>().Any()) 
            SimNumber = await _simRepository.GetSimNumber(PhoneNumber!);

        GenerateOutput();
    }

    public void GenerateOutput()
    {
        if (HasErrors || string.IsNullOrEmpty(DeliveryAddress)) return;

        _orderPhone.DespatchDetails = DeliveryAddress;
        _orderPhone.NewUser = NewUser;
        _orderPhone.PhoneNumber = PhoneNumber;
        _orderPhone.SimNumber = SimNumber;
        _orderPhone.Ticket = int.Parse(Ticket!);

        OrderDetails orderDetails = new(_orderPhone)
        {
            OrderType = OrderType.New
        };
        orderDetails.Execute(SelectedLocation);

        EmailHtml = orderDetails.EmailText;
        OnPropertyChanged(nameof(EmailHtml));
    }

    private bool _loaded = false;

    public SimsMainViewModel(
        ILocationsRepository locationsRepository,
        IPrintDymoLabel dymoLabel,
        IPrintEnvelope printEnvelope,
        IServiceProvider serviceProvider,
        ISimRepository simRepository
    ) : base(serviceProvider)
    {
        _locationsRepository = locationsRepository ?? throw new ArgumentNullException(nameof(locationsRepository));
        _dymoLabel = dymoLabel ?? throw new ArgumentNullException(nameof(dymoLabel));
        _printEnvelope = printEnvelope ?? throw new ArgumentNullException(nameof(printEnvelope));
        _simRepository = simRepository ?? throw new ArgumentNullException(nameof(simRepository));
    }

    public override async Task LoadAsync()
    {
        if (_loaded) return;

        if (_deliveryAddressModel is null)
        {
            _deliveryAddressModel = new DeliveryAddressModel(_locationsRepository);
            _deliveryAddressModel.SelectedLocationChanged += DeliveryAddressModel_SelectedLocationChanged;
        }

        await _deliveryAddressModel.LoadAsync();

        _loaded = true;
    }
}
