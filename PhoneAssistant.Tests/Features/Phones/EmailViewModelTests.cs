using PhoneAssistant.WPF.Features.Phones;
using PhoneAssistant.WPF.Application.Entities;
using Xunit;
using Moq.AutoMock;
using Moq;
using PhoneAssistant.WPF.Application.Repositories;
using System.Text;
using System.ComponentModel;
using FluentAssertions;

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

    [Fact]
    public void CloseCommand_SetsGeneratingEmail_False()
    {
        TestSetup(_phone);

        _vm.CloseCommand.Execute(null);

        Assert.False(_vm.GeneratingEmail);
    }

    [Fact]
    public void CloseCommand_UpdatesDb()
    {
        TestSetup(_phone);

        _vm.CloseCommand.Execute(null);

        _phones.Verify(r => r.UpdateAsync(_phone), Times.Once);
    }

    [Fact]
    public void Constructor_SetsGeneratingEmail_True()
    {
        TestSetup(_phone);

        Assert.True(_vm.GeneratingEmail);
    }

    [Fact]
    public void DefaultPhone_Contains_BoilerPlate()
    {
        TestSetup(_phone);

        Assert.Contains(
            """
            <span style="font-size:14px; font-family:Verdana;">
            """, _vm.EmailHtml);
        Assert.Contains("</span>", _vm.EmailHtml);

        Assert.Contains(@"<p><br />Before setting up your phone please ensure you register with <a href=""https://www.wifi.service.gov.uk/connect-to-govwifi/"">GovWifi</a></p>", _vm.EmailHtml);
    }

    [Fact]
    public void DespatchDetails_Null_SetsDeliveryAddress()
    {
        _phone.DespatchDetails = null;
        TestSetup(_phone);
        StringBuilder expected = new();
        expected.AppendLine(_phone.NewUser);

        Assert.Equal(expected.ToString(), _vm.DeliveryAddress);
    }

    [Fact]
    public void GenerateEmail_ShouldBeCollection_WhenPrintDateTrue()
    {
        TestSetup(_phone);
        Location location = new() { Name = "name", Address = "address", PrintDate = true};
        _vm.SelectedLocation = location;

        _vm.GenerateEmailHtml();

        _vm.EmailHtml.Should().Contain($"<p>Your {_vm.OrderDetails.Phone.OEM} {_vm.OrderDetails.Phone.Model} {_vm.OrderDetails.DeviceType.ToString().ToLower()} can be collected from</br>");
    }

    [Fact]
    public void GenerateEmail_ShouldDelivery_WhenPrintDateFalse()
    {
        TestSetup(_phone);
        Location location = new() { Name = "name", Address = "address", PrintDate = false };

        _vm.GenerateEmailHtml();

        _vm.EmailHtml.Should().Contain($"<p>Your {_vm.OrderDetails.Phone.OEM} {_vm.OrderDetails.Phone.Model} {_vm.OrderDetails.DeviceType.ToString().ToLower()} has been sent to<br />");
    }

    [Fact]
    public void GenerateEmail_ShouldDelivery_WhenSelectedLocationNull()
    {
        TestSetup(_phone);

        _vm.GenerateEmailHtml();

        _vm.EmailHtml.Should().Contain($"<p>Your {_vm.OrderDetails.Phone.OEM} {_vm.OrderDetails.Phone.Model} {_vm.OrderDetails.DeviceType.ToString().ToLower()} has been sent to<br />");
    }

    [Theory]
    [InlineData("N", "New")]
    [InlineData("R", "Repurposed")]
    public void NorR_Includes_DeviceSupplied(string norr, string norrDescription)
    {
        _phone.Condition = norr;
        OrderDetails orderDetails = new(_phone);
        _vm.OrderDetails = orderDetails;

        Assert.Contains($"<td>Device supplied:</td><td>{norrDescription} {_phone.OEM} {_phone.Model}</td>", _vm.EmailHtml);
    }

    [Fact]
    public void OrderDetails_ShouldSetDeviceTypePhone_WhenModelDoesNotContanIPad()
    {
        TestSetup(_phone);

        Assert.Equal(DeviceType.Phone, _vm.OrderDetails.DeviceType);
    }

    [Fact]
    public void OrderDetails_ShouldSetDeviceTypeTable_WhenModelContainsIPad()
    {
        _phone.Model = "iPad";
        TestSetup(_phone);

        Assert.Equal(DeviceType.Tablet, _vm.OrderDetails.DeviceType);
    }

    [Fact]
    public void OrderDetails_ShouldSetOrderTypeNew_WhenPhoneDetailsSupplied()
    {
        TestSetup(_phone);

        Assert.Equal(OrderType.New, _vm.OrderType);
    }

    [Fact]
    public void OrderDetails_ShouldSetOrderTypeReplacement_WhenPhoneDetailsNotSupplied()
    {
        _phone.PhoneNumber = null;
        _phone.SimNumber = null;
        TestSetup(_phone);

        Assert.Equal(OrderType.Replacement, _vm.OrderType);
    }

    [Fact]
    public void OrderType_New_GeneratesHtml()
    {
        TestSetup(_phone);

        _vm.OrderType = OrderType.New;

        Assert.DoesNotContain("Don't forget to transfer your old sim", _vm.EmailHtml);
        Assert.Contains("<td>Order type:</td><td>New ", _vm.EmailHtml);
    }

    [Fact]
    public void OrderType_Replacement_GeneratesHtml()
    {
        TestSetup(_phone);

        _vm.OrderType = OrderType.Replacement;

        Assert.Contains("Don't forget to transfer your old sim", _vm.EmailHtml);
        Assert.Contains("<td>Order type:</td><td>Replacement ", _vm.EmailHtml);
    }

    [Fact]
    public void OEM_Apple_Includes_AppleDetails()
    {
        _phone.OEM = OEMs.Apple;
        TestSetup(_phone);

        Assert.Contains(DataUsage, _vm.EmailHtml);
        _vm.EmailHtml.Should().Contain(@"<a href=""https://devoncc.sharepoint.com/:w:/r/sites/ICTKB/_layouts/15/Doc.aspx?sourcedoc=%7BABC3F4D7-1159-4F72-9C0B-7E155B970A28%7D&file=How%20to%20set%20up%20your%20new%20DCC%20iPhone.docx&action=default&mobileredirect=true"">");
        Assert.Contains("Apple (iOS) Smartphone", _vm.EmailHtml);
    }

    [Fact]
    [Description("Issue 43")]
    public void OEM_Nokia_Includes_NokiaDetails()
    {
        _phone.OEM = OEMs.Nokia;
        TestSetup(_phone);

        Assert.DoesNotContain(DataUsage, _vm.EmailHtml);
        Assert.DoesNotContain("Smartphone", _vm.EmailHtml);
    }

    [Fact]
    public void OEM_Samsung_Includes_SamsungDetails()
    {
        _phone.OEM = OEMs.Samsung;
        TestSetup(_phone);

        Assert.Contains(DataUsage, _vm.EmailHtml);
        _vm.EmailHtml.Should().Contain(@"<a href=""https://devoncc.sharepoint.com/:w:/r/sites/ICTKB/Public/Android%20Enterprise%20-%20Setting%20up%20your%20Android%20Phone.docx?d=w64bb3f0a09e44557a64bb78311ee513b&csf=1&web=1"">");
        Assert.Contains("Android Smartphone", _vm.EmailHtml);
    }

    [Fact]
    public void PhoneNumber_Null_ExcludesPhoneNumber()
    {
        _phone.PhoneNumber = null;
        TestSetup(_phone);

        Assert.DoesNotContain($"<tr><td>Phone number:</td><td>", _vm.EmailHtml);
    }

    [Fact]
    public void ReformatDeliveryAddress_ShouldStripHeadings()
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

        Assert.Equal("""
            User Name
            DCS, Springfield Court
            Fishleigh Road
            Barnstaple
            Devon
            EX31 3UD
            """, actual);
    }

    [Fact]
    public void PhoneNumber_NotNull_IncludesPhoneNumber()
    {
        TestSetup(_phone);

        Assert.Contains($"<tr><td>Phone number:</td><td>{_phone.PhoneNumber}</td></tr></table>", _vm.EmailHtml);
    }

    [Fact]
    public void SelectedLocation_InterpolatesValuesFor_DeliveryAddress()
    {
        _phone.NewUser = "New User";
        _phone.SR = 42;
        _phone.PhoneNumber = "999";
        TestSetup(_phone);

        _vm.SelectedLocation = new Location { Name = "Collect", Address = "{NewUser}, {SR}, {PhoneNumber}", PrintDate = true };

        Assert.Contains(_phone.NewUser, _vm.DeliveryAddress);
        Assert.Contains(_phone.SR.ToString()!, _vm.DeliveryAddress);
        Assert.Contains(_phone.PhoneNumber, _vm.DeliveryAddress);
    }

    [Fact]
    public void SelectedLocation_WithPrintDateTrue_SetsCollectionDetails()
    {
        TestSetup(_phone);

        _vm.SelectedLocation = new Location { Name = "Collect", Address = "Collection Address", PrintDate = true };

        Assert.Contains(" can be collected from</br>", _vm.EmailHtml);
        Assert.Contains("It will be available for collection from", _vm.EmailHtml);
    }

    [Fact]
    public void SelectedLocation_WithPrintDateTrueAndNameIncludesL87_AddTeamChatDetails()
    {
        TestSetup(_phone);

        _vm.SelectedLocation = new Location { Name = "Collect L87", Address = "Collection Address", PrintDate = true };

        Assert.Contains("County Hall EUC Appointments &amp; Collections", _vm.EmailHtml);        
    }

    [Fact]
    public void SelectedLocation_WithPrintDateFalse_SetsDeliveryDetails()
    {
        TestSetup(_phone);

        _vm.SelectedLocation = new Location { Name = "Deliver", Address = "Delivery Address", PrintDate = false };

        Assert.Contains(" has been sent to", _vm.EmailHtml);
        Assert.Contains("It was sent on", _vm.EmailHtml);
    }

    [Theory]
    [InlineData("2/12/2023", "Monday 4<sup>th</sup> December 2023")]
    [InlineData("3/12/2023", "Monday 4<sup>th</sup> December 2023")]
    [InlineData("1/1/2024", "Monday 1<sup>st</sup> January 2024")]
    [InlineData("2/1/2024", "Tuesday 2<sup>nd</sup> January 2024")]
    [InlineData("3/1/2024", "Wednesday 3<sup>rd</sup> January 2024")]
    [InlineData("21/12/2023", "Thursday 21<sup>st</sup> December 2023")]
    [InlineData("22/12/2023", "Friday 22<sup>nd</sup> December 2023")]
    [InlineData("23/1/2024", "Tuesday 23<sup>rd</sup> January 2024")]
    [InlineData("31/1/2024", "Wednesday 31<sup>st</sup> January 2024")]
    [InlineData("12/02/2024", "Monday 12<sup>th</sup> February 2024")] // Issue #40
    private void ToOrdinalWorkingDate_IgnoresWeekends(string date, string expected)
    {
        string actual = EmailViewModel.ToOrdinalWorkingDate(DateTime.Parse(date));

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("2/12/2023", "Monday 4\x1D57\x02B0 December 2023")]
    [InlineData("3/12/2023", "Monday 4\x1D57\x02B0 December 2023")]
    [InlineData("1/1/2024", "Monday 1\x02E2\x1D57 January 2024")]
    [InlineData("2/1/2024", "Tuesday 2\x207F\x1D48 January 2024")]
    [InlineData("3/1/2024", "Wednesday 3\x02B3\x1D48 January 2024")]
    [InlineData("21/12/2023", "Thursday 21\x02E2\x1D57 December 2023")]
    [InlineData("22/12/2023", "Friday 22\x207F\x1D48 December 2023")]
    [InlineData("23/1/2024", "Tuesday 23\x02B3\x1D48 January 2024")]
    [InlineData("31/1/2024", "Wednesday 31\x02E2\x1d57 January 2024")]
    [InlineData("12/02/2024", "Monday 12\x1D57\x02B0 February 2024")] // Issue #40
    private void ToOrdinalWorkingDate_WithHexSuperscript(string date, string expected)
    {
        string actual = EmailViewModel.ToOrdinalWorkingDate(DateTime.Parse(date), true);

        Assert.Equal(expected, actual);
    }
}
