using Moq.AutoMock;

using Xunit;
using PhoneAssistant.WPF.Application.Entities;
using PhoneAssistant.WPF.Features.AddItem;
using PhoneAssistant.WPF.Application.Repositories;
using Moq;
using System.ComponentModel.DataAnnotations;
using PhoneAssistant.WPF.Application;
using CommunityToolkit.Mvvm.Messaging;
using PhoneAssistant.Tests.Shared;
using NPOI.SS.Formula.Functions;
using PhoneAssistant.Model;

namespace PhoneAssistant.Tests.Features.AddItem;
public partial class AddItemViewModelTests
{
    private readonly AutoMocker _mocker = new AutoMocker();
    private readonly AddItemViewModel _sut;

    public AddItemViewModelTests()
    {
        _sut = _mocker.CreateInstance<AddItemViewModel>();
    }

    [Fact]
    public void CanSavePhone_ShouldBeEnabled_WhenNoErrors_WithPhoneHasSim()
    {
        Mock<IBaseReportRepository> sims = _mocker.GetMock<IBaseReportRepository>();
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
        sims.Setup(r => r.GetSimNumberAsync("07123456789")).ReturnsAsync((string)null);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

        _sut.Condition = "condition";
        _sut.Imei = "355808981147090";
        _sut.Model = "model";
        _sut.PhoneNumber = "07123456789";
        _sut.Status = "status";

        IEnumerable<ValidationResult> errors = _sut.GetErrors();
        Assert.Empty(errors);
        Assert.True(_sut.CanSavePhone());
        _mocker.VerifyAll();
    }

    [Fact]
    public void CanSavePhone_ShouldBeEnabled_WhenNoErrors_WithPhoneInStock()
    {
        Mock<IPhonesRepository> _phones = _mocker.GetMock<IPhonesRepository>();
        _phones.Setup(r => r.AssetTagUniqueAsync("MP00001")).ReturnsAsync(true);
        Mock<IBaseReportRepository> sims = _mocker.GetMock<IBaseReportRepository>();
        _sut.AssetTag = "MP00001";
        _sut.Condition = "condition";
        _sut.Imei = "355808981147090";
        _sut.Model = "model";
        _sut.Status = ApplicationConstants.StatusInStock;

        IEnumerable<ValidationResult> errors = _sut.GetErrors();
        Assert.Empty(errors);
        Assert.True(_sut.CanSavePhone());
        _mocker.VerifyAll();
    }

    [Fact]
    public void CanSavePhone_ShouldBeEnabled_WhenNoErrors_WithPhoneOnly()
    {
        _sut.Condition = "condition";
        _sut.Imei = "355808981147090";
        _sut.Model = "model";
        _sut.Status = "status";

        IEnumerable<ValidationResult> errors = _sut.GetErrors();
        Assert.Empty(errors);
        Assert.True(_sut.CanSavePhone());
    }

    [Fact]
    public void GetErrors_ShouldContainAssetTagError_WhenAssetTagNotUnique()
    {
        Mock<IPhonesRepository> _repository = _mocker.GetMock<IPhonesRepository>();
        _repository.Setup(r => r.AssetTagUniqueAsync("MP99999")).ReturnsAsync(false);

        _sut.AssetTag = "MP99999";

        _mocker.VerifyAll();
        IEnumerable<ValidationResult> errors = _sut.GetErrors(nameof(_sut.AssetTag));
        Assert.Equal("Asset Tag must be unique", errors.First().ToString());
    }

    [Theory]
    [InlineData(ApplicationConstants.StatusDecommissioned)]
    [InlineData(ApplicationConstants.StatusDisposed)]
    public void GetErrors_ShouldContainError_WhenDisposalWithDefaultTicket(string status)
    {
        Mock<IUserSettings> setings = _mocker.GetMock<IUserSettings>();
        setings.Setup(s => s.DefaultDecommissionedTicket).Returns(123456);

        _sut.Status = status;

        IEnumerable<ValidationResult> errors = _sut.GetErrors(nameof(_sut.Ticket));
        Assert.Empty(errors);
    }

