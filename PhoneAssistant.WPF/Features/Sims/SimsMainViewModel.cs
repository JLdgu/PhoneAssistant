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

    [RelayCommand]
    private void Clear()
    {
        Esim = false;
        NewUser = null;
        PhoneNumber = null;
        SimNumber = null;
        Ticket = null;
    }

    [ObservableProperty]
    public partial string EmailHtml { get; set; } = "<p>Control will update when no errors are present</p>";

    [ObservableProperty]
    public partial bool Esim { get; set; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(PrintEnvelopeCommand))]
    public partial string? NewUser { get; set; }
    async partial void OnNewUserChanged(string? value) => await ValidateAllPropertiesAsync();

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(PrintEnvelopeCommand))]
    public partial string? PhoneNumber { get; set; }
    async partial void OnPhoneNumberChanged(string? value)
    {
        await ValidateAllPropertiesAsync();
        if (GetErrors(nameof(PhoneNumber)).Cast<string>().Any()) return;

        SimNumber = await _simRepository.GetSimNumber(PhoneNumber!);
        GenerateEmailHtml();
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(PrintEnvelopeCommand))]
    public partial string? SimNumber { get;  set; }
    async partial void OnSimNumberChanged(string? value)
    {
        await ValidateAllPropertiesAsync();
        GenerateEmailHtml();
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(PrintEnvelopeCommand))]
    public partial string? Ticket { get;  set; } 
    async partial void OnTicketChanged(string? value)
    {
        await ValidateAllPropertiesAsync();
        GenerateEmailHtml();
    }

    [RelayCommand(CanExecute = nameof(CanPrintEnvelope))]
    private async Task PrintEnvelope()
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(Ticket);
        int? ticket = int.Parse(Ticket);
        Phone phone = new()
        {
            Condition = "norr",
            Imei = "imei",
            Model = "SIM Card",
            NewUser = NewUser,
            OEM = Manufacturer.Apple,
            PhoneNumber = PhoneNumber,
            SimNumber = SimNumber,
            Status = "Production",
            Ticket = ticket
        };

        OrderDetails orderDetails = new(phone);
        orderDetails.Execute(null);
        

        await Task.Run(() => _printEnvelope.Execute(orderDetails));
    }
    private bool CanPrintEnvelope() => HasErrors == false;

    private void DeliveryAddressModel_SelectedLocationChanged(object? sender, Location? value)
    {
        if (value is null) return;

        //string deliveryAddress = value.Address;
        //deliveryAddress = deliveryAddress.Replace("{NewUser}", _orderDetails!.Phone.NewUser);
        //deliveryAddress = deliveryAddress.Replace("{SR}", Ticket + " " + _orderDetails.OrderType + " " + _orderDetails.DeviceType);
        //deliveryAddress = deliveryAddress.Replace("{PhoneNumber}", PhoneNumber);

        //DeliveryAddress = deliveryAddress;
        GenerateEmailHtml();

        //OnPropertyChanged(nameof(SelectedLocation));
    }

    public void GenerateEmailHtml()
    {
        if (HasErrors) return;
        
        OrderDetails orderDetails = new(
            new() { 
                Condition = "norr", 
                Imei = "imei", 
                Model = "SIM Card", 
                NewUser = NewUser,
                OEM = Manufacturer.EE,
                PhoneNumber = PhoneNumber,
                SimNumber = SimNumber,
                Ticket = int.Parse(Ticket!),
                Status = "Production" 
            });

        orderDetails.Execute(null);
        EmailHtml = orderDetails.EmailText;
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
