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
    private bool _generatingEmail;


    [ObservableProperty]
    private string? _emailHtml;

    public void GenerateEmailHtml()
    {
        if (_phone is null) throw new NullReferenceException();

        StringBuilder html = new StringBuilder("<span style=\"font-size:12px; font-family:Verdana;\">");
        html.AppendLine("<p>[user name]’s phone has been sent to [address]</p>");

        switch (DespatchMethod)
        {
            case DespatchMethod.CollectGMH:
                html.AppendLine("<p>Your phone can be collected from</p>");
                html.AppendLine("<p>DTS End User Compute Team, Hardware Room, Great Moor House, Bittern Road, Exeter, EX2 7FW</p>");
                break;
            case DespatchMethod.CollectL87:
                html.AppendLine("<p>Your phone can be collected from</p>");
                html.AppendLine("<p>DTS End User Compute Team, Room L87, County Hall, Topsham Road, Exeter, EX2 4QD</p>");
                break;
            case DespatchMethod.Delivery:
                break;
        }

        html.AppendLine("<table style=\"font-size:12px;font-family: Verdana, Arial, Times, serif;\">");
        html.AppendLine("<tr><th>Order Details</th><th></th></tr>");
        switch (OrderType)
        {
            case OrderType.New:
                html.AppendLine("<tr><td>Order type:</td><td>New</td></tr>");
                break;
            case OrderType.Replacement:
                html.AppendLine("<tr><td>Order type:</td><td>Replacement</td></tr>");
                break;
        }

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

        string span = """
                <p>It was sent on [date]</p>
                <p>&nbsp;</p>

                <p>Your phone can be collected from</p>
                <p>DTS End User Compute Team, Hardware Room, Great Moor House, Bittern Road, Exeter, EX2 7FW</p>
                <p>DTS End User Compute Team, Room L87, County Hall, Topsham Road, Exeter, EX2 4QD</p>
                <p>It will be available for collection from [date]</p>
                <p>&nbsp;</p>

                <p>To find out how to set up your phone, please go here:</p>
                <p><a href="https://devoncc.sharepoint.com/:w:/r/sites/ICTKB/_layouts/15/Doc.aspx?sourcedoc=%7B64BB3F0A-09E4-4557-A64B-B78311EE513B%7D&amp;file=Android%20Enterprise%20-%20Setting%20up%20your%20Android%20Phone.docx&amp;action=default&amp;mobileredirect=true">
                        Android Enterprise - Setting up your Android Smartphone.docx (devoncc.sharepoint.com)</a></p>
                <!--
                    <p><a href = "https://devoncc.sharepoint.com/sites/ICTKB/Public/DCC%20Mobile%20Phone%20Service%20-%20Setting%20up%20Apple%20(iOS)%20Smartphone.docx?d=w5a23e7d6e2404401a5039a4936743875">
                        Setting up your Apple (iOS) Smartphone.docx (devoncc.sharepoint.com)</a></p>
                    -->
                <p>On many sites DCC Wi-Fi no longer allows setup / registration of phones. </p>
                <p>To setup the phone either use Gov Wi-Fi, tether the phone to another phone, setup at another site or setup at home.</p>
                <p>&nbsp;</p>
                <p>Don't forget to transfer your old sim to the replacement phone before returning the old phone to
                    DTS End User Compute, Room L87, County Hall, Topsham Road, Exeter, EX2 4QD</p>

                <p>You can use <a href="https://www.royalmail.com/track-my-return#/details/5244">Royal Mail Tracked Returns for DCC</a>
                    to have the phone picked up from your home or you
                    can drop off the item at a Parcel Post Box, Delivery Office or Post Office branch.</p>
                <p>&nbsp;</p>
            </span>
            """;
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