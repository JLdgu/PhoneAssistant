using PhoneAssistant.WPF.Features.Phones;
using PhoneAssistant.WPF.Application.Entities;
using Moq.AutoMock;
using Moq;
using PhoneAssistant.WPF.Application.Repositories;
using System.Text;
using System.ComponentModel;
using FluentAssertions;
using System.Globalization;

namespace PhoneAssistant.Tests.Features.Phones;

public sealed class EmailViewModelTests
{
    private readonly Phone _phone = new()
    {
        PhoneNumber = "phoneNumber",
        SimNumber = "simNumber",
        Status = "status",
        AssetTag = "at",
        DespatchDetails = "dd",
        FormerUser = "fu",
        Imei = "imei",
        Model = "model",
        NewUser = "nu",
        Condition = "norr",
        Notes = "note",
        OEM = OEMs.Apple,
        SR = 123456
    };

    private const string DataUsage = """
            <p><br /><a href="https://devoncc.sharepoint.com/:w:/r/sites/ICTKB/Public/DCC%20mobile%20phone%20data%20usage%20guidance%20and%20policies.docx?d=w9ce15b2ddbb343739f131311567dd305&csf=1&web=1">
            DCC mobile phone data usage guidance and policies</a></p>
            """;

    private readonly AutoMocker _mocker = new AutoMocker();
    private readonly Mock<IPhonesRepository> _phones;

    private readonly EmailViewModel _vm;

    public EmailViewModelTests()
    {
        _phones = _mocker.GetMock<IPhonesRepository>();

        _vm = _mocker.CreateInstance<EmailViewModel>();
    }

    private void TestSetup(Phone phone)
    {
        OrderDetails orderDetails = new(phone);
        _vm.OrderDetails = orderDetails;
    }

    [Test]
    public async Task CloseCommand_SetsGeneratingEmail_FalseAsync()
    {
        TestSetup(_phone);

        _vm.CloseCommand.Execute(null);

        await Assert.That(_vm.GeneratingEmail).IsFalse();
    }

    [Test]
    public void CloseCommand_UpdatesDb()
    {
        TestSetup(_phone);

        _vm.CloseCommand.Execute(null);

        _phones.Verify(r => r.UpdateAsync(_phone), Times.Once);
    }

    [Test]
    public async Task Constructor_SetsGeneratingEmail_TrueAsync()
    {
        TestSetup(_phone);

        await Assert.That(_vm.GeneratingEmail).IsTrue();
    }

    [Test]
    public async Task DefaultPhone_Contains_BoilerPlateAsync()
    {
        TestSetup(_phone);

        await Assert.That(_vm.EmailHtml).Contains(
            """
            <span style="font-size:14px; font-family:Verdana;">
            """);
        await Assert.That(_vm.EmailHtml).Contains("</span>");

        await Assert.That(_vm.EmailHtml).Contains(@"<p><br />Before setting up your phone please ensure you register with <a href=""https://www.wifi.service.gov.uk/connect-to-govwifi/"">GovWifi</a></p>");
    }

    [Test]
    public async Task DespatchDetails_Null_SetsDeliveryAddressAsync()
    {
        _phone.DespatchDetails = null;
        TestSetup(_phone);
        StringBuilder expected = new();
        expected.AppendLine(_phone.NewUser);

        await Assert.That(_vm.DeliveryAddress).IsEqualTo(expected.ToString());
    }

    [Test]
    public void GenerateEmail_ShouldBeCollection_WhenPrintDateTrue()
    {
        TestSetup(_phone);
        Location location = new() { Name = "name", Address = "address", PrintDate = true};
        _vm.SelectedLocation = location;

        _vm.GenerateEmailHtml();

        _vm.EmailHtml.Should().Contain($"<p>Your {_vm.OrderDetails.Phone.OEM} {_vm.OrderDetails.Phone.Model} {_vm.OrderDetails.DeviceType.ToString().ToLower()} can be collected from</br>");
    }

    [Test]
    public void GenerateEmail_ShouldDelivery_WhenPrintDateFalse()
    {
        TestSetup(_phone);
        Location location = new() { Name = "name", Address = "address", PrintDate = false };

        _vm.GenerateEmailHtml();

        _vm.EmailHtml.Should().Contain($"<p>Your {_vm.OrderDetails.Phone.OEM} {_vm.OrderDetails.Phone.Model} {_vm.OrderDetails.DeviceType.ToString().ToLower()} has been sent to<br />");
    }

    [Test]
    public void GenerateEmail_ShouldDelivery_WhenSelectedLocationNull()
    {
        TestSetup(_phone);

        _vm.GenerateEmailHtml();

        _vm.EmailHtml.Should().Contain($"<p>Your {_vm.OrderDetails.Phone.OEM} {_vm.OrderDetails.Phone.Model} {_vm.OrderDetails.DeviceType.ToString().ToLower()} has been sent to<br />");
    }

