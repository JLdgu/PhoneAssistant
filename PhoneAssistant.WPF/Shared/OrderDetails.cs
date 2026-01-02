using PhoneAssistant.Model;
using System.ComponentModel;
using System.Text;

namespace PhoneAssistant.WPF.Shared;

public sealed record class Order(OrderDetails OrderDetails);

public sealed class OrderDetails
{
    public string AssetTag { get; } = string.Empty;
    
    public string DeliveryAddress { get; set; } = string.Empty;

    public string DeviceSupplied
    {
        get
        {
            return Phone.Condition == "N" ? $"New {Phone.OEM} {Phone.Model}" : $"Repurposed {Phone.OEM} {Phone.Model}";
        }
    }

    public DeviceType DeviceType { get; }

    public string EmailText { get; private set; } = string.Empty;

    public string EnvelopeInsertText { get; private set; } = string.Empty;

    public string Imei { get; }

    public OrderType OrderType { get; set; } = OrderType.None;

    public Phone Phone { get; }
    
    public string PhoneNumber { get; }
    
    public string Ticket { get; } = string.Empty;

    public OrderDetails(Phone phone)
    {
        Phone = phone ?? throw new ArgumentNullException(nameof(phone));
        AssetTag = phone.AssetTag ?? string.Empty;
        if (phone.DespatchDetails is null)
        {
            StringBuilder user = new();
            user.AppendLine(phone.NewUser!);
            DeliveryAddress = user.ToString();
        }
        else
            DeliveryAddress = phone.DespatchDetails;

        Imei = phone.Imei;
        PhoneNumber = phone.PhoneNumber ?? string.Empty;
        Ticket = phone.Ticket.ToString() ?? string.Empty;

        DeviceType = DeviceType.Phone;
        if (phone.Model is not null)
        {
            if (phone.Model.Contains("ipad", StringComparison.InvariantCultureIgnoreCase))
                DeviceType = DeviceType.Tablet;
            else if (phone.Model.Contains("SIM", StringComparison.InvariantCultureIgnoreCase))
                DeviceType = DeviceType.SIM;
        }

        OrderType = Phone.PhoneNumber is null ? OrderType.Replacement : OrderType.New;
    }

    public void Execute(Location? location)
    {
        location ??= new() { Name = string.Empty, Address = string.Empty, Collection = false };

        StringBuilder html = new(Application.Constants.Email_Main_Boilerplate);
        if (location.Collection)
        {
            html.AppendLine($"<p>{Phone.NewUser} your {Phone.OEM} {Phone.Model} {DeviceType.ToString().ToLower()} can be collected from</br>");
            if (location.Name.Contains("GMH"))
            {
                html.AppendLine("DTS End User Compute Team, Hardware Room, Great Moor House, Bittern Road, Exeter, EX2 7FW</br>");
                html.AppendLine($"It will be available for collection from {ToOrdinalWorkingDate(DateTime.Now, buffer: 2)}</p>");
            }
            else
            {
                html.AppendLine("DTS End User Compute Team, Room L87, County Hall, Topsham Road, Exeter, EX2 4QD</br>");
                html.AppendLine($"It will be available for collection from {ToOrdinalWorkingDate(DateTime.Now)}</p>");
            }
        }
        else
        {
            string formattedAddress = string.IsNullOrWhiteSpace(Phone.DespatchDetails) ? string.Empty : Phone.DespatchDetails.Replace(Environment.NewLine, "<br />");
            html.AppendLine($"<p>Your {Phone.OEM} {Phone.Model} {DeviceType.ToString().ToLower()} has been sent to<br />{formattedAddress}</br>");
            html.AppendLine($"It was sent on {ToOrdinalWorkingDate(DateTime.Now)}</p>");
        }

        if (location.Note is not null)
            html.AppendLine(location.Note);

        if (Phone.OEM != Manufacturer.Nokia)
        {
            html.AppendLine(Application.Constants.Mobile_Device_DataUsage_Guidance_And_Policy);

            html.AppendLine(Application.Constants.Register_With_GovWiFi);

            html.AppendLine($"<p><br />Detailed setup instructions for your {DeviceType.ToString().ToLower()}, are available here:</br>");
            if (Phone.OEM == Manufacturer.Apple)
            {
                html.Append(Application.Constants.Setup_iOS_Device);
            }
            else
            {
                html.AppendLine(Application.Constants.Setup_Android_Device);
            }
        }

        if (OrderType == OrderType.Replacement && DeviceType == DeviceType.Phone)
        {
            html.AppendLine(Application.Constants.Transfer_SIM_Return_Old_Device);
        }

        html.AppendLine(Application.Constants.Email_Table_Boilerplate);

        StringBuilder envelopeText = new();
        if (Phone.Ticket > 999999)
            envelopeText.AppendLine($"Issue:\t#{Ticket}");
        else
            envelopeText.AppendLine($"Service Request:\t#{Ticket}");

        envelopeText.AppendLine("");
        envelopeText.AppendLine($"Device User:\t{Phone.NewUser}");
        envelopeText.AppendLine("");
        envelopeText.AppendLine($"Order type:\t{OrderType} {DeviceType}");
        html.AppendLine($"<tr><td>Order type:</td><td>{OrderType} {DeviceType}</td></tr>");
        envelopeText.AppendLine("");
        if (DeviceType != DeviceType.SIM)
        {
            envelopeText.AppendLine($"Device supplied:\t{DeviceSupplied}");
            html.AppendLine($"<tr><td>Device supplied:</td><td>{DeviceSupplied}</td></tr>");
            envelopeText.AppendLine("");
            envelopeText.AppendLine($"Handset identifier:\t{Imei}");
            html.AppendLine($"<tr><td>Handset identifier:</td><td>{Imei}</td></tr>");
            envelopeText.AppendLine("");
            envelopeText.AppendLine($"Asset Tag:\t{AssetTag}");
            html.AppendLine($"<tr><td>Asset tag:</td><td>{AssetTag}</td></tr>");
            envelopeText.AppendLine("");
        }
        if (!string.IsNullOrWhiteSpace(PhoneNumber))
        {
            envelopeText.AppendLine($"Phone number:\t{PhoneNumber}");
            html.AppendLine($"<tr><td>Phone number:</td><td>{PhoneNumber}</td></tr></table>");
            envelopeText.AppendLine("");
            envelopeText.AppendLine($"SIM:\t{Phone.SimNumber}");
        }

        EmailText = html.ToString();
        EnvelopeInsertText = envelopeText.ToString();
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
                ordinalDay = hexSuperscript ? number.ToString() + "\x1D57\x02B0" : number.ToString() + "<sup>th</sup>";
                break;
        }

        if (ordinalDay == string.Empty)
        {
            switch (number % 10)
            {
                case 1:
                    ordinalDay = hexSuperscript ? number.ToString() + "\x02E2\x1D57" : number.ToString() + "<sup>st</sup>";
                    break;
                case 2:
                    ordinalDay = hexSuperscript ? number.ToString() + "\x207F\x1D48" : number.ToString() + "<sup>nd</sup>";
                    break;
                case 3:
                    ordinalDay = hexSuperscript ? number.ToString() + "\x02B3\x1D48" : number.ToString() + "<sup>rd</sup>";
                    break;
                default:
                    ordinalDay = hexSuperscript ? number.ToString() + "\x1D57\x02B0" : number.ToString() + "<sup>th</sup>";
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

public enum DeviceType
{
    None = 0,
    Phone = 1,
    Tablet = 2,
    [Description("SIM Card")]
    SIM
}
