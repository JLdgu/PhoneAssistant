using FluentAssertions;

using PhoneAssistant.WPF.Application.Entities;
using PhoneAssistant.WPF.Shared;

using Xunit;

namespace PhoneAssistant.Tests.Shared;
public sealed class OrderDetailsTests
{

    [Fact]
    public void New_Builds_EnvelopeText()
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

        sut.EnvelopeText.Should().Contain("Order type:");
        sut.EnvelopeText.Should().Contain(sut.OrderedItem);

    }

    [Theory]
    [InlineData("iPad", DeviceType.Tablet)]
    [InlineData("iPhone", DeviceType.Phone)]
    [InlineData("A32", DeviceType.Phone)]
    public void New_PhoneModel_Sets_DeviceType(string model, DeviceType deviceType)
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

        sut.DeviceType.Should().Be(deviceType);
    }

    [Theory]
    [InlineData(null, OrderType.Replacement)]
    [InlineData("0123456789", OrderType.New)]
    public void New_PhoneNumber_Sets_OrderType(string? phoneNumber, OrderType orderType)
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

        sut.OrderType.Should().Be(orderType);
    }
}