    [Theory]
    [InlineData(ApplicationConstants.StatusDecommissioned)]
    [InlineData(ApplicationConstants.StatusDisposed)]
    public void GetErrors_ShouldContainError_WhenDisposalWithNoDefaultTicket(string status)
    {
        _sut.Status = status;

        IEnumerable<ValidationResult> errors = _sut.GetErrors(nameof(_sut.Ticket));
        Assert.Equal("Ticket must 6 or 7 digits", errors.First().ToString());
    }

    [Theory]
    [InlineData(" ")]
    [InlineData("12345")]
    [InlineData("12345678")]
    [InlineData("1A345")]
    public void GetErrors_ShouldContainTicketError_WhenTicketInvalid(string ticket)
    {
        Mock<IPhonesRepository> _repository = _mocker.GetMock<IPhonesRepository>();

        _sut.Ticket = ticket;

        IEnumerable<ValidationResult> errors = _sut.GetErrors(nameof(_sut.Ticket));
        Assert.Equal("Ticket must 6 or 7 digits", errors.First().ToString());
    }

    [Fact]
    public void GetErrors_ShouldContainStatueError_WhenInvalidStatusAssetTagCombination()
    {
        _sut.AssetTag = null;
        _sut.Status = ApplicationConstants.StatusInStock;

        IEnumerable<ValidationResult> errors = _sut.GetErrors(nameof(_sut.Status));
        Assert.Equal("Asset Tag required", errors.First().ToString());
    }

    [Theory]
    [InlineData("PC00001")]
    [InlineData("MP00002")]
    public void GetErrors_ShouldBeEmpty_WhenAssetTagFormatValid(string assetTag)
    {
        Mock<IPhonesRepository> _repository = _mocker.GetMock<IPhonesRepository>();
        _repository.Setup(r => r.AssetTagUniqueAsync(assetTag)).ReturnsAsync(true);

        _sut.AssetTag = assetTag;

        _mocker.VerifyAll();
        IEnumerable<ValidationResult> errors = _sut.GetErrors(nameof(_sut.AssetTag));
        Assert.Empty(errors);
    }

    [Fact]
    public void GetErrors_ShouldBeEmpty_WhenAssetTagUnique()
    {
        Mock<IPhonesRepository> _repository = _mocker.GetMock<IPhonesRepository>();
        _repository.Setup(r => r.AssetTagUniqueAsync("MP99999")).ReturnsAsync(true);

        _sut.AssetTag = "MP99999";

        _mocker.VerifyAll();
        IEnumerable<ValidationResult> errors = _sut.GetErrors(nameof(_sut.AssetTag));
        Assert.Empty(errors);
    }

    [Fact]
    public void GetErrors_ShouldBeEmpty_WhenIMEISet()
    {
        _sut.Imei = "355808981147090";

        IEnumerable<ValidationResult> errors = _sut.GetErrors(nameof(_sut.Imei));
        Assert.Empty(errors);
    }

    [Fact]
    public void GetErrors_ShouldBeEmpty_WhenModelSet()
    {
        _sut.Model = "model";

        IEnumerable<ValidationResult> errors = _sut.GetErrors(nameof(_sut.Model));
        Assert.Empty(errors);
    }


    [Theory]
    [InlineData(ApplicationConstants.StatusInStock, "MP00001")]
    [InlineData(ApplicationConstants.StatusProduction, null)]
    [InlineData(ApplicationConstants.StatusProduction, "PC00001")]
    [InlineData(ApplicationConstants.StatusInRepair, null)]
    [InlineData(ApplicationConstants.StatusInRepair, "PC00002")]
    public void GetErrors_ShouldBeEmpty_WhenValidStatusAssetTagCombination(string status, string? assetTag)
    {
        Mock<IPhonesRepository> _repository = _mocker.GetMock<IPhonesRepository>();
        _repository.Setup(r => r.AssetTagUniqueAsync(assetTag)).ReturnsAsync(true);

        _sut.AssetTag = assetTag;
        _sut.Status = status;

        IEnumerable<ValidationResult> errors = _sut.GetErrors(nameof(_sut.Status));
        Assert.Empty(errors);

    }

