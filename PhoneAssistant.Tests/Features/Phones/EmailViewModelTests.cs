using PhoneAssistant.WPF.Features.Phones;
using PhoneAssistant.WPF.Application.Entities;
using Xunit;
using Moq.AutoMock;
using Moq;
using PhoneAssistant.WPF.Application.Repositories;
using System.Text;
using System.ComponentModel;

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
    private void CloseCommand_SetsGeneratingEmail_False()
    {
        _vm.CloseCommand.Execute(null);

        Assert.False(_vm.GeneratingEmail);
    }
    [Fact]
    private void CloseCommand_UpdatesDb()
    {
        TestSetup(_phone);

        _vm.CloseCommand.Execute(null);

        _phones.Verify(r => r.UpdateAsync(_phone), Times.Once);
    }

    [Fact]
    private void Constructor_SetsGeneratingEmail_True()
    {
        TestSetup(_phone);

        Assert.True(_vm.GeneratingEmail);
    }

    [Fact]
    private void DefaultPhone_Contains_BoilerPlate()
    {
        TestSetup(_phone);

        Assert.Contains(
            """
            <span style="font-size:14px; font-family:Verdana;">
            """, _vm.EmailHtml);
        Assert.Contains("</span>", _vm.EmailHtml);
    }

    [Fact]
    private void DespatchDetails_Null_SetsDeliveryAddress()
    {
        _phone.DespatchDetails = null;
        TestSetup(_phone);
        StringBuilder expected = new();
        expected.AppendLine(_phone.NewUser);

        Assert.Equal(expected.ToString(), _vm.DeliveryAddress);
    }

    [Theory]
    [InlineData("N", "New")]
    [InlineData("R", "Repurposed")]
    private void NorR_Includes_DeviceSupplied(string norr, string norrDescription)
    {
        _phone.Condition = norr;
        OrderDetails orderDetails = new(_phone);
        _vm.OrderDetails = orderDetails;

        Assert.Contains($"<td>Device supplied:</td><td>{norrDescription} {_phone.OEM} {_phone.Model}</td>", _vm.EmailHtml);
    }

    [Fact]
    private void OrderType_New_GeneratesHtml()
    {
        TestSetup(_phone);

        _vm.OrderType = OrderType.New;

        Assert.DoesNotContain("Don't forget to transfer your old sim", _vm.EmailHtml);
        Assert.Contains("<td>Order type:</td><td>New ", _vm.EmailHtml);
    }
    
    [Fact]
    private void OrderType_Replacement_GeneratesHtml()
    {
        TestSetup(_phone);

        _vm.OrderType = OrderType.Replacement;

        Assert.Contains("Don't forget to transfer your old sim", _vm.EmailHtml);
        Assert.Contains("<td>Order type:</td><td>Replacement ", _vm.EmailHtml);
    }

    [Fact]
    private void OEM_Apple_Includes_AppleDetails()
    {
        _phone.OEM = OEMs.Apple;
        TestSetup(_phone);

        Assert.Contains(DataUsage, _vm.EmailHtml);
        Assert.Contains("Apple (iOS) Smartphone", _vm.EmailHtml);
    }

    [Fact]
    [Description("Issue 43")]
    private void OEM_Nokia_Includes_NokiaDetails()
    {
        _phone.OEM = OEMs.Nokia;
        TestSetup(_phone);

        Assert.DoesNotContain(DataUsage, _vm.EmailHtml);
        Assert.DoesNotContain("Smartphone", _vm.EmailHtml);
    }

    [Fact]
    private void OEM_Samsung_Includes_SamsungDetails()
    {
        _phone.OEM = OEMs.Samsung;
        TestSetup(_phone);

        Assert.Contains(DataUsage, _vm.EmailHtml);
        Assert.Contains("Android Smartphone", _vm.EmailHtml);
    }

    [Fact]
    private void PhoneNumber_Null_ExcludesPhoneNumber()
    {
        _phone.PhoneNumber = null;
        TestSetup(_phone);

        Assert.DoesNotContain($"<tr><td>Phone number:</td><td>", _vm.EmailHtml);
    }

    [Fact]
    private void PhoneNumber_NotNull_IncludesPhoneNumber()
    {
        TestSetup(_phone);

        Assert.Contains($"<tr><td>Phone number:</td><td>{_phone.PhoneNumber}</td></tr></table>", _vm.EmailHtml);
    }

    [Fact]
    private void SelectedLocation_InterpolatesValuesFor_DeliveryAddress()
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
    private void SelectedLocation_WithPrintDateTrue_SetsCollectionDetails()
    {
        TestSetup(_phone);

        _vm.SelectedLocation = new Location { Name = "Collect", Address = "Collection Address", PrintDate = true };

        Assert.Contains("<p>Your phone can be collected from</br>", _vm.EmailHtml);
        Assert.Contains("It will be available for collection from", _vm.EmailHtml);
    }

    [Fact]
    private void SelectedLocation_WithPrintDateFalse_SetsDeliveryDetails()
    {
        TestSetup(_phone);

        _vm.SelectedLocation = new Location { Name = "Deliver", Address = "Delivery Address", PrintDate = false };

        Assert.Contains("Your phone has been sent to", _vm.EmailHtml);
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
    private void ToOrdinalWorkingDate_IgnoresWeekends(string date,string expected)
    {
        string actual = EmailViewModel.ToOrdinalWorkingDate(DateTime.Parse(date));

        Assert.Equal(expected,actual);
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
