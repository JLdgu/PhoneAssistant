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

            Imei = value.Phone.Imei;
            PhoneNumber = value.Phone.PhoneNumber ?? string.Empty;
            AssetTag = value.Phone.AssetTag ?? string.Empty;
            Ticket = value.Phone.SR.ToString();
            OrderType = value.OrderType;
            SelectedLocation = null;            
            if (value.Phone.DespatchDetails is null)
            {
                StringBuilder user = new();
                user.AppendLine(value.Phone.NewUser!);
                DeliveryAddress = user.ToString();
            }
            else
                DeliveryAddress = value.Phone.DespatchDetails;            

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
                includeDate = ToOrdinalWorkingDate(DateTime.Now, true);

            _dymoLabel.Execute(DeliveryAddress, includeDate);
        });

        Clipboard.SetText(DeliveryAddress);
    }

    private bool loaded = false;
    public ObservableCollection<Location> Locations { get; set; } = new();

    [ObservableProperty]
    private Location? _selectedLocation;
    partial void OnSelectedLocationChanged(Location? value)
    {
        if (value is null) return;

        string deliveryAddress = value.Address;
        deliveryAddress = deliveryAddress.Replace("{NewUser}", _orderDetails!.Phone.NewUser);
        deliveryAddress = deliveryAddress.Replace("{SR}", _orderDetails!.Phone.SR.ToString());
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

        _formattedAddress = value.Replace(Environment.NewLine, "<br />");
        GenerateEmailHtml();
    }
    private string _formattedAddress = string.Empty;

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
        Location location = new () { Name = string.Empty, Address = string.Empty, Collection = false };
        if (SelectedLocation is not null)
            location = SelectedLocation;
        StringBuilder html = new("""
            <span style="font-size:14px; font-family:Verdana;">
            """);
        if (location.Collection)
        {
            html.AppendLine($"<p>{OrderDetails.Phone.NewUser} your {OrderDetails.Phone.OEM} {OrderDetails.Phone.Model} {OrderDetails.DeviceType.ToString().ToLower()} can be collected from</br>");
            if (location.Name.Contains("GMH"))
            {
                html.AppendLine("DTS End User Compute Team, Hardware Room, Great Moor House, Bittern Road, Exeter, EX2 7FW</br>");
                html.AppendLine($"It will be available for collection from {ToOrdinalWorkingDate(DateTime.Now,buffer: 2)}</p>");
            }
            else
            {
                html.AppendLine("DTS End User Compute Team, Room L87, County Hall, Topsham Road, Exeter, EX2 4QD</br>");
                html.AppendLine($"It will be available for collection from {ToOrdinalWorkingDate(DateTime.Now)}</p>");
            }
        }
        else
        {
            html.AppendLine($"<p>Your {OrderDetails.Phone.OEM} {OrderDetails.Phone.Model} {OrderDetails.DeviceType.ToString().ToLower()} has been sent to<br />{_formattedAddress}</br>");
            html.AppendLine($"It was sent on {ToOrdinalWorkingDate(DateTime.Now)}</p>");
        }

        if (location.Note is not null)
            html.AppendLine(location.Note);

        if (OrderDetails.Phone.OEM != Manufacturer.Nokia)
        {
            html.AppendLine("""
            <p><br /><a href="https://devoncc.sharepoint.com/:w:/r/sites/ICTKB/Public/DCC%20mobile%20phone%20data%20usage%20guidance%20and%20policies.docx?d=w9ce15b2ddbb343739f131311567dd305&csf=1&web=1">
            DCC mobile phone data usage guidance and policies</a></p>
            """);

            html.AppendLine("""
                <p><br />Before setting up your phone please ensure you register with <a href="https://www.wifi.service.gov.uk/connect-to-govwifi/">GovWifi</a></p>
                """);
            
            html.AppendLine($"<p><br />Detailed setup instructions for your {OrderDetails.DeviceType.ToString().ToLower()}, are available here:</br>");
            if (OrderDetails.Phone.OEM == Manufacturer.Apple)
            {
                html.Append(
                    """
                <a href="https://devoncc.sharepoint.com/:w:/r/sites/ICTKB/_layouts/15/Doc.aspx?sourcedoc=%7BABC3F4D7-1159-4F72-9C0B-7E155B970A28%7D&file=How%20to%20set%20up%20your%20new%20DCC%20iPhone.docx&action=default&mobileredirect=true">
                Setting up your Apple (iOS) Smartphone.docx (devoncc.sharepoint.com)</a></p>
                """);
            }
            else
            {
                html.AppendLine(
                    """
                <a href="https://devoncc.sharepoint.com/:w:/r/sites/ICTKB/Public/Android%20Enterprise%20-%20Setting%20up%20your%20Android%20Phone.docx?d=w64bb3f0a09e44557a64bb78311ee513b&csf=1&web=1">
                Android Enterprise - Setting up your Android Smartphone.docx (devoncc.sharepoint.com)</a></p>
                """);
            }
        }

        if (OrderType == OrderType.Replacement && OrderDetails.DeviceType == DeviceType.Phone)
        {
            html.AppendLine("<p><br /><span style=\"color: red;\">Don't forget to transfer your old sim to the replacement phone</span> before returning the old phone to");
            html.AppendLine("DTS End User Compute, Room L87, County Hall, Topsham Road, Exeter, EX2 4QD</br>");
            html.AppendLine(
                """
                You can use <a href="https://www.royalmail.com/track-my-return#/details/5244">Royal Mail Tracked Returns for DCC</a>
                """);
            html.AppendLine(" to have the phone picked up from your home or you can drop off the item at a Parcel Post Box, Delivery Office or Post Office branch.</p>");
        }
        html.AppendLine("<p><br /></p></span>");

        html.AppendLine(
            """
            <table style="font-size:12px;font-family: Verdana">
            """);
        html.AppendLine("<tr><th>Order Details</th><th></th></tr>");
        html.AppendLine($"<tr><td>Order type:</td><td>{OrderDetails.OrderedItem}</td></tr>");
        html.AppendLine($"<tr><td>Device supplied:</td><td>{OrderDetails.DeviceSupplied}</td></tr>");

        html.AppendLine($"<tr><td>Handset identifier:</td><td>{Imei}</td></tr>");
        html.AppendLine($"<tr><td>Asset tag:</td><td>{AssetTag}</td></tr>");
        if (!string.IsNullOrEmpty(PhoneNumber))
            html.AppendLine($"<tr><td>Phone number:</td><td>{PhoneNumber}</td></tr></table>");
        EmailHtml = html.ToString();
    }

    public static string ToOrdinalWorkingDate(DateTime date, bool hexSuperscript = false, int buffer = 0)
    {
        DateTime weekDay = date.AddDays(buffer);
        if (buffer > 0)
        {
            while (weekDay.DayOfWeek == DayOfWeek.Saturday || weekDay.DayOfWeek == DayOfWeek.Sunday)
                weekDay = weekDay.AddDays(buffer);
        }

        string ordinalDay = string.Empty;
        int number = weekDay.Day;
        switch (number % 100)
        {
            case 11:
            case 12:
            case 13:
                if (hexSuperscript)
                    ordinalDay = number.ToString() + "\x1D57\x02B0";
                else
                    ordinalDay = number.ToString() + "<sup>th</sup>";
                break;
        }

        if (ordinalDay == string.Empty)
        {
            switch (number % 10)
            {
                case 1:
                    if (hexSuperscript)
                        ordinalDay = number.ToString() + "\x02E2\x1D57";
                    else
                        ordinalDay = number.ToString() + "<sup>st</sup>";
                    break;
                case 2:
                    if (hexSuperscript)
                        ordinalDay = number.ToString() + "\x207F\x1D48";
                    else
                        ordinalDay = number.ToString() + "<sup>nd</sup>";
                    break;
                case 3:
                    if (hexSuperscript)
                        ordinalDay = number.ToString() + "\x02B3\x1D48";
                    else
                        ordinalDay = number.ToString() + "<sup>rd</sup>";
                    break;
                default:
                    if (hexSuperscript)
                        ordinalDay = number.ToString() + "\x1D57\x02B0";
                    else
                        ordinalDay = number.ToString() + "<sup>th</sup>";
                    break;
            }
        }
        string from = weekDay.ToString("dddd * MMMM yyyy");
        from = from.Replace("*", ordinalDay);

        return from;
    }

    public async Task LoadAsync()
    {
        if (loaded) return;

        IEnumerable<Location> locations = await _locationsRepository.GetAllLocationsAsync();

        foreach (Location location in locations)
        {
            Locations.Add(location);
        }

        loaded = true;
    }

    [GeneratedRegex(@"First line of address\r\n|Second line of address\r\n|Town/City\r\n|County\r\n|Postcode\r\n", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-GB")]
    private static partial Regex AddressReformat();
}