    [Test]
    [Arguments("N", "New")]
    [Arguments("R", "Repurposed")]
    public async Task NorR_Includes_DeviceSuppliedAsync(string norr, string norrDescription)
    {
        _phone.Condition = norr;
        OrderDetails orderDetails = new(_phone);
        _vm.OrderDetails = orderDetails;

        await Assert.That(_vm.EmailHtml).Contains($"<td>Device supplied:</td><td>{norrDescription} {_phone.OEM} {_phone.Model}</td>");
    }

    [Test]
    public async Task OrderDetails_ShouldSetDeviceTypePhone_WhenModelDoesNotContanIPadAsync()
    {
        TestSetup(_phone);

        await Assert.That(_vm.OrderDetails.DeviceType).IsEqualTo(DeviceType.Phone);
    }

    [Test]
    public async Task OrderDetails_ShouldSetDeviceTypeTable_WhenModelContainsIPadAsync()
    {
        _phone.Model = "iPad";
        TestSetup(_phone);

        await Assert.That(_vm.OrderDetails.DeviceType).IsEqualTo(DeviceType.Tablet);
    }

    [Test]
    public async Task OrderDetails_ShouldSetOrderTypeNew_WhenPhoneDetailsSuppliedAsync()
    {
        TestSetup(_phone);

        await Assert.That(_vm.OrderType).IsEqualTo(OrderType.New);
    }

    [Test]
    public async Task OrderDetails_ShouldSetOrderTypeReplacement_WhenPhoneDetailsNotSuppliedAsync()
    {
        _phone.PhoneNumber = null;
        _phone.SimNumber = null;
        TestSetup(_phone);

        await Assert.That(_vm.OrderType).IsEqualTo(OrderType.Replacement);
    }

    [Test]
    public async Task OrderType_New_GeneratesHtmlAsync()
    {
        TestSetup(_phone);

        _vm.OrderType = OrderType.New;

        await Assert.That(_vm.EmailHtml).DoesNotContain("Don't forget to transfer your old sim");
        await Assert.That(_vm.EmailHtml).Contains("<td>Order type:</td><td>New ");
    }

    [Test]
    public async Task OrderType_Replacement_GeneratesHtmlAsync()
    {
        TestSetup(_phone);

        _vm.OrderType = OrderType.Replacement;

        await Assert.That(_vm.EmailHtml).Contains("Don't forget to transfer your old sim");
        await Assert.That(_vm.EmailHtml).Contains("<td>Order type:</td><td>Replacement ");
    }

    [Test]
    public async Task OEM_Apple_Includes_AppleDetailsAsync()
    {
        _phone.OEM = OEMs.Apple;
        TestSetup(_phone);

        await Assert.That(_vm.EmailHtml).Contains(DataUsage);
        _vm.EmailHtml.Should().Contain(@"<a href=""https://devoncc.sharepoint.com/:w:/r/sites/ICTKB/_layouts/15/Doc.aspx?sourcedoc=%7BABC3F4D7-1159-4F72-9C0B-7E155B970A28%7D&file=How%20to%20set%20up%20your%20new%20DCC%20iPhone.docx&action=default&mobileredirect=true"">");
        await Assert.That(_vm.EmailHtml).Contains("Apple (iOS) Smartphone");
    }

    [Test]
    [Description("Issue 43")]
    public async Task OEM_Nokia_Includes_NokiaDetailsAsync()
    {
        _phone.OEM = OEMs.Nokia;
        TestSetup(_phone);

        await Assert.That(_vm.EmailHtml).DoesNotContain(DataUsage);
        await Assert.That(_vm.EmailHtml).DoesNotContain("Smartphone");
    }

    [Test]
    public async Task OEM_Samsung_Includes_SamsungDetailsAsync()
    {
        _phone.OEM = OEMs.Samsung;
        TestSetup(_phone);

        await Assert.That(_vm.EmailHtml).Contains(DataUsage);
        _vm.EmailHtml.Should().Contain(@"<a href=""https://devoncc.sharepoint.com/:w:/r/sites/ICTKB/Public/Android%20Enterprise%20-%20Setting%20up%20your%20Android%20Phone.docx?d=w64bb3f0a09e44557a64bb78311ee513b&csf=1&web=1"">");
        await Assert.That(_vm.EmailHtml).Contains("Android Smartphone");
    }

    [Test]
    public async Task PhoneNumber_Null_ExcludesPhoneNumberAsync()
    {
        _phone.PhoneNumber = null;
        TestSetup(_phone);

        await Assert.That(_vm.EmailHtml).DoesNotContain($"<tr><td>Phone number:</td><td>");
    }

    [Test]
    public async Task ReformatDeliveryAddress_ShouldStripHeadingsAsync()
    {
        string actual = EmailViewModel.ReformatDeliveryAddress("""
            User Name
            First line of address
            DCS, Springfield Court
            Second line of address
            Fishleigh Road
            Town/city
            Barnstaple
            County
            Devon
            Postcode
            EX31 3UD
            """);

        await Assert.That(actual).IsEqualTo("""
            User Name
            DCS, Springfield Court
            Fishleigh Road
            Barnstaple
            Devon
            EX31 3UD
            """);
    }

