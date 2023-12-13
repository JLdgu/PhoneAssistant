using PhoneAssistant.WPF.Features.Phones;
using PhoneAssistant.WPF.Application.Entities;
using Xunit;
using Moq.AutoMock;
using Moq;

namespace PhoneAssistant.Tests.Features.Phones;

public sealed class EmailViewModelTests
{
    private readonly v1Phone _phone = new()
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
        _vm.Phone = _phone;
    }

    [Fact]
    private void Constructor_SetsGeneratingEmail_False()
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
    private void CloseCommand_NullParameter_DoesNotSaveChanges()
    {
        _vm.CloseCommand.Execute(null);
        _repository.Verify(r => r.UpdateAsync(_phone), Times.Never);
    }

    [Fact]
    private void CloseCommand_WithNoneNullParameter_SavesChanges()
    {
        _vm.CloseCommand.Execute("Something");
        _repository.Verify(r => r.UpdateAsync(_phone), Times.Once);
    }

    [Fact]
    private void EmailHtml_WithOrderTypeNew()
    {

        _vm.OrderType = OrderType.New;

        Assert.Contains("<td>Order type:</td><td>New</td>", _vm.EmailHtml);
    }
    
    [Fact]
    private void EmailHtml_WithOrderTypeReplacement()
    {
        _vm.OrderType = OrderType.Replacement;

        Assert.Contains("Don't forget to transfer your old sim", _vm.EmailHtml);
        Assert.Contains("<td>Order type:</td><td>Replacement</td>", _vm.EmailHtml);
    }

    [Fact]
    private void DespatchMethod_CollectGMH()
    {
        _vm.DespatchMethod = DespatchMethod.CollectGMH;

        Assert.Contains("Your phone can be collected from", _vm.EmailHtml);
        Assert.Contains("Hardware Room, Great Moor House", _vm.EmailHtml);
        Assert.Contains("It will be available for collection from", _vm.EmailHtml);

        Assert.True(_vm.Phone!.Collection);
    }
    
    [Fact]
    private void EmailHtml_WithDespatchMethodColletL87()
    {
        _vm.DespatchMethod = DespatchMethod.CollectL87;

        Assert.Contains("Your phone can be collected from", _vm.EmailHtml);
        Assert.Contains("Room L87, County Hall", _vm.EmailHtml);
        Assert.Contains("It will be available for collection from", _vm.EmailHtml);

        Assert.True(_vm.Phone!.Collection);
    }

    [Fact]
    private void EmailHtml_WithDespatchDelivery()
    {
        _vm.DespatchMethod = DespatchMethod.Delivery;

        Assert.Contains("Your phone has been sent to", _vm.EmailHtml);
        Assert.Contains("It was sent on", _vm.EmailHtml);

        Assert.False(_vm.Phone!.Collection);
    }

    [Fact]
    private void EmailHtml_WithAppleOEM()
    {
        _phone.OEM = "Apple";
        _vm.Phone = _phone;

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
        _vm.Phone = _phone;

        Assert.Contains($"<td>Phone supplied:</td><td>{norrDescription} {_phone.OEM} {_phone.Model}</td>", _vm.EmailHtml);
    }

    [Theory]
    [InlineData("2/12/2023")]
    [InlineData("3/12/2023")]
    private void ToOrdinalWorkingDate_IgnoresWeekends(string date)
    {
        string actual = EmailViewModel.ToOrdinalWorkingDate(DateTime.Parse(date));

        Assert.Equal("Monday 4th December 2023",actual);
    }
}
