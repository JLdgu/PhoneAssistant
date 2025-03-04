using PhoneAssistant.WPF.Application.Entities;
using PhoneAssistant.WPF.Shared;

namespace PhoneAssistant.Tests.Shared;
public sealed class OrderDetailsTests
{

    [Test]
    public async Task New_Builds_EnvelopeTextAsync()
    {
        Phone phone = new()
        {
            Condition = "norr",
            Imei = "imei",
            Model = "model",
            OEM = OEMs.Apple,
            Status = "status"
        };

        OrderDetails sut = new(phone);

        await Assert.That(sut.EnvelopeText).Contains("Order type:");
        await Assert.That(sut.EnvelopeText).Contains(sut.OrderedItem);

    }

    [Test]
    [Arguments("iPad", DeviceType.Tablet)]
    [Arguments("iPhone", DeviceType.Phone)]
    [Arguments("A32", DeviceType.Phone)]
    public async Task New_PhoneModel_Sets_DeviceTypeAsync(string model, DeviceType deviceType)
    {
        Phone phone = new()
        {
            Condition = "norr",
            Imei = "imei",
            Model = model,
            OEM = OEMs.Apple,
            Status = "status"
        };

        OrderDetails sut = new(phone);

        await Assert.That(sut.DeviceType).IsEqualTo(deviceType);
    }

    [Test]
    [Arguments(null, OrderType.Replacement)]
    [Arguments("0123456789", OrderType.New)]
    public async Task New_PhoneNumber_Sets_OrderTypeAsync(string? phoneNumber, OrderType orderType)
    {
        Phone phone = new()
        {
            Condition = "norr",
            Imei = "imei",
            Model = "model",
            OEM = OEMs.Apple,
            PhoneNumber = phoneNumber,
            Status = "status"
        };

        OrderDetails sut = new(phone);

        await Assert.That(sut.OrderType).IsEqualTo(orderType);
    }
}
