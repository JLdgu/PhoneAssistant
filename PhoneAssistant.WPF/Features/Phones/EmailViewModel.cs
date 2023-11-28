using System.Text;

using CommunityToolkit.Mvvm.ComponentModel;

namespace PhoneAssistant.WPF.Features.Phones;

public partial class EmailViewModel : ObservableObject
{
    public void GenerateEmail(string imei, string phoneNumber)
    {
        Imei = imei;
        PhoneNumber = phoneNumber;
        EmailHtml = GenerateEmailHtml();
    }

    [ObservableProperty]
    private string? _imei;

    [ObservableProperty]
    private string? _phoneNumber;

    [ObservableProperty]
    private string? _emailHtml;

    public string GenerateEmailHtml()
    {
        //bodyText.AppendLine($"Mobile Phone Type: {_phone.OEM} {_phone.Model}");
        StringBuilder html = new StringBuilder("<span style=\"font-size:12px; font-family:Verdana;\">");
        html.AppendLine("  <p>[user name]’s phone has been sent to [address]</p>");

        string span = """
            <span style="font-size:12px; font-family:Verdana;">
                <p>[user name]’s phone has been sent to [address]</p>
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
            <table style="font-size:12px;font-family: Verdana, Arial, Times, serif;">
                <tr><th>Order Details</th><th></th></tr>
                <tr><td>Order type:</td><td>{OrderType}</td></tr>
                <tr><td>Phone supplied:</td><td>{PhoneSupplied}</td></tr>
                <tr><td>Handset identifier:</td><td>{Imei} </td></tr>
                <tr><td>Asset tag:</td><td>{Asset Tag}</td></tr>
                <tr><td>Phone number:</td><td>{</td></tr>
            </table>
            """;
         
        return span; 
    }
}