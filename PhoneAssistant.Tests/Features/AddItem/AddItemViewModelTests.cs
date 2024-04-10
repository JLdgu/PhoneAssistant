using Moq.AutoMock;

using Xunit;
using PhoneAssistant.WPF.Features.AddItem;
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
    void CanSavePhone_ShouldBeEnabled_WhenAllRequiredPropertiesSupplied()
    {
        _sut.AssetTag = "MP00001";
        _sut.Condition = "condition";
        _sut.Imei = "355808981147090";
        _sut.Status = "status";
        
        Assert.True(_sut.CanSavePhone());
    }

    [Fact]
    void GetErrors_ShouldNotContainAssetTagRequired_WhenAssetTagSet()
    {
        _sut.AssetTag = "MP00001"; 

        IEnumerable<ValidationResult> errors = _sut.GetErrors();
        Assert.NotEmpty(errors);            
        Assert.DoesNotContain(errors, e => e.ErrorMessage!.Contains("Asset Tag"));        
    }

    [Fact]
    void GetErrors_ShouldNotContainConditionRequired_WhenConditionSet()
    {
        _sut.Condition = "condition";

        IEnumerable<ValidationResult> errors = _sut.GetErrors();
        Assert.NotEmpty(errors);
        Assert.DoesNotContain(errors, e => e.ErrorMessage!.Contains("Condition"));
    }

    [Fact]
    void GetErrors_ShouldNotContainIMEIRequired_WhenIMEISet()
    {
        _sut.Imei = "355808981147090";

        IEnumerable<ValidationResult> errors = _sut.GetErrors();
        Assert.NotEmpty(errors);
        Assert.DoesNotContain(errors, e => e.ErrorMessage!.Contains("IMEI"));
    }

    [Fact]
    void GetErrors_ShouldNotContainStatusRequired_WhenStatusSet()
    {
        _sut.Status = "status";

        IEnumerable<ValidationResult> errors = _sut.GetErrors();
        Assert.NotEmpty(errors);
        Assert.DoesNotContain(errors, e => e.ErrorMessage!.Contains("Status"));
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
        ArrangeSetAllProperties();

        _sut.PhoneClearCommand.Execute(null);

        AssertResetAllProperties();
    }

    private void ArrangeSetAllProperties()
    {
        _sut.AssetTag = "MP00000";
        _sut.Condition = "condition";
        _sut.FormerUser = "former user";
        _sut.Imei = "imei";
        _sut.Status = "status";
    }

    private void AssertResetAllProperties()
    {
        Assert.Equal(string.Empty, _sut.AssetTag);
        Assert.Null(_sut.Condition);
        Assert.Equal(string.Empty, _sut.FormerUser);
        Assert.Equal(string.Empty, _sut.Imei);
        Assert.Null(_sut.Status);
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
        ArrangeSetAllProperties();

        _sut.PhoneSaveCommand.Execute(null);

        AssertResetAllProperties();
    }

    //[Fact]
    //void PhoneSaveCommand_ShouldSaveChanges()
    //{        
    //Mock<IPhonesRepository> repository = _mocker.GetMock<IPhonesRepository>();
    ////repository.Setup()            .ReturnsAsync("LastUpdate");


    //_sut.PhoneSaveCommand.Execute(null);        
    //Assert.Fail();
    //}

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
        Assert.Equal("IMEI check digit incorrect", actual.ErrorMessage);
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
        Assert.Equal("IMEI check digit incorrect", actual.ErrorMessage);
    }

    [Fact]
    void ValidateIMEI_ShouldReturnValidResult_WhenIMEIValid()
    {
        ValidationContext ctx = new(_sut, null, null);

        ValidationResult actual = AddItemViewModel.ValidateImeiAsync("355808981132845", ctx); // A valid 15-digit IMEI

        Assert.Equal(ValidationResult.Success, actual);
    }
}
