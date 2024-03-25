using PhoneAssistant.WPF.Application.Entities;

namespace PhoneAssistant.WPF.Features.Phones;

public sealed record class Order(OrderDetails OrderDetails);

public sealed class OrderDetails
{
    public Phone Phone { get; }

    public OrderDetails(Phone phone)
    {
        Phone = phone;

        DeviceType = DeviceType.Phone;
        if (Phone.Model != null && Phone.Model.Contains("ipad",StringComparison.InvariantCultureIgnoreCase))
            DeviceType  = DeviceType.Phone;
    }

    public OrderType OrderType { get; set; } = OrderType.None;
    public DeviceType DeviceType { get; }

    public string OrderedItem
    {
        get
        {
            return $"{OrderType.ToString()} {DeviceType.ToString()}";
        }
    }
    public string DeviceSupplied
    {
        get
        {
            if (Phone.NorR == "N")
                return $"New {Phone.OEM} {Phone.Model}";
            return $"Repurposed {Phone.OEM} {Phone.Model}";            
        }
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
    Tablet = 2
}
