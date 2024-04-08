using Moq.AutoMock;

using Xunit;
using PhoneAssistant.WPF.Features.AddItem;
using PhoneAssistant.WPF.Application;
using PhoneAssistant.WPF.Application.Repositories;
using Moq;
using System.ComponentModel.DataAnnotations;

namespace PhoneAssistant.Tests.Features.AddItem;
public class AddItemViewModelTests
{
    private readonly AutoMocker _mocker = new AutoMocker();
    private readonly AddItemViewModel _sut;

    public AddItemViewModelTests()
    {
        _sut = _mocker.CreateInstance<AddItemViewModel>();
    }

    [Fact]
    void PhoneClearCommand_ShouldDisablePhoneSave()
    {
        _sut.PhoneClearCommand.Execute(null);

        Assert.False(_sut.CanSavePhone());
    }

    [Fact]
    void PhoneClearCommand_ShouldResetAllProperties()
    {
        _sut.PhoneClearCommand.Execute(null);

        Assert.Equal(string.Empty, _sut.PhoneCondition); 
        Assert.Equal(string.Empty, _sut.PhoneStatus);
        Assert.Equal(string.Empty,_sut.PhoneImei);
    }

    [Fact]
    void PhoneSaveCommand_ShouldDisablePhoneSave()
    {
        _sut.PhoneSaveCommand.Execute(null);

        Assert.False(_sut.CanSavePhone());
    }

    [Fact]
    void PhoneSaveCommand_ShouldResetAllProperties()
    {
        _sut.PhoneSaveCommand.Execute(null);

        Assert.Equal(string.Empty, _sut.PhoneCondition);
        Assert.Equal(string.Empty, _sut.PhoneStatus);
        Assert.Equal(string.Empty, _sut.PhoneImei);
    }

    [Fact]
    void PhoneSaveCommand_ShouldSaveChanges()
    {        
        Mock<IPhonesRepository> repository = _mocker.GetMock<IPhonesRepository>();
        //repository.Setup()            .ReturnsAsync("LastUpdate");


        _sut.PhoneSaveCommand.Execute(null);        
        Assert.Fail();
    }

    [Fact]
    void PhoneWithValidProperties_ShouldEnableSave()
    {
        _sut.PhoneCondition = ApplicationSettings.Conditions[0];
        _sut.PhoneStatus = ApplicationSettings.Statuses[0];

        Assert.True(_sut.CanSavePhone());
    }

    [Fact]
    void ValidateImei_ShouldReturnError_WhenIMEIEmptyOrWhiteSpace()
    {
        ValidationContext ctx = new(_sut, null, null);

        ValidationResult actual1 = AddItemViewModel.ValidateImeiAsync("", ctx);
        ValidationResult actual2 = AddItemViewModel.ValidateImeiAsync("  ", ctx);

        Assert.NotNull(actual2);
        Assert.Equal("IMEI is required", actual2.ErrorMessage);
    }

    [Fact]
    void ValidateImei_ShouldReturnError_WhenIMEINotNumeric()
    {
        ValidationContext ctx = new(_sut, null, null);

        ValidationResult actual = AddItemViewModel.ValidateImeiAsync("abc", ctx);

        Assert.NotNull(actual);
        Assert.Equal("IMEI must be 15 digits", actual.ErrorMessage);
    }

    [Fact]
    void ValidateImei_ShouldReturnError_WhenIMEINotUnique()
    {
        ValidationContext ctx = new(_sut, null, null);
        Mock<IPhonesRepository> _repository = new Mock<IPhonesRepository>();
        _repository = _mocker.GetMock<IPhonesRepository>();
        _repository.Setup(p => p.ExistsAsync("353427866717729")).ReturnsAsync(true);

        ValidationResult actual = AddItemViewModel.ValidateImeiAsync("353427866717729", ctx);
        _repository.VerifyAll();
        Assert.NotNull(actual);
        Assert.Equal("IMEI must be unique", actual.ErrorMessage);
    }

    [Fact]
    void ValidateIMEI_ShouldReturnError_WhenIMEIInvalid()
    {
        ValidationContext ctx = new(_sut, null, null);

        ValidationResult actual = AddItemViewModel.ValidateImeiAsync("355808981132899", ctx); // An invalid 15-digit IMEI

        Assert.NotNull(actual);
        Assert.Equal("IMEI must be 15 digits", actual.ErrorMessage);
    }

    [Fact]
    void ValidateIMEI_ShouldReturnValidResult_WhenIMEIValid()
    {
        ValidationContext ctx = new(_sut, null, null);

        ValidationResult actual = AddItemViewModel.ValidateImeiAsync("355808981132845", ctx); // A valid 15-digit IMEI

        Assert.Equal(ValidationResult.Success, actual);
    }
}