    [Fact]
    public void GetErrors_ShouldBeEmpty_WhenPhoneNumberNull()
    {
        _sut.PhoneNumber = "07123456789";

        IEnumerable<ValidationResult> errors = _sut.GetErrors(nameof(_sut.PhoneNumber));
        Assert.Empty(errors);
    }

    [Fact]
    public void GetErrors_ShouldBeEmpty_WhenPhoneNumberSet()
    {
        _sut.PhoneNumber = "07123456789";

        IEnumerable<ValidationResult> errors = _sut.GetErrors(nameof(_sut.PhoneNumber));
        Assert.Empty(errors);
    }

    [Fact]
    public void GetErrors_ShouldBeEmpty_WhenSimNumberNull()
    {
        _sut.SimNumber = null;

        IEnumerable<ValidationResult> errors = _sut.GetErrors(nameof(_sut.SimNumber));
        Assert.Empty(errors);
    }

    [Fact]
    public void GetErrors_ShouldBeEmpty_WhenSimNumberSet()
    {
        _sut.SimNumber = "8944122605566849402";

        IEnumerable<ValidationResult> errors = _sut.GetErrors(nameof(_sut.SimNumber));
        Assert.Empty(errors);
    }

    [Fact]
    public void OnPhoneNumberChanged_ShouldSetSimNumber_WhenSimExists()
    {
        Mock<IBaseReportRepository> _repository = _mocker.GetMock<IBaseReportRepository>();
        _repository.Setup(r => r.GetSimNumberAsync("07123456789")).ReturnsAsync("sim number");

        _sut.PhoneNumber = "07123456789";

        _mocker.VerifyAll();
        Assert.Equal("sim number", _sut.SimNumber);
    }

    [Fact]
    public void PhoneClearCommand_ShouldDisablePhoneSave()
    {
        _sut.PhoneClearCommand.Execute(null);

        Assert.False(_sut.CanSavePhone());
    }

    [Fact]
    public void PhoneClearCommand_ShouldResetAllProperties()
    {
        ArrangeSetAllPhoneProperties();

        _sut.PhoneClearCommand.Execute(null);

        AssertResetAllPhoneProperties();
    }

    [Fact]
    public void PhoneSaveCommand_WithPhoneAndSim_ShouldCallRepository()
    {
        const string expectedAssetTag = "MP00001";
        const string expectedImei = "355808981147090";
        const string expectedModel = "model";
        const string expectedPhoneNumber = "07123456789";
        const string expectedSimNumber = "355808981147090";
        Phone actual = new()
        {
            Condition = "norr",
            Imei = "imei",
            Model = "model",
            OEM = OEMs.Apple,
            Status = "status"
        };
        Mock<IPhonesRepository> _repository = _mocker.GetMock<IPhonesRepository>();
        _repository.Setup(r => r.AssetTagUniqueAsync("MP00001")).ReturnsAsync(true);
        _repository.Setup(r => r.CreateAsync(It.IsAny<Phone>())).Callback<Phone>((p) => actual = p);

        _sut.AssetTag = expectedAssetTag;
        _sut.Imei = expectedImei;
        _sut.Model = expectedModel;
        _sut.PhoneNumber = expectedPhoneNumber;
        _sut.SimNumber = expectedSimNumber;

        _sut.PhoneSaveCommand.Execute(null);

        Assert.Equal(expectedAssetTag, actual.AssetTag);
        Assert.Equal(expectedImei, actual.Imei);
        Assert.Equal(expectedModel, actual.Model);
        Assert.Equal(expectedPhoneNumber, actual.PhoneNumber);
        Assert.Equal(expectedSimNumber, actual.SimNumber);
        _mocker.VerifyAll();
    }

