using PhoneAssistant.WPF.Application.Entities;

namespace PhoneAssistant.WPF.Features.Phones;

public sealed record class Order(OrderDetails OrderDetails);

public sealed class OrderDetails
{
    public Phone Phone { get; }

    public OrderDetails(Phone phone)
    {
        Phone = phone;
    }

    public OrderType OrderType { get; set; } = OrderType.New;
    public DeviceType DeviceType { get; set; } = DeviceType.Phone;    
    public DespatchMethod DespatchMethod { get; set; } = DespatchMethod.CollectL87;

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

public enum DespatchMethod
{
    None = 0,
    CollectGMH = 1,
    CollectL87 = 2,
    Delivery = 3
}
