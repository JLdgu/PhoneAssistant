using System.Text;
using System.Threading.Channels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using PhoneAssistant.WPF.Application.Entities;

namespace PhoneAssistant.WPF.Features.Phones;

public partial class EmailViewModel : ObservableObject
{
    private v1Phone? _phone;

    public void SetupEmail(v1Phone phone)
    {
        _phone = phone ?? throw new ArgumentNullException(nameof(phone));
        Imei = phone.Imei;
        PhoneNumber = phone.PhoneNumber;
        AssetTag = phone.AssetTag;

        GeneratingEmail = true;
        GenerateEmailHtml();
    }

    [ObservableProperty]
    private string? _imei;

    [ObservableProperty]
    private string? _phoneNumber;

    [ObservableProperty]
    private string? _assetTag;

    [ObservableProperty]
    private OrderType _orderType;

    partial void OnOrderTypeChanged(OrderType value)
    {
        GenerateEmailHtml();
    }

    [ObservableProperty]
    private DespatchMethod _despatchMethod;

    partial void OnDespatchMethodChanged(DespatchMethod value)
    {
        GenerateEmailHtml();
    }

    [RelayCommand]
    private void Close()
    {
        GeneratingEmail = false;
    }

    [ObservableProperty]
    private string? _deliveryAddress;

    [ObservableProperty]
    private bool _generatingEmail;

    [ObservableProperty]
    private string? _emailHtml;

    public void GenerateEmailHtml()
    {
        if (_phone is null) throw new NullReferenceException();

        StringBuilder html = new StringBuilder("<span style=\"font-size:12px; font-family:Verdana;\">");

        switch (DespatchMethod)
        {
            case DespatchMethod.CollectGMH:
                html.AppendLine("<p>Your phone can be collected from</p>");
                html.AppendLine("<p>DTS End User Compute Team, Hardware Room, Great Moor House, Bittern Road, Exeter, EX2 7FW</p>");
                html.AppendLine($"<p>It will be available for collection from {ToOrdinalWorkingDate(DateTime.Now.AddDays(2))}</p><p>&nbsp;</p>");
                html.AppendLine("<p>&nbsp;</p>");
                break;
            case DespatchMethod.CollectL87:
                html.AppendLine("<p>Your phone can be collected from</p>");
                html.AppendLine("<p>DTS End User Compute Team, Room L87, County Hall, Topsham Road, Exeter, EX2 4QD</p>");
                html.AppendLine($"<p>It will be available for collection from {ToOrdinalWorkingDate(DateTime.Now)}</p><p>&nbsp;</p>");
                html.AppendLine("<p>&nbsp;</p>");
                break;
            case DespatchMethod.Delivery:
                html.AppendLine("<p>Your phone has been sent to [address]</p>");
                html.AppendLine($"<p>It was sent on {ToOrdinalWorkingDate(DateTime.Now)}</p>");
                break;
        }
        html.AppendLine("<p>&nbsp;</p>");

        html.AppendLine("<p>To find out how to set up your phone, please go here:</p>");
        if (_phone.OEM == "Apple")
        {
            html.Append(@"<p><a href=""https://devoncc.sharepoint.com/sites/ICTKB/Public/DCC%20Mobile%20Phone%20Service%20-%20Setting%20up%20Apple%20(iOS)%20Smartphone.docx?d=w5a23e7d6e2404401a5039a4936743875"">
            Setting up your Apple (iOS) Smartphone.docx (devoncc.sharepoint.com)</a></p>");
        }
        else
        {
            html.AppendLine(@"<p><a href=""https://devoncc.sharepoint.com/:w:/r/sites/ICTKB/Public/Android%20Enterprise%20-%20Setting%20up%20your%20Android%20Phone.docx?d=w64bb3f0a09e44557a64bb78311ee513b&csf=1&web=1"">
            Android Enterprise - Setting up your Android Smartphone.docx (devoncc.sharepoint.com)</a></p>");
        }
        html.AppendLine("<p>On many sites DCC Wi-Fi no longer allows setup / registration of phones. </p>");
        html.AppendLine("<p>To setup the phone either use Gov Wi-Fi, tether the phone to another phone, setup at another site or setup at home.</p>");
        html.AppendLine("<p>&nbsp;</p>");

        if (OrderType == OrderType.Replacement)
        {
            html.AppendLine("<p>Don't forget to transfer your old sim to the replacement phone before returning the old phone to");
            html.AppendLine("DTS End User Compute, Room L87, County Hall, Topsham Road, Exeter, EX2 4QD</p>");
            html.AppendLine(@"<p>You can use <a href=""https://www.royalmail.com/track-my-return#/details/5244"">Royal Mail Tracked Returns for DCC</a>");
            html.AppendLine("to have the phone picked up from your home or you can drop off the item at a Parcel Post Box, Delivery Office or Post Office branch.</p>");
            html.AppendLine("<p>&nbsp;</p>");
        }
        html.AppendLine("</span>");


        html.AppendLine("<table style=\"font-size:12px;font-family: Verdana, Arial, Times, serif;\">");
        html.AppendLine("<tr><th>Order Details</th><th></th></tr>");
        if (OrderType == OrderType.New)
            html.AppendLine("<tr><td>Order type:</td><td>New</td></tr>");
        else
            html.AppendLine("<tr><td>Order type:</td><td>Replacement</td></tr>");

        html.Append("<tr><td>Phone supplied:</td><td>");
        switch (_phone!.NorR)
        {
            case "N":
                html.Append("New");
                break;
            case "R":
                html.Append("Repurposed");
                break;
        }
        html.AppendLine($" {_phone.OEM} {_phone.Model}</td></tr>");

        html.AppendLine($"<tr><td>Handset identifier:</td><td>{Imei}</td></tr>");
        html.AppendLine($"<tr><td>Asset tag:</td><td>{AssetTag}</td></tr>");
        html.AppendLine($"<tr><td>Phone number:</td><td>{PhoneNumber}</td></tr>\r\n</table>");

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
                ordinalDay = number.ToString() + "th";
                break;
        }

        if (ordinalDay == string.Empty)
        {
            switch (number % 10)
            {
                case 1:
                    ordinalDay = number.ToString() + "st";
                    break;
                case 2:
                    ordinalDay = number.ToString() + "nd";
                    break;
                case 3:
                    ordinalDay = number.ToString() + "rd";
                    break;
                default:
                    ordinalDay = number.ToString() + "th";
                    break;
            }
        }
        string from = weekDay.ToString("dddd * MMMM yyyy");
        from = from.Replace("*", ordinalDay);

        return from;
    }
}

public enum OrderType
{
    None = 0,
    New = 1,
    Replacement
}

public enum DespatchMethod
{
    None = 0,
    CollectGMH = 1,
    CollectL87 = 2,
    Delivery = 3
}