    [Fact]
    public void PhoneSaveCommand_WithPhoneOnly_ShouldCallRepository()
    {
        const string expectedAssetTag = "MP00001";
        const string expectedImei = "355808981147090";
        const string expectedModel = "model";        
        Phone actual = new()
        {
            Condition = "norr",
            Imei = "imei",
            Model = "model",
            OEM = OEMs.Apple,
            Status = "status"
        };
        Mock<IPhonesRepository> _repository = _mocker.GetMock<IPhonesRepository>();
        _repository.Setup(r => r.AssetTagUniqueAsync("MP00001")).ReturnsAsync(true);
        _repository.Setup(r => r.CreateAsync(It.IsAny<Phone>())).Callback<Phone>((p) => actual = p);

        _sut.AssetTag = expectedAssetTag;
        _sut.Imei = expectedImei;
        _sut.Model = expectedModel;

        _sut.PhoneSaveCommand.Execute(null);

        Assert.Equal(expectedAssetTag, actual.AssetTag);
        Assert.Equal(expectedImei, actual.Imei);
        Assert.Equal(expectedModel, actual.Model);
        Assert.Null(actual.PhoneNumber);
        Assert.Null(actual.SimNumber);
        _mocker.VerifyAll();
    }

    [Fact]
    public void PhoneSaveCommand_ShouldDisablePhoneSave()
    {
        _sut.PhoneSaveCommand.Execute(null);

        Assert.False(_sut.CanSavePhone());
    }

    [Fact]
    public void PhoneSaveCommand_ShouldResetAllPhoneProperties()
    {
        ArrangeSetAllPhoneProperties();

        _sut.PhoneSaveCommand.Execute(null);

        AssertResetAllPhoneProperties();
    }

    [Fact]
    public void PhoneSaveCommand_ShouldSendPhoneMessage()
    {
        Mock<IMessenger> message = _mocker.GetMock<IMessenger>();
        message.Setup(m => m.Send(It.IsAny<Phone>(), It.IsAny<IsAnyToken>()));

        _sut.AssetTag = "MP00001";
        _sut.Imei = "355808981147090";
        _sut.Model = "model";

        _sut.PhoneSaveCommand.Execute(null);

        message.Verify(x => x.Send(It.IsAny<Phone>(), It.IsAny<IsAnyToken>()), Times.Once);
    }

    [Fact]
    public void ValidateImei_ShouldReturnError_WhenIMEIEmptyOrWhiteSpace()
    {
        ValidationContext ctx = new(_sut, null, null);

        ValidationResult actual1 = AddItemViewModel.ValidateImeiAsync("", ctx);
        ValidationResult actual2 = AddItemViewModel.ValidateImeiAsync("  ", ctx);

        Assert.NotNull(actual1);
        Assert.Equal("IMEI is required", actual1.ErrorMessage);
        Assert.NotNull(actual2);
        Assert.Equal("IMEI is required", actual2.ErrorMessage);
    }

    [Fact]
    public void ValidateImei_ShouldReturnError_WhenIMEINotNumeric()
    {
        ValidationContext ctx = new(_sut, null, null);

        ValidationResult actual = AddItemViewModel.ValidateImeiAsync("abc", ctx);

        Assert.NotNull(actual);
        Assert.Equal("IMEI check digit incorrect", actual.ErrorMessage);
    }

    [Fact]
    public void ValidateImei_ShouldReturnError_WhenIMEINotUnique()
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
    public void ValidateIMEI_ShouldReturnError_WhenIMEIInvalid()
    {
        ValidationContext ctx = new(_sut, null, null);

        ValidationResult actual = AddItemViewModel.ValidateImeiAsync("355808981132899", ctx); // An invalid 15-digit IMEI

        Assert.NotNull(actual);
        Assert.Equal("IMEI check digit incorrect", actual.ErrorMessage);
    }

    [Fact]
    public void ValidateIMEI_ShouldReturnValidResult_WhenIMEIValid()
    {
        ValidationContext ctx = new(_sut, null, null);

        ValidationResult actual = AddItemViewModel.ValidateImeiAsync("355808981132845", ctx); // A valid 15-digit IMEI

        Assert.Equal(ValidationResult.Success, actual);
    }

