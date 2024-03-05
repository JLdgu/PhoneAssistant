using PhoneAssistant.WPF.Features.Phones;
using PhoneAssistant.WPF.Application.Entities;
using Xunit;
using Moq.AutoMock;
using Moq;
using PhoneAssistant.WPF.Application.Repositories;

namespace PhoneAssistant.Tests.Features.Phones;

public sealed class EmailViewModelTests
{
    private readonly Phone _phone = new()
    {
        PhoneNumber = "phoneNumber",
        SimNumber = "simNumber",
        Status = "status",
        AssetTag = "at",
        FormerUser = "fu",
        Imei = "imei",
        LastUpdate = "lastupdate",
        Model = "model",
        NewUser = "nu",
        NorR = "norr",
        Notes = "note",
        OEM = "oem",
        SR = 123456
    };

    private readonly AutoMocker _mocker = new AutoMocker();
    private readonly Mock<IPhonesRepository> _repository;

    private readonly EmailViewModel _vm;

    public EmailViewModelTests()
    {
        _mocker.Use(_phone);
        _repository = _mocker.GetMock<IPhonesRepository>();
        _vm = _mocker.CreateInstance<EmailViewModel>();
        
        OrderDetails orderDetails = new(_phone);
        _vm.OrderDetails = orderDetails;
    }

    [Fact]
    private void Constructor_SetsGeneratingEmail_True()
    {
        Assert.True(_vm.GeneratingEmail);
    }

    [Fact]
    private void CloseCommand_SetsGeneratingEmail_False()
    {
        _vm.CloseCommand.Execute(null);
        Assert.False(_vm.GeneratingEmail);
    }

    [Fact]
    private void EmailHtml_BoilerPlate()
    {
        Assert.Contains(
            """
            <span style="font-size:12px; font-family:Verdana;">
            """, _vm.EmailHtml);
        Assert.Contains(
            """
            <p><br /><a href="https://devoncc.sharepoint.com/:w:/r/sites/ICTKB/Public/DCC%20mobile%20phone%20data%20usage%20guidance%20and%20policies.docx?d=w9ce15b2ddbb343739f131311567dd305&csf=1&web=1"
                  style="font-size:12px; font-family:Verdana";>
            DCC mobile phone data usage guidance and policies</a></p>
            """, _vm.EmailHtml);
        Assert.Contains("</span>",_vm.EmailHtml);
    }

    [Fact]
    private void EmailHtml_WithOrderTypeNew()
    {

        _vm.OrderType = OrderType.New;

        Assert.Contains("<td>Order type:</td><td>New ", _vm.EmailHtml);
    }
    
    [Fact]
    private void EmailHtml_WithOrderTypeReplacement()
    {
        _vm.OrderType = OrderType.Replacement;

        Assert.Contains("Don't forget to transfer your old sim", _vm.EmailHtml);
        Assert.Contains("<td>Order type:</td><td>Replacement ", _vm.EmailHtml);
    }

    [Fact]
    private void DespatchMethod_CollectGMH()
    {
        _vm.DespatchMethod = DespatchMethod.CollectGMH;

        Assert.Contains("Your phone can be collected from", _vm.EmailHtml);
        Assert.Contains("Hardware Room, Great Moor House", _vm.EmailHtml);
        Assert.Contains("It will be available for collection from", _vm.EmailHtml);
    }
    
    [Fact]
    private void EmailHtml_WithDespatchMethodColletL87()
    {
        _vm.DespatchMethod = DespatchMethod.CollectL87;

        Assert.Contains("Your phone can be collected from", _vm.EmailHtml);
        Assert.Contains("Room L87, County Hall", _vm.EmailHtml);
        Assert.Contains("It will be available for collection from", _vm.EmailHtml);
    }

    [Fact]
    private void EmailHtml_WithDespatchDelivery()
    {
        _vm.DespatchMethod = DespatchMethod.Delivery;

        Assert.Contains("Your phone has been sent to", _vm.EmailHtml);
        Assert.Contains("It was sent on", _vm.EmailHtml);
    }

    [Fact]
    private void EmailHtml_WithAppleOEM()
    {
        _phone.OEM = "Apple";
        OrderDetails orderDetails = new(_phone);
        _vm.OrderDetails = orderDetails;


        Assert.Contains("Apple (iOS) Smartphone", _vm.EmailHtml);
    }

    [Fact]
    private void EmailHtml_WithNoneAppleOEM()
    {
        Assert.Contains("Android Smartphone", _vm.EmailHtml);
    }

    [Theory]
    [InlineData("N", "New")]
    [InlineData("R", "Repurposed")]   
    private void EmailHtml_ContainsPhoneDetails(string norr, string norrDescription)
    {
        _phone.NorR = norr;
        OrderDetails orderDetails = new(_phone);
        _vm.OrderDetails = orderDetails;

        Assert.Contains($"<td>Device supplied:</td><td>{norrDescription} {_phone.OEM} {_phone.Model}</td>", _vm.EmailHtml);
    }

    [Fact]
    private void EmailHtml_WithNullPhoneNumber()
    {
        _phone.PhoneNumber = null;
        OrderDetails orderDetails = new(_phone);
        _vm.OrderDetails = orderDetails;

        Assert.DoesNotContain($"<tr><td>Phone number:</td><td>", _vm.EmailHtml);
    }

    [Fact]
    private void EmailHtml_WithPhoneNumber()
    {
        Assert.Contains($"<tr><td>Phone number:</td><td>{_phone.PhoneNumber}</td></tr></table>", _vm.EmailHtml);
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
}
