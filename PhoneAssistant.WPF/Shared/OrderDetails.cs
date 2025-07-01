using PhoneAssistant.Model;
using System.ComponentModel;
using System.Text;

namespace PhoneAssistant.WPF.Shared;

public sealed record class Order(OrderDetails OrderDetails);

public sealed class OrderDetails
{
    public Phone Phone { get; }

    public OrderDetails(Phone phone)
    {
        Phone = phone;

        DeviceType = DeviceType.Phone;
        if (Phone.Model != null)
        {
            if (Phone.Model.Contains("ipad", StringComparison.InvariantCultureIgnoreCase))
                DeviceType = DeviceType.Tablet;
            else if (Phone.Model.Contains("SIM", StringComparison.InvariantCultureIgnoreCase))
                DeviceType = DeviceType.SIM;
        }

        OrderType = Phone.PhoneNumber is null ? OrderType.Replacement : OrderType.New;

        StringBuilder envelopeText = new();
        if (Phone.SR > 999999)
            envelopeText.AppendLine($"Issue:\t#{Phone.SR}");
        else
            envelopeText.AppendLine($"Service Request:\t#{Phone.SR}");

        envelopeText.AppendLine("");
        envelopeText.AppendLine($"Device User:\t{Phone.NewUser}");
        envelopeText.AppendLine("");
        envelopeText.AppendLine($"Order type:\t{OrderedItem}");
        envelopeText.AppendLine("");
        if (DeviceType != DeviceType.SIM)
        {
            envelopeText.AppendLine($"Device supplied:\t{DeviceSupplied}");
            envelopeText.AppendLine("");
            envelopeText.AppendLine($"Handset identifier:\t{Phone.Imei}");
            envelopeText.AppendLine("");
            envelopeText.AppendLine($"Asset Tag:\t{Phone.AssetTag}");
            envelopeText.AppendLine("");
        }
        if (!string.IsNullOrWhiteSpace(Phone.PhoneNumber))
        {
            envelopeText.AppendLine($"Phone number:\t{Phone.PhoneNumber}");
            envelopeText.AppendLine("");
            envelopeText.AppendLine($"SIM:\t{Phone.SimNumber}");
        }
        EnvelopeText = envelopeText.ToString();
    }

    public string DeviceSupplied
    {
        get
        {
            return Phone.Condition == "N" ? $"New {Phone.OEM} {Phone.Model}" : $"Repurposed {Phone.OEM} {Phone.Model}";
        }
    }

    public DeviceType DeviceType { get; }

    public string EnvelopeText { get; init; }

    public string OrderedItem
    {
        get
        {
            return $"{OrderType} {DeviceType}";
        }
    }

    public OrderType OrderType { get; set; } = OrderType.None;
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
