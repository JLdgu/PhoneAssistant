using PhoneAssistant.Model;
using PhoneAssistant.WPF.Shared;

namespace PhoneAssistant.Tests.Shared;

public sealed class OrderDetailsTests
{
    private readonly Phone _phone;
    public OrderDetailsTests()
    {
        _phone = new()
        {
            Condition = "norr",
            Imei = "imei",
            Model = "model",
            OEM = Manufacturer.Apple,
            Status = "status",
            Ticket = 1000000
        };
    }

    [Test]
    [Arguments(null, "")]
    [Arguments("phonenumber", "phonenumber")]
    public async Task Execute_Sets_Properties(string? phoneNumber, string expected)
    {
        _phone.PhoneNumber = phoneNumber;
        OrderDetails sut = new(_phone);

        sut.Execute();

        await Assert.That(sut.Imei).IsEqualTo("imei");
        await Assert.That(sut.PhoneNumber).IsEqualTo(expected);
    }
    [Test]
    public async Task Execute_Generates_EmailText_BoilerPlate()
    {
        OrderDetails sut = new(_phone);

        sut.Execute();

        await Assert.That(sut.EmailText).StartsWith(
            """
            <span style="font-size:14px; font-family:Verdana;">
            """);
        await Assert.That(sut.EmailText).Contains("</span>");
        await Assert.That(sut.EmailText).Contains(@"<p><br />Before setting up your phone please ensure you register with <a href=""https://www.wifi.service.gov.uk/connect-to-govwifi/"">GovWifi</a></p>");
    }

    [Test]
    [Arguments("iPad", DeviceType.Tablet)]
    [Arguments("iPhone", DeviceType.Phone)]
    [Arguments("A32", DeviceType.Phone)]
    [Arguments("eSIM", DeviceType.SIM)]
    [Arguments("SIM", DeviceType.SIM)]
    public async Task PhoneModel_Sets_DeviceType(string model, DeviceType deviceType)
    {
        _phone.Model = model;
        OrderDetails sut = new(_phone);

        sut.Execute();

        await Assert.That(sut.DeviceType).IsEqualTo(deviceType);
    }

    [Test]
    [Arguments(null, OrderType.Replacement)]
    [Arguments("0123456789", OrderType.New)]
    public async Task PhoneNumber_Sets_OrderType(string? phoneNumber, OrderType orderType)
    {
        _phone.PhoneNumber = phoneNumber;
        OrderDetails sut = new(_phone);

        sut.Execute();

        await Assert.That(sut.OrderType).IsEqualTo(orderType);
    }

    [Test]
    [Arguments(123456,"Service Request")]
    [Arguments(1234567,"Issue")]
    public async Task Ticket_Length_Determines_Issue_or_ServiceRequest(int ticket,string label)
    {
        _phone.Ticket = ticket;
        OrderDetails sut = new(_phone);

        sut.Execute();
        
        await Assert.That(sut.EnvelopeInsertText).IsEqualTo($"{label}:\t#{ticket}");        
    }
}
