using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PhoneAssistant.Model;
using PhoneAssistant.WPF.Shared;

namespace PhoneAssistant.WPF.Features.Sims;

public interface ISimsMainViewModel : IViewModel { }

public sealed partial class SimsMainViewModel(
    ILocationsRepository locationsRepository,
    IPrintEnvelope printEnvelope,
    IServiceProvider serviceProvider,
    ISimRepository simRepository
    ) : ValidatableViewModel<SimsMainViewModel>(serviceProvider), ISimsMainViewModel
{
    private readonly ILocationsRepository _locationsRepository = locationsRepository ?? throw new ArgumentNullException(nameof(locationsRepository));
    private readonly IPrintEnvelope _printEnvelope = printEnvelope ?? throw new ArgumentNullException(nameof(printEnvelope));
    private readonly ISimRepository _simRepository = simRepository ?? throw new ArgumentNullException(nameof(simRepository));

    private DeliveryAddressModel? _deliveryAddressModel;
    private readonly OrderDetails _orderDetails = new(new() { Status = "Production", Imei = "imei", Model = "SIM Card", Condition = "norr", OEM = Manufacturer.EE });

    [ObservableProperty]
    public partial string EmailHtml { get; set; } = "<p>Control will update when no errors are present</p>";

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(PrintEnvelopeCommand))]
    public partial string? NewUser { get; set; }
    async partial void OnNewUserChanged(string? value) => await ValidatePropertyAsync(nameof(NewUser));

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(PrintEnvelopeCommand))]
    public partial string? PhoneNumber { get; set; }
    async partial void OnPhoneNumberChanged(string? value)
    {
        await ValidatePropertyAsync(nameof(PhoneNumber));        

        SimNumber = await _simRepository.GetSimNumber(PhoneNumber!);
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(PrintEnvelopeCommand))]
    public partial string? SimNumber { get;  set; }
    async partial void OnSimNumberChanged(string? value)
    {
        await ValidatePropertyAsync(nameof(SimNumber));
        GenerateEmailHtml();
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(PrintEnvelopeCommand))]
    public partial string? Ticket { get;  set; } 
    async partial void OnTicketChanged(string? value)
    {
        await ValidatePropertyAsync(nameof(Ticket));
        GenerateEmailHtml();
    }


    [RelayCommand(CanExecute = nameof(CanPrintEnvelope))]
    private async Task PrintEnvelope()
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(Ticket);
        int? ticket = int.Parse(Ticket);
        Phone phone = new()
        {
            PhoneNumber = PhoneNumber,
            SimNumber = SimNumber,
            Status = "Production",
            Imei = "imei",
            Model = "SIM Card",
            NewUser = NewUser,
            Condition = "norr",
            OEM = Manufacturer.Apple,
            Ticket = ticket
        };

        OrderDetails orderDetails = new(phone);
        orderDetails.Execute(null);
        

        await Task.Run(() => _printEnvelope.Execute(orderDetails.DocumentName, orderDetails.EnvelopeInsertText));
    }
    private bool CanPrintEnvelope() => HasErrors == false;

    private void DeliveryAddressModel_SelectedLocationChanged(object? sender, Location? value)
    {
        if (value is null) return;

        string deliveryAddress = value.Address;
        deliveryAddress = deliveryAddress.Replace("{NewUser}", _orderDetails!.Phone.NewUser);
        deliveryAddress = deliveryAddress.Replace("{SR}", Ticket + " " + _orderDetails.OrderType + " " + _orderDetails.DeviceType);
        deliveryAddress = deliveryAddress.Replace("{PhoneNumber}", PhoneNumber);

        //DeliveryAddress = deliveryAddress;
        GenerateEmailHtml();

        //OnPropertyChanged(nameof(SelectedLocation));
    }


    public void GenerateEmailHtml()
    {
        if (HasErrors) return;

        _orderDetails.Phone.PhoneNumber = PhoneNumber;
        _orderDetails.Phone.NewUser = NewUser;
        _orderDetails.Phone.SimNumber = SimNumber;
        _orderDetails.Phone.Ticket = int.Parse(Ticket!);

        _orderDetails.Execute(null);
        EmailHtml = _orderDetails.EmailText;
    }

    private bool _loaded = false;
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
