using System.Text;
using System.Windows;
using System.Windows.Controls;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using PhoneAssistant.WPF.Application.Repositories;

namespace PhoneAssistant.WPF.Features.Phones;

public partial class EmailViewModel(IPhonesRepository phonesRepository, 
                                    IPrintEnvelope printEnvelope) : ObservableObject
{
    private readonly IPhonesRepository _phonesRepository = phonesRepository ?? throw new ArgumentNullException();
    private readonly IPrintEnvelope _printEnvelope = printEnvelope ?? throw new ArgumentNullException();   

    private OrderDetails _orderDetails;
    
    public OrderDetails OrderDetails
    {
        get => _orderDetails;             
        set
        {
            _orderDetails = value;

            Imei = value.Phone.Imei;
            PhoneNumber = value.Phone.PhoneNumber ?? string.Empty;
            AssetTag = value.Phone.AssetTag ?? string.Empty;
            OrderType = value.OrderType;
            DeviceType = value.DeviceType;
            DespatchMethod = value.DespatchMethod;
            DeliveryAddress = value.Phone.NewUser ?? string.Empty;
            if (value.Phone.DespatchDetails is not null)
                DeliveryAddress = value.Phone.DespatchDetails;

            GenerateEmailHtml();
            GeneratingEmail = true;
        }
    }
        
    [ObservableProperty]
    private string _imei = string.Empty;

    [ObservableProperty]
    private string _phoneNumber = string.Empty;

    [ObservableProperty]
    private string _assetTag = string.Empty;

    [ObservableProperty]
    private OrderType _orderType;
    partial void OnOrderTypeChanged(OrderType value)
    {
        OrderDetails.OrderType = OrderType;
        GenerateEmailHtml();
    }

    [ObservableProperty]
    private DeviceType _deviceType;
    partial void OnDeviceTypeChanged(DeviceType value)
    {
        OrderDetails.DeviceType = DeviceType;
        GenerateEmailHtml();
    }

    [ObservableProperty]
    private DespatchMethod _despatchMethod;

    partial void OnDespatchMethodChanged(DespatchMethod value)
    {
        _orderDetails.Phone.Collection = true;
        if (value == DespatchMethod.Delivery)
            _orderDetails.Phone.Collection = false;                
        
        GenerateEmailHtml();
    }

    [RelayCommand]
    private async Task CloseAsync(string? SaveAndCopy)
    {
        GeneratingEmail = false;
        if (SaveAndCopy is null) return;

        await _phonesRepository.UpdateAsync(_orderDetails.Phone);
        //Clipboard.SetText(EmailHtml);
    }

    [RelayCommand]
    private async Task PrintEnvelope()
    {
        await Task.Run(() =>
        {
            _printEnvelope.Execute(_orderDetails);
        });
    }
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(EmailHtml))]
    private string _deliveryAddress = string.Empty;

    partial void OnDeliveryAddressChanged(string value)
    {
        _orderDetails.Phone.DespatchDetails = value;

        _formattedAddress = value.Replace(Environment.NewLine,"<br />");
        GenerateEmailHtml();
    }
    private string _formattedAddress = string.Empty;

    [ObservableProperty]
    private bool _generatingEmail;

    [ObservableProperty]
    private string _emailHtml = string.Empty;

    public void GenerateEmailHtml()
    {
        StringBuilder html = new StringBuilder(
            """
            <span style="font-size:12px; font-family:Verdana;">
            """);

        switch (DespatchMethod)
        {
            case DespatchMethod.CollectGMH:
                html.AppendLine($"<p>Your {DeviceType.ToString().ToLower()} can be collected from</br>");
                html.AppendLine("DTS End User Compute Team, Hardware Room, Great Moor House, Bittern Road, Exeter, EX2 7FW</br>");
                html.AppendLine($"It will be available for collection from {ToOrdinalWorkingDate(DateTime.Now.AddDays(2))}</p>");
                break;
            case DespatchMethod.CollectL87:
                html.AppendLine($"<p>Your {DeviceType.ToString().ToLower()} can be collected from</br>");
                html.AppendLine("DTS End User Compute Team, Room L87, County Hall, Topsham Road, Exeter, EX2 4QD</br>");
                html.AppendLine($"It will be available for collection from {ToOrdinalWorkingDate(DateTime.Now)}");
                break;
            case DespatchMethod.Delivery:
                html.AppendLine($"<p>Your {DeviceType.ToString().ToLower()} has been sent to<br />{_formattedAddress}</br>");
                html.AppendLine($"It was sent on {ToOrdinalWorkingDate(DateTime.Now)}</p>");
                break;
        }
        html.AppendLine(
            """
            <p><br /><a href="https://devoncc.sharepoint.com/:w:/r/sites/ICTKB/Public/DCC%20mobile%20phone%20data%20usage%20guidance%20and%20policies.docx?d=w9ce15b2ddbb343739f131311567dd305&csf=1&web=1"
                  style="font-size:12px; font-family:Verdana";>
            DCC mobile phone data usage guidance and policies</a></p>
            """);

        html.AppendLine($"<p><br />To find out how to set up your {DeviceType.ToString().ToLower()}, please go here:</br>");
        if (_orderDetails.Phone.OEM == "Apple")
        {
            html.Append(
                """
                <a href="https://devoncc.sharepoint.com/sites/ICTKB/Public/DCC%20Mobile%20Phone%20Service%20-%20Setting%20up%20Apple%20(iOS)%20Smartphone.docx?d=w5a23e7d6e2404401a5039a4936743875"
                   style="font-size:12px; font-family:Verdana";>
                Setting up your Apple (iOS) Smartphone.docx (devoncc.sharepoint.com)</a></p>
                """);            
        }
        else
        {
            html.AppendLine(
                """
                <a href="https://devoncc.sharepoint.com/:w:/r/sites/ICTKB/Public/Android%20Enterprise%20-%20Setting%20up%20your%20Android%20Phone.docx?d=w64bb3f0a09e44557a64bb78311ee513b&csf=1&web=1"
                   style="font-size:12px; font-family:Verdana";>
                Android Enterprise - Setting up your Android Smartphone.docx (devoncc.sharepoint.com)</a></p>
                """);
        }
        html.AppendLine("<p>On many sites DCC Wi-Fi no longer allows setup / registration of phones. </br>");
        html.AppendLine("To setup the phone either use Gov Wi-Fi, tether the phone to another phone, setup at another site or setup at home.</p>");

        var a = OrderType.ToString();

        if (OrderType == OrderType.Replacement && DeviceType == DeviceType.Phone)
        {
            html.AppendLine("<p>Don't forget to transfer your old sim to the replacement phone before returning the old phone to");
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
        html.AppendLine($"<tr><td>Order type:</td><td>{_orderDetails.OrderedItem}</td></tr>");
        html.AppendLine($"<tr><td>Device supplied:</td><td>{_orderDetails.DeviceSupplied}</td></tr>");

        html.AppendLine($"<tr><td>Handset identifier:</td><td>{Imei}</td></tr>");
        html.AppendLine($"<tr><td>Asset tag:</td><td>{AssetTag}</td></tr>");
        if (!string.IsNullOrEmpty(PhoneNumber))
            html.AppendLine($"<tr><td>Phone number:</td><td>{PhoneNumber}</td></tr></table>");
        EmailHtml = html.ToString();
    }

    public static string ToOrdinalWorkingDate(DateTime date)
    {
        int addDays = 0;
        switch (date.DayOfWeek)
        {
            case DayOfWeek.Sunday:
                addDays = 1;
                break;  
            case DayOfWeek.Saturday:
                addDays = 2;
                break;
        }
        DateTime weekDay = date.AddDays(addDays);

        string ordinalDay = string.Empty;
        int number = weekDay.Day;
        switch (number % 100)
        {
            case 11:
            case 12:
            case 13:
                ordinalDay = number.ToString() + "<sup>th></sup>";
                break;
        }

        if (ordinalDay == string.Empty)
        {
            switch (number % 10)
            {
                case 1:
                    ordinalDay = number.ToString() + "<sup>st</sup>";
                    break;
                case 2:
                    ordinalDay = number.ToString() + "<sup>nd</sup>";
                    break;
                case 3:
                    ordinalDay = number.ToString() + "<sup>rd</sup>";
                    break;
                default:
                    ordinalDay = number.ToString() + "<sup>th</sup>";
                    break;
            }
        }        
        string from = weekDay.ToString("dddd * MMMM yyyy");
        from = from.Replace("*", ordinalDay);

        return from;
    }
}

public static class WebBrowserHelper
{
    public static readonly DependencyProperty NavigateToProperty =
        DependencyProperty.RegisterAttached("NavigateTo", typeof(string), typeof(WebBrowserHelper), new UIPropertyMetadata(null, NavigateToPropertyChanged));

    public static string GetNavigateTo(DependencyObject obj)
    {
        return (string)obj.GetValue(NavigateToProperty);
    }

    public static void SetNavigateTo(DependencyObject obj, string value)
    {
        obj.SetValue(NavigateToProperty, value);
    }

    public static void NavigateToPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
        if (o is WebBrowser browser)            
            if (e.NewValue is string html)
                if (!string.IsNullOrEmpty(html))
                    browser.NavigateToString(html);
    }
}

//public enum OrderType
//{
//    None = 0,
//    New = 1,
//    Replacement
//}

//public enum DeviceType
//{
//    None = 0,
//    Phone = 1,
//    Tablet = 2
//}

//public enum DespatchMethod
//{
//    None = 0,
//    CollectGMH = 1,
//    CollectL87 = 2,
//    Delivery = 3
//}