    [Fact]
    public void ValidatePhoneNumber_ShouldReturnError_WhenPhoneNumberNotUnique()
    {
        ValidationContext ctx = new(_sut, null, null);
        Mock<IPhonesRepository> _repository = new Mock<IPhonesRepository>();
        _repository = _mocker.GetMock<IPhonesRepository>();
        _repository.Setup(p => p.PhoneNumberExistsAsync("07123456789")).ReturnsAsync(true);

        ValidationResult? actual = AddItemViewModel.ValidatePhoneNumber("07123456789", ctx);

        _repository.VerifyAll();
        Assert.NotNull(actual);
        Assert.Equal("Phone Number already linked to phone", actual.ErrorMessage);
    }

    [Fact]
    public void ValidatePhoneNumber_ShouldReturnValidResult_WhenPhoneNumberUnique()
    {
        ValidationContext ctx = new(_sut, null, null);
        Mock<IPhonesRepository> _repository = new Mock<IPhonesRepository>();
        _repository = _mocker.GetMock<IPhonesRepository>();
        _repository.Setup(p => p.PhoneNumberExistsAsync("07123456789")).ReturnsAsync(false);

        ValidationResult? actual = AddItemViewModel.ValidatePhoneNumber("07123456789", ctx);

        _repository.VerifyAll();
        Assert.Equal(ValidationResult.Success, actual);
    }

    [Fact]
    public void ValidateSimNumber_ShouldReturnValidResult_WhenSimNumberEmptyOrWhiteSpace()
    {
        ValidationContext ctx = new(_sut, null, null);

        ValidationResult actual1 = AddItemViewModel.ValidateSimNumber("", ctx);
        ValidationResult actual2 = AddItemViewModel.ValidateSimNumber("  ", ctx);

        Assert.Null(actual1);
        Assert.Null(actual2);
    }

    [Fact]
    public void ValidateSimNumber_ShouldReturnError_WhenSimNumberNotNumeric()
    {
        ValidationContext ctx = new(_sut, null, null);

        ValidationResult actual = AddItemViewModel.ValidateSimNumber("abc", ctx);

        Assert.NotNull(actual);
        Assert.Equal("SIM Number must be 19 digits", actual.ErrorMessage);
    }

    [Fact]
    public void ValidateSimNumber_ShouldReturnError_WhenSimNumberInvalid()
    {
        ValidationContext ctx = new(_sut, null, null);

        ValidationResult actual = AddItemViewModel.ValidateSimNumber("8944125605540324744", ctx); // An invalid 15-digit SIM Number

        Assert.NotNull(actual);
        Assert.Equal("SIM Number check digit incorrect", actual.ErrorMessage);
    }

    [Fact]
    public void ValidateSimNumber_ShouldReturnValidResult_WhenSimNumberValid()
    {
        ValidationContext ctx = new(_sut, null, null);

        ValidationResult actual = AddItemViewModel.ValidateSimNumber("8944125605540324743", ctx); // A valid 19-digit SimNumber

        Assert.Equal(ValidationResult.Success, actual);
    }

    private void ArrangeSetAllPhoneProperties()
    {
        _sut.AssetTag = "MP00000";
        _sut.Condition = "condition";
        _sut.FormerUser = "former user";
        _sut.Imei = "imei";
        _sut.Model = "model";
        _sut.PhoneNotes = "notes";
        _sut.PhoneNumber = "07123456789";
        _sut.OEM = OEMs.Samsung;
        _sut.SimNumber = "8944125605540324743";
        _sut.Status = "status";
        _sut.Ticket = 7654321.ToString();
    }

    private void AssertResetAllPhoneProperties()
    {
        Assert.Null(_sut.AssetTag);
        Assert.Equal(ApplicationConstants.Conditions[1].Substring(0, 1), _sut.Condition);
        Assert.Null(_sut.FormerUser);
        Assert.Equal(string.Empty, _sut.Imei);
        Assert.Equal(string.Empty, _sut.Model);
        Assert.Null(_sut.PhoneNotes);
        Assert.Null(_sut.PhoneNumber);
        Assert.Equal(OEMs.Samsung, _sut.OEM);
        Assert.Equal(ApplicationConstants.Statuses[1], _sut.Status);
        Assert.Null(_sut.SimNumber);
        Assert.Null(_sut.Ticket);
    }
}

