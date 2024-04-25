using Moq.AutoMock;

using Xunit;
using PhoneAssistant.WPF.Application.Entities;
using PhoneAssistant.WPF.Features.AddItem;
using PhoneAssistant.WPF.Application.Repositories;
using Moq;
using System.ComponentModel.DataAnnotations;
using PhoneAssistant.WPF.Application;

namespace PhoneAssistant.Tests.Features.AddItem;
public class AddItemViewModelTests
{
    private readonly AutoMocker _mocker = new AutoMocker();
    private readonly AddItemViewModel _sut;

    public AddItemViewModelTests()
    {
        _sut = _mocker.CreateInstance<AddItemViewModel>();
    }

    #region NewPhone
    [Fact]
    void CanSavePhone_ShouldBeEnabled_WhenAllRequiredPropertiesSupplied()
    {
        Mock<IPhonesRepository> _repository = _mocker.GetMock<IPhonesRepository>();
        _repository.Setup(r => r.AssetTagUniqueAsync("MP00001")).ReturnsAsync(true);

        _sut.AssetTag = "MP00001";
        _sut.Condition = "condition";
        _sut.Imei = "355808981147090";
        _sut.Model = "model";
        _sut.Status = "status";
        
        _mocker.VerifyAll();
        Assert.True(_sut.CanSavePhone());
    }

    [Fact]
    void GetErrors_ShouldContainAssetTagError_WhenAssetTagNotUnique()
    {
        Mock<IPhonesRepository> _repository = _mocker.GetMock<IPhonesRepository>();
        _repository.Setup(r => r.AssetTagUniqueAsync("MP99999")).ReturnsAsync(false);

        _sut.AssetTag = "MP99999";

        _mocker.VerifyAll();
        IEnumerable<ValidationResult> errors = _sut.GetErrors(nameof(_sut.AssetTag));
        Assert.Equal("Asset Tag must be unique", errors.First().ToString());
    }

    [Fact]
    void GetErrors_ShouldBeEmpty_WhenAssetTagUnique()
    {
        Mock<IPhonesRepository> _repository = _mocker.GetMock<IPhonesRepository>();
        _repository.Setup(r => r.AssetTagUniqueAsync("MP99999")).ReturnsAsync(true);

        _sut.AssetTag = "MP99999";

        _mocker.VerifyAll();
        IEnumerable<ValidationResult> errors = _sut.GetErrors(nameof(_sut.AssetTag));
        Assert.Empty(errors);
    }

    [Fact]
    void GetErrors_ShouldBeEmpty_WhenIMEISet()
    {
        _sut.Imei = "355808981147090";

        IEnumerable<ValidationResult> errors = _sut.GetErrors(nameof(_sut.Imei));
        Assert.Empty(errors);
    }

    [Fact]
    void GetErrors_ShouldBeEmpty_WhenModelSet()
    {
        _sut.Model = "model";

        IEnumerable<ValidationResult> errors = _sut.GetErrors(nameof(_sut.Model));
        Assert.Empty(errors);
    }

    [Fact]
    void OnPhoneNumberChanged_ShouldSetSimNumber_WhenSIMExists()
    {
        Mock<ISimsRepository> _repository = _mocker.GetMock<ISimsRepository>();
        _repository.Setup(r => r.GetSIMNumberAsync("07123456789")).ReturnsAsync("sim number");

        _sut.PhoneNumber = "07123456789";

        _repository.VerifyAll();
        Assert.Equal("sim number", _sut.SimNumber);
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
        ArrangeSetAllPhoneProperties();

        _sut.PhoneClearCommand.Execute(null);

        AssertResetAllPhoneProperties();
    }

    private void ArrangeSetAllPhoneProperties()
    {
        _sut.AssetTag = "MP00000";
        _sut.Condition = "condition";
        _sut.FormerUser = "former user";
        _sut.Imei = "imei";
        _sut.Model = "model";
        _sut.PhoneNotes = "notes";
        _sut.OEM = OEMs.Samsung;
        _sut.Status = "status";
    }

    private void AssertResetAllPhoneProperties()
    {
        Assert.Null(_sut.AssetTag);
        Assert.Equal(ApplicationSettings.Conditions[1].Substring(0, 1), _sut.Condition);
        Assert.Null( _sut.FormerUser);
        Assert.Equal(string.Empty, _sut.Imei);
        Assert.Equal(string.Empty, _sut.Model);
        Assert.Null(_sut.PhoneNotes);
        Assert.Equal(OEMs.Apple,_sut.OEM);
        Assert.Equal(ApplicationSettings.Statuses[1], _sut.Status);
    }

    [Fact]
    void PhoneSaveCommand_ShouldCallRepository()
    {
        Mock<IPhonesRepository> _repository = _mocker.GetMock<IPhonesRepository>();
        _repository.Setup(r => r.AssetTagUniqueAsync("MP00001")).ReturnsAsync(true);
        _repository.Setup(r => r.CreateAsync(It.IsAny<Phone>()));

        _sut.AssetTag = "MP00001";
        _sut.Imei = "355808981147090";
        _sut.Model = "model";

        _sut.PhoneSaveCommand.Execute(null);

        _mocker.VerifyAll();
    }

    [Fact]
    void PhoneSaveCommand_ShouldDisablePhoneSave()
    {
        _sut.PhoneSaveCommand.Execute(null);

        Assert.False(_sut.CanSavePhone());
    }

    [Fact]
    void PhoneSaveCommand_ShouldResetAllPhoneProperties()
    {
        ArrangeSetAllPhoneProperties();

        _sut.PhoneSaveCommand.Execute(null);

        AssertResetAllPhoneProperties();
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
    #endregion

    #region NewSIM
    [Fact]
    void CanSaveSIM_ShouldBeEnabled_WhenAllRequiredPropertiesSupplied()
    {
        //Mock<IPhonesRepository> _repository = _mocker.GetMock<IPhonesRepository>();
        //_repository.Setup(r => r.AssetTagUniqueAsync("MP00001")).ReturnsAsync(true);

        //_sut.AssetTag = "MP00001";
        //_sut.Condition = "condition";
        //_sut.Imei = "355808981147090";
        //_sut.Model = "model";
        //_sut.Status = "status";

        //_mocker.VerifyAll();
        //Assert.True(_sut.CanSavePhone());
    }
    [Fact]
    void SIMClearCommand_ShouldDisableSIMSave()
    {
        _sut.SIMClearCommand.Execute(null);

        Assert.False(_sut.CanSaveSIM());
    }

    [Fact]
    void SIMClearCommand_ShouldResetAllSIMProperties()
    {
        _sut.PhoneNumber = "1234567890";
        _sut.SimNumber = "1234567890123456789";
        _sut.SimNotes = "Notes";

        _sut.SIMClearCommand.Execute(null);

        Assert.Null(_sut.PhoneNumber);
        Assert.Null(_sut.SimNumber);
        Assert.Null(_sut.SimNotes);
    }
    #endregion
}
