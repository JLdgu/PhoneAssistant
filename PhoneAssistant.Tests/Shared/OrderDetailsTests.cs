using PhoneAssistant.Model;
using PhoneAssistant.WPF.Shared;
using System.ComponentModel;
using System.Text;

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
    [Arguments(OrderType.New)]
    [Arguments(OrderType.Replacement)]
    public async Task Execute_OrderType_Email_Contains_Order_Type_Details(OrderType orderType)
    {
        OrderDetails sut = new(_phone)
        {
            OrderType = orderType
        };

        sut.Execute(null);

        await Assert.That(sut.EmailText).Contains($"<td>Order type:</td><td>{orderType}");
    }

    [Test]
    [Arguments(null, "", OrderType.Replacement)]
    [Arguments("phonenumber", "phonenumber", OrderType.New)]
    public async Task Execute_Sets_Properties(string? phoneNumber, string expected, OrderType orderType)
    {
        _phone.PhoneNumber = phoneNumber;
        OrderDetails sut = new(_phone);

        sut.Execute(null);

        await Assert.That(sut.AssetTag).IsEqualTo("");
        await Assert.That(sut.Imei).IsEqualTo("imei");
        await Assert.That(sut.OrderType).IsEqualTo(orderType);
        await Assert.That(sut.PhoneNumber).IsEqualTo(expected);
        await Assert.That(sut.Ticket).IsEqualTo("1000000");
    }

    [Test]
    public async Task Execute_After_Constructor_Email_Contains_BoilerPlate()
    {
        OrderDetails sut = new(_phone);

        sut.Execute(null);

        await Assert.That(sut.EmailText).StartsWith(WPF.Application.Constants.Email_Main_Boilerplate);
        await Assert.That(sut.EmailText).Contains(WPF.Application.Constants.Register_With_GovWiFi);
        await Assert.That(sut.EmailText).Contains("<p><br /></p></span>");
        await Assert.That(sut.EmailText).Contains(WPF.Application.Constants.Email_Table_Boilerplate);
    }

    [Test]
    public async Task Execute_When_Collection_False_Email_Contains_DeliveryDetails()
    {
        OrderDetails sut = new(_phone);
        Location location = new() { Name = "name", Address = "address", Collection = false };

        sut.Execute(location);

        await Assert.That(sut.EmailText).Contains("has been sent to<br />");
        await Assert.That(sut.EmailText).Contains("It was sent on");
    }

    [Test]
    public async Task Execute_When_Collection_True_Email_Contains_CollectionDetails()
    {
        _phone.NewUser = "new user";
        OrderDetails sut = new(_phone);
        Location location = new() { Name = "name", Address = "address", Collection = true };

        sut.Execute(location);

        await Assert.That(sut.EmailText).Contains($"<p>{sut.Phone.NewUser} your");
        await Assert.That(sut.EmailText).Contains("can be collected from</br>");
        await Assert.That(sut.EmailText).Contains("It will be available for collection from");
    }

    [Test]
    public async Task Execute_When_Collection_True_And_Location_GMH_Email_Contains_GMHDetails()
    {
        OrderDetails sut = new(_phone);
        Location location = new() { Name = "GMH", Address = "address", Collection = true };

        sut.Execute(location);

        await Assert.That(sut.EmailText).Contains("Great Moor House");
    }

    [Test]
    public async Task Execute_When_Collection_True_And_Location_L87_Email_Contains_CountyHallDetails()
    {
        OrderDetails sut = new(_phone);
        Location location = new() { Name = "name", Address = "address", Collection = true };

        sut.Execute(location);

        await Assert.That(sut.EmailText).Contains("County Hall");
    }

    [Test]
    public async Task Execute_When_DespatchDetails_NotNull_SetsDeliveryAddress_To_DespatchDetails()
    {
        _phone.DespatchDetails = "despatch details";
        _phone.NewUser = "new user";
        OrderDetails sut = new(_phone);

        sut.Execute(null);

        await Assert.That(sut.DeliveryAddress).IsEqualTo("despatch details");
    }

    [Test]
    public async Task Execute_When_DespatchDetails_Null_SetsDeliveryAddress_To_User()
    {
        _phone.DespatchDetails = null;
        _phone.NewUser = "new user";
        OrderDetails sut = new(_phone);
        StringBuilder expected = new();
        expected.AppendLine(_phone.NewUser!);

        sut.Execute(null);

        await Assert.That(sut.DeliveryAddress).IsEqualTo(expected.ToString());
    }

    [Test]
    public async Task Execute_When_Location_NotNull_Email_Contains_Note()
    {
        OrderDetails sut = new(_phone);
        Location location = new() { Name = "name", Address = "address", Collection = true, Note = "**note**" };

        sut.Execute(location);

        await Assert.That(sut.EmailText).Contains("**note**");
    }

    [Test]
    public async Task Execute_When_OEM_Apple_Email_Contains_AppleDetails()
    {

        _phone.OEM = Manufacturer.Apple;
        OrderDetails sut = new(_phone);

        sut.Execute(null);

        await Assert.That(sut.EmailText).Contains(WPF.Application.Constants.Mobile_Device_DataUsage_Guidance_And_Policy);
        await Assert.That(sut.EmailText).Contains(WPF.Application.Constants.Setup_iOS_Device);
    }

    [Test]
    [Description("Issue 43")]
    public async Task Execute_When_OEM_Nokia_Email_Contains_NokiaDetails()
    {
        _phone.OEM = Manufacturer.Nokia;
        OrderDetails sut = new(_phone);

        sut.Execute(null);

        await Assert.That(sut.EmailText).DoesNotContain(WPF.Application.Constants.Mobile_Device_DataUsage_Guidance_And_Policy);
        await Assert.That(sut.EmailText).DoesNotContain("Smartphone");
    }

    [Test]
    public async Task Execute_When_OEM_Samsung_Email_Contains_SamsungDetails()
    {
        _phone.OEM = Manufacturer.Samsung;
        OrderDetails sut = new(_phone);

        sut.Execute(null);

        await Assert.That(sut.EmailText).Contains(WPF.Application.Constants.Mobile_Device_DataUsage_Guidance_And_Policy);
        await Assert.That(sut.EmailText).Contains(WPF.Application.Constants.Setup_Android_Device);
    }

    [Test]
    public async Task Execute_When_OrderType_New_Email_DoesNot_Contain_TransferSIM()
    {
        OrderDetails sut = new(_phone)
        {
            OrderType = OrderType.New
        };

        sut.Execute(null);

        await Assert.That(sut.EmailText).DoesNotContain(WPF.Application.Constants.Transfer_SIM_Return_Old_Device);
    }

    [Test]
    public async Task Execute_When_OrderType_Replacement_Email_Contains_TransferSIM()
    {
        OrderDetails sut = new(_phone)
        {
            OrderType = OrderType.Replacement
        };

        sut.Execute(null);

        await Assert.That(sut.EmailText).Contains(WPF.Application.Constants.Transfer_SIM_Return_Old_Device);
    }

    [Test]
    [Arguments("N", "New")]
    [Arguments("R", "Repurposed")]
    public async Task Execute_With_Phone_Condition_Changes_Email_Device_Supplied_Details(string condition, string conditionDescription)
    {
        _phone.Condition = condition;
        OrderDetails sut = new(_phone);

        sut.Execute(null);

        await Assert.That(sut.EmailText).Contains($"<td>Device supplied:</td><td>{conditionDescription} {_phone.OEM} {_phone.Model}</td>");
    }

    [Test]
    public async Task Execute_With_PhoneNumber_Null_Email_Excludes_Phone_Number_Details()
    {
        _phone.PhoneNumber = null;
        OrderDetails sut = new(_phone);

        sut.Execute(null);

        await Assert.That(sut.EmailText).DoesNotContain($"<tr><td>Phone number:</td><td>");
    }

    [Test]
    public async Task Execute_With_PhoneNumber_Email_Excludes_Phone_Number_Details()
    {
        _phone.PhoneNumber = "phone number";
        OrderDetails sut = new(_phone);

        sut.Execute(null);

        await Assert.That(sut.EmailText).Contains($"<tr><td>Phone number:</td><td>{_phone.PhoneNumber}</td></tr></table>");
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

        sut.Execute(null);

        await Assert.That(sut.DeviceType).IsEqualTo(deviceType);
    }

    [Test]
    [Arguments(123456, "Service Request")]
    [Arguments(1234567, "Issue")]
    public async Task Ticket_Length_Determines_Issue_or_ServiceRequest(int ticket, string label)
    {
        _phone.Ticket = ticket;
        OrderDetails sut = new(_phone);

        sut.Execute(null);

        await Assert.That(sut.EnvelopeInsertText).Contains($"{label}:\t#{ticket}");
    }    
}