    [Test]
    public async Task PhoneNumber_NotNull_IncludesPhoneNumberAsync()
    {
        TestSetup(_phone);

        await Assert.That(_vm.EmailHtml).Contains($"<tr><td>Phone number:</td><td>{_phone.PhoneNumber}</td></tr></table>");
    }

    [Test]
    public async Task SelectedLocation_InterpolatesValuesFor_DeliveryAddressAsync()
    {
        _phone.NewUser = "New User";
        _phone.SR = 42;
        _phone.PhoneNumber = "999";
        TestSetup(_phone);

        _vm.SelectedLocation = new Location { Name = "Collect", Address = "{NewUser}, {SR}, {PhoneNumber}", PrintDate = true };

        await Assert.That(_vm.DeliveryAddress).Contains(_phone.NewUser);
        await Assert.That(_vm.DeliveryAddress).Contains(_phone.SR.ToString()!);
        await Assert.That(_vm.DeliveryAddress).Contains(_phone.PhoneNumber);
    }

    [Test]
    public async Task SelectedLocation_WithPrintDateTrue_SetsCollectionDetailsAsync()
    {
        TestSetup(_phone);

        _vm.SelectedLocation = new Location { Name = "Collect", Address = "Collection Address", PrintDate = true };

        await Assert.That(_vm.EmailHtml).Contains(" can be collected from</br>");
        await Assert.That(_vm.EmailHtml).Contains("It will be available for collection from");
    }

    [Test]
    public async Task SelectedLocation_WithPrintDateTrueAndNameIncludesL87_AddTeamChatDetailsAsync()
    {
        TestSetup(_phone);

        _vm.SelectedLocation = new Location { Name = "Collect L87", Address = "Collection Address", PrintDate = true };

        await Assert.That(_vm.EmailHtml).Contains("County Hall EUC Appointments &amp; Collections");        
    }

    [Test]
    public async Task SelectedLocation_WithPrintDateFalse_SetsDeliveryDetailsAsync()
    {
        TestSetup(_phone);

        _vm.SelectedLocation = new Location { Name = "Deliver", Address = "Delivery Address", PrintDate = false };

        await Assert.That(_vm.EmailHtml).Contains(" has been sent to");
        await Assert.That(_vm.EmailHtml).Contains("It was sent on");
    }

    [Test]
    [Arguments("2/12/2023", "Monday 4<sup>th</sup> December 2023")]
    [Arguments("3/12/2023", "Monday 4<sup>th</sup> December 2023")]
    [Arguments("1/1/2024", "Monday 1<sup>st</sup> January 2024")]
    [Arguments("2/1/2024", "Tuesday 2<sup>nd</sup> January 2024")]
    [Arguments("3/1/2024", "Wednesday 3<sup>rd</sup> January 2024")]
    [Arguments("21/12/2023", "Thursday 21<sup>st</sup> December 2023")]
    [Arguments("22/12/2023", "Friday 22<sup>nd</sup> December 2023")]
    [Arguments("23/1/2024", "Tuesday 23<sup>rd</sup> January 2024")]
    [Arguments("31/1/2024", "Wednesday 31<sup>st</sup> January 2024")]
    [Arguments("12/02/2024", "Monday 12<sup>th</sup> February 2024")] // Issue #40
    public async Task ToOrdinalWorkingDate_IgnoresWeekendsAsync(string date, string expected)
    {
        CultureInfo culture = new("en-GB");
        string actual = EmailViewModel.ToOrdinalWorkingDate(DateTime.Parse(date,culture));

        await Assert.That(actual).IsEqualTo(expected);
    }

    [Test]
    [Arguments("2/12/2023", "Monday 4\x1D57\x02B0 December 2023")]
    [Arguments("3/12/2023", "Monday 4\x1D57\x02B0 December 2023")]
    [Arguments("1/1/2024", "Monday 1\x02E2\x1D57 January 2024")]
    [Arguments("2/1/2024", "Tuesday 2\x207F\x1D48 January 2024")]
    [Arguments("3/1/2024", "Wednesday 3\x02B3\x1D48 January 2024")]
    [Arguments("21/12/2023", "Thursday 21\x02E2\x1D57 December 2023")]
    [Arguments("22/12/2023", "Friday 22\x207F\x1D48 December 2023")]
    [Arguments("23/1/2024", "Tuesday 23\x02B3\x1D48 January 2024")]
    [Arguments("31/1/2024", "Wednesday 31\x02E2\x1d57 January 2024")]
    [Arguments("12/02/2024", "Monday 12\x1D57\x02B0 February 2024")] // Issue #40
    public async Task ToOrdinalWorkingDate_WithHexSuperscriptAsync(string date, string expected)
    {
        CultureInfo culture = new("en-GB");
        string actual = EmailViewModel.ToOrdinalWorkingDate(DateTime.Parse(date,culture), true);

        await Assert.That(actual).IsEqualTo(expected);
    }
}
