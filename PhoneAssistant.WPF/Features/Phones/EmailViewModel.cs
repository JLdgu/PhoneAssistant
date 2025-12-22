using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PhoneAssistant.Model;
using PhoneAssistant.WPF.Shared;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace PhoneAssistant.WPF.Features.Phones;

public partial class EmailViewModel(IPhonesRepository phonesRepository,
                                    ILocationsRepository locationsRepository,
                                    IPrintEnvelope printEnvelope,
                                    IPrintDymoLabel dymoLabel
                                   ) : ObservableObject, IViewModel
{
    private readonly IPhonesRepository _phonesRepository = phonesRepository ?? throw new ArgumentNullException();
    private readonly ILocationsRepository _locationsRepository = locationsRepository ?? throw new ArgumentNullException();
    private readonly IPrintEnvelope _printEnvelope = printEnvelope ?? throw new ArgumentNullException();
    private readonly IPrintDymoLabel _dymoLabel = dymoLabel ?? throw new ArgumentNullException();

    private OrderDetails? _orderDetails;
    public OrderDetails OrderDetails
    {
        get => _orderDetails ?? throw new ArgumentNullException();
        set
        {
            _orderDetails = value;
            EnvelopePrinted = false;

            AssetTag = value.AssetTag;
            DeliveryAddress = value.DeliveryAddress;
            Imei = value.Imei;
            OrderType = value.OrderType;
            PhoneNumber = value.PhoneNumber;
            SelectedLocation = null;
            Ticket = value.Ticket;

            GeneratingEmail = true;
            GenerateEmailHtml();
        }
    }
    [ObservableProperty]
    private bool _envelopePrinted = false;

    [ObservableProperty]
    private string _imei = string.Empty;

    [ObservableProperty]
    private string _phoneNumber = string.Empty;

    [ObservableProperty]
    private string _assetTag = string.Empty;

    [ObservableProperty]
    private string? _ticket;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(PrintEnvelopeCommand))]
    private OrderType _orderType;
    partial void OnOrderTypeChanged(OrderType value)
    {
        OrderDetails.OrderType = OrderType;
        GenerateEmailHtml();
    }

    [RelayCommand]
    private async Task CloseAsync()
    {
        await _phonesRepository.UpdateAsync(_orderDetails!.Phone);
        GeneratingEmail = false;
    }
    
    private bool CanPrintEnvelope() => OrderType != OrderType.None;
    [RelayCommand(CanExecute = nameof(CanPrintEnvelope))]
    private async Task PrintEnvelope()
    {
        if (_orderDetails is null) return;

        await Task.Run(() =>
        {
            _printEnvelope.Execute(_orderDetails);
        });
        EnvelopePrinted = true;
    }

    [RelayCommand]
    private async Task PrintDymoLabel()
    {
        await Task.Run(() =>
        {
            string? includeDate = null;
            if (SelectedLocation is not null && SelectedLocation.Collection)
                includeDate = OrderDetails.ToOrdinalWorkingDate(DateTime.Now, true);

            _dymoLabel.Execute(DeliveryAddress, includeDate);
        });

        Clipboard.SetText(DeliveryAddress);
    }

    private bool _loaded = false;
    public ObservableCollection<Location> Locations { get; set; } = [];

    [ObservableProperty]
    private Location? _selectedLocation;
    partial void OnSelectedLocationChanged(Location? value)
    {
        if (value is null) return;

        string deliveryAddress = value.Address;
        deliveryAddress = deliveryAddress.Replace("{NewUser}", _orderDetails!.Phone.NewUser);
        deliveryAddress = deliveryAddress.Replace("{SR}", Ticket);
        deliveryAddress = deliveryAddress.Replace("{PhoneNumber}", PhoneNumber);

        DeliveryAddress = deliveryAddress;
        GenerateEmailHtml();
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(EmailHtml))]
    private string _deliveryAddress = string.Empty;

    partial void OnDeliveryAddressChanged(string value)
    {
        if (_orderDetails is null) return;

        _orderDetails.Phone.DespatchDetails = value;

        GenerateEmailHtml();
    }

    public static string ReformatDeliveryAddress(string address)
    {
        Regex regex = AddressReformat();

        string reformatted = regex.Replace(address,string.Empty);

        return reformatted;
    }

    [ObservableProperty]
    private bool _generatingEmail;

    [ObservableProperty]
    private string _emailHtml = string.Empty;

    public void GenerateEmailHtml()
    {
        OrderDetails.Execute(SelectedLocation);
        EmailHtml = OrderDetails.EmailText;
    }

    public async Task LoadAsync()
    {
        if (_loaded) return;

        IEnumerable<Location> locations = await _locationsRepository.GetAllLocationsAsync();

        foreach (Location location in locations)
        {
            Locations.Add(location);
        }

        _loaded = true;
    }

    [GeneratedRegex(@"First line of address\r\n|Second line of address\r\n|Town/City\r\n|County\r\n|Postcode\r\n", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-GB")]
    private static partial Regex AddressReformat();
}
