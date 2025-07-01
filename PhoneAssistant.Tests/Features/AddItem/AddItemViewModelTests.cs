using CommunityToolkit.Mvvm.Messaging;
using Moq;
using Moq.AutoMock;
using PhoneAssistant.Model;
using PhoneAssistant.Tests.Shared;
using PhoneAssistant.WPF.Application;
using PhoneAssistant.WPF.Features.AddItem;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace PhoneAssistant.Tests.Features.AddItem;
public partial class AddItemViewModelTests
{
    private readonly AutoMocker _mocker = new AutoMocker();
    private readonly AddItemViewModel _sut;

    public AddItemViewModelTests()
    {
        _sut = _mocker.CreateInstance<AddItemViewModel>();
    }

    [Test]
    public async Task AddItemViewModel_DefaultOEMAndModelAsync()
    {
        await Assert.That((Manufacturer)_sut.OEM).IsEqualTo(Manufacturer.Apple);
        await Assert.That(_sut.Model).IsEqualTo("iPhone SE 2022");
    }

    [Test]
    [Arguments(Manufacturer.Apple,"iPhone SE 2022")]
    [Arguments(Manufacturer.Nokia, "110 4G")]
    [Arguments(Manufacturer.Other, "")]
    [Arguments(Manufacturer.Samsung, "A32")]
    public async Task OnOEMChanged_ShouldChangeModelAsync(Manufacturer oem, string model)
    {
        _sut.OEM = oem;

        await Assert.That(_sut.Model).IsEqualTo(model);
    }

    [Test]
    public async Task CanSavePhone_ShouldBeEnabled_WhenNoErrors_WithPhoneHasSimAsync()
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
        await Assert.That(errors).IsEmpty();
        await Assert.That(_sut.CanSavePhone()).IsTrue();
        _mocker.VerifyAll();
    }

    [Test]
    public async Task CanSavePhone_ShouldBeEnabled_WhenNoErrors_WithPhoneInStockAsync()
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
        await Assert.That(errors).IsEmpty();
        await Assert.That(_sut.CanSavePhone()).IsTrue();
        _mocker.VerifyAll();
    }

    [Test]
    public async Task CanSavePhone_ShouldBeEnabled_WhenNoErrors_WithPhoneOnlyAsync()
    {
        _sut.Condition = "condition";
        _sut.Imei = "355808981147090";
        _sut.Model = "model";
        _sut.Status = "status";

        IEnumerable<ValidationResult> errors = _sut.GetErrors();
        await Assert.That(errors).IsEmpty();
        await Assert.That(_sut.CanSavePhone()).IsTrue();
    }

    [Test]
    public async Task GetErrors_ShouldContainAssetTagError_WhenAssetTagNotUniqueAsync()
    {
        Mock<IPhonesRepository> _repository = _mocker.GetMock<IPhonesRepository>();
        _repository.Setup(r => r.AssetTagUniqueAsync("MP99999")).ReturnsAsync(false);

        _sut.AssetTag = "MP99999";

        _mocker.VerifyAll();
        IEnumerable<ValidationResult> errors = _sut.GetErrors(nameof(_sut.AssetTag));
        await Assert.That(errors.First().ToString()).IsEqualTo("Asset Tag must be unique");
    }

    [Test]
    [Arguments(ApplicationConstants.StatusDecommissioned)]
    [Arguments(ApplicationConstants.StatusDisposed)]
    public async Task GetErrors_ShouldContainError_WhenDisposalWithDefaultTicketAsync(string status)
    {
        Mock<IUserSettings> setings = _mocker.GetMock<IUserSettings>();
        setings.Setup(s => s.DefaultDecommissionedTicket).Returns(123456);

        _sut.Status = status;

        IEnumerable<ValidationResult> errors = _sut.GetErrors(nameof(_sut.Ticket));
        await Assert.That(errors).IsEmpty();
    }

    [Test]
    [Arguments(ApplicationConstants.StatusDecommissioned)]
    [Arguments(ApplicationConstants.StatusDisposed)]
    public async Task GetErrors_ShouldContainError_WhenDisposalWithNoDefaultTicketAsync(string status)
    {
        _sut.Status = status;

        IEnumerable<ValidationResult> errors = _sut.GetErrors(nameof(_sut.Ticket));
        await Assert.That(errors.First().ToString()).IsEqualTo("Ticket must 6 or 7 digits");
    }

    [Test]
    [Arguments(" ")]
    [Arguments("12345")]
    [Arguments("12345678")]
    [Arguments("1A345")]
    public async Task GetErrors_ShouldContainTicketError_WhenTicketInvalidAsync(string ticket)
    {
        Mock<IPhonesRepository> _repository = _mocker.GetMock<IPhonesRepository>();

        _sut.Ticket = ticket;

        IEnumerable<ValidationResult> errors = _sut.GetErrors(nameof(_sut.Ticket));
        await Assert.That(errors.First().ToString()).IsEqualTo("Ticket must 6 or 7 digits");
    }

    [Test]
    public async Task GetErrors_ShouldContainStatueError_WhenInvalidStatusAssetTagCombinationAsync()
    {
        _sut.AssetTag = null;
        _sut.Status = ApplicationConstants.StatusInStock;

        IEnumerable<ValidationResult> errors = _sut.GetErrors(nameof(_sut.Status));
        await Assert.That(errors.First().ToString()).IsEqualTo("Asset Tag required");
    }

    [Test]
    [Arguments("PC00001")]
    [Arguments("MP00002")]
    public async Task GetErrors_ShouldBeEmpty_WhenAssetTagFormatValidAsync(string assetTag)
    {
        Mock<IPhonesRepository> _repository = _mocker.GetMock<IPhonesRepository>();
        _repository.Setup(r => r.AssetTagUniqueAsync(assetTag)).ReturnsAsync(true);

        _sut.AssetTag = assetTag;

        _mocker.VerifyAll();
        IEnumerable<ValidationResult> errors = _sut.GetErrors(nameof(_sut.AssetTag));
        await Assert.That(errors).IsEmpty();
    }

    [Test]
    public async Task GetErrors_ShouldBeEmpty_WhenAssetTagUniqueAsync()
    {
        Mock<IPhonesRepository> _repository = _mocker.GetMock<IPhonesRepository>();
        _repository.Setup(r => r.AssetTagUniqueAsync("MP99999")).ReturnsAsync(true);

        _sut.AssetTag = "MP99999";

        _mocker.VerifyAll();
        IEnumerable<ValidationResult> errors = _sut.GetErrors(nameof(_sut.AssetTag));
        await Assert.That(errors).IsEmpty();
    }

    [Test]
    public async Task GetErrors_ShouldBeEmpty_WhenIMEISetAsync()
    {
        _sut.Imei = "355808981147090";

        IEnumerable<ValidationResult> errors = _sut.GetErrors(nameof(_sut.Imei));
        await Assert.That(errors).IsEmpty();
    }

    [Test]
    public async Task GetErrors_ShouldBeEmpty_WhenModelSetAsync()
    {
        _sut.Model = "model";

        IEnumerable<ValidationResult> errors = _sut.GetErrors(nameof(_sut.Model));
        await Assert.That(errors).IsEmpty();
    }


    [Test]
    [Arguments(ApplicationConstants.StatusInStock, "MP00001")]
    [Arguments(ApplicationConstants.StatusProduction, null)]
    [Arguments(ApplicationConstants.StatusProduction, "PC00001")]
    [Arguments(ApplicationConstants.StatusInRepair, null)]
    [Arguments(ApplicationConstants.StatusInRepair, "PC00002")]
    public async Task GetErrors_ShouldBeEmpty_WhenValidStatusAssetTagCombinationAsync(string status, string? assetTag)
    {
        Mock<IPhonesRepository> _repository = _mocker.GetMock<IPhonesRepository>();
        _repository.Setup(r => r.AssetTagUniqueAsync(assetTag)).ReturnsAsync(true);

        _sut.AssetTag = assetTag;
        _sut.Status = status;

        IEnumerable<ValidationResult> errors = _sut.GetErrors(nameof(_sut.Status));
        await Assert.That(errors).IsEmpty();

    }

    [Test]
    public async Task GetErrors_ShouldBeEmpty_WhenPhoneNumberNullAsync()
    {
        _sut.PhoneNumber = "07123456789";

        IEnumerable<ValidationResult> errors = _sut.GetErrors(nameof(_sut.PhoneNumber));
        await Assert.That(errors).IsEmpty();
    }

    [Test]
    public async Task GetErrors_ShouldBeEmpty_WhenPhoneNumberSetAsync()
    {
        _sut.PhoneNumber = "07123456789";

        IEnumerable<ValidationResult> errors = _sut.GetErrors(nameof(_sut.PhoneNumber));
        await Assert.That(errors).IsEmpty();
    }

    [Test]
    public async Task GetErrors_ShouldBeEmpty_WhenSimNumberNullAsync()
    {
        _sut.SimNumber = null;

        IEnumerable<ValidationResult> errors = _sut.GetErrors(nameof(_sut.SimNumber));
        await Assert.That(errors).IsEmpty();
    }

    [Test]
    public async Task GetErrors_ShouldBeEmpty_WhenSimNumberSetAsync()
    {
        _sut.SimNumber = "8944122605566849402";

        IEnumerable<ValidationResult> errors = _sut.GetErrors(nameof(_sut.SimNumber));
        await Assert.That(errors).IsEmpty();
    }

    [Test]
    public async Task OnPhoneNumberChanged_ShouldSetSimNumber_WhenSimExistsAsync()
    {
        Mock<IBaseReportRepository> _repository = _mocker.GetMock<IBaseReportRepository>();
        _repository.Setup(r => r.GetSimNumberAsync("07123456789")).ReturnsAsync("sim number");

        _sut.PhoneNumber = "07123456789";

        _mocker.VerifyAll();
        await Assert.That(_sut.SimNumber).IsEqualTo("sim number");
    }

    [Test]
    public async Task PhoneClearCommand_ShouldDisablePhoneSaveAsync()
    {
        _sut.PhoneClearCommand.Execute(null);

        await Assert.That(_sut.CanSavePhone()).IsFalse();
    }

    [Test]
    public async Task PhoneClearCommand_ShouldResetAllPropertiesAsync()
    {
        ArrangeSetAllPhoneProperties();

        _sut.PhoneClearCommand.Execute(null);

        await AssertResetAllPhonePropertiesAsync();
    }

    [Test]
    [Description("Issue #65")]
    public async Task PhoneSaveCommand_WithConditionN_ShouldLogNewAsync()
    {
        _sut.Condition = ApplicationConstants.Conditions[0].Substring(0, 1);
        _sut.Imei = "355808981147090";
        _sut.Model = "model";
        _sut.OEM = Manufacturer.Apple;
        _sut.Status = "status";

        _sut.PhoneSaveCommand.Execute(null);

        var actual = _sut.LogItems.First();
        await Assert.That(actual).Contains("New");
    }

    [Test]
    [Description("Issue #65")]
    public async Task PhoneSaveCommand_WithConditionR_ShouldLogRepurposedAsync()
    {
        _sut.Condition = ApplicationConstants.Conditions[1].Substring(0, 1);
        _sut.Imei = "355808981147090";
        _sut.Model = "model";
        _sut.OEM = Manufacturer.Apple;
        _sut.Status = "status";

        _sut.PhoneSaveCommand.Execute(null);

        var actual = _sut.LogItems.First();
        await Assert.That(actual).Contains("Repurposed");
    }

    [Test]
    public async Task PhoneSaveCommand_WithPhoneAndSim_ShouldCallRepositoryAsync()
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
            OEM = Manufacturer.Apple,
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

        await Assert.That(actual.AssetTag).IsEqualTo(expectedAssetTag);
        await Assert.That(actual.Imei).IsEqualTo(expectedImei);
        await Assert.That(actual.Model).IsEqualTo(expectedModel);
        await Assert.That(actual.PhoneNumber).IsEqualTo(expectedPhoneNumber);
        await Assert.That(actual.SimNumber).IsEqualTo(expectedSimNumber);
        _mocker.VerifyAll();
    }

    [Test]
    public async Task PhoneSaveCommand_WithPhoneOnly_ShouldCallRepositoryAsync()
    {
        const string expectedAssetTag = "MP00001";
        const string expectedImei = "355808981147090";
        const string expectedModel = "model";        
        Phone actual = new()
        {
            Condition = "norr",
            Imei = "imei",
            Model = "model",
            OEM = Manufacturer.Apple,
            Status = "status"
        };
        Mock<IPhonesRepository> _repository = _mocker.GetMock<IPhonesRepository>();
        _repository.Setup(r => r.AssetTagUniqueAsync("MP00001")).ReturnsAsync(true);
        _repository.Setup(r => r.CreateAsync(It.IsAny<Phone>())).Callback<Phone>((p) => actual = p);

        _sut.AssetTag = expectedAssetTag;
        _sut.Imei = expectedImei;
        _sut.Model = expectedModel;

        _sut.PhoneSaveCommand.Execute(null);

        await Assert.That(actual.AssetTag).IsEqualTo(expectedAssetTag);
        await Assert.That(actual.Imei).IsEqualTo(expectedImei);
        await Assert.That(actual.Model).IsEqualTo(expectedModel);
        await Assert.That(actual.PhoneNumber).IsNull();
        await Assert.That(actual.SimNumber).IsNull();
        _mocker.VerifyAll();
    }

    [Test]
    public async Task PhoneSaveCommand_ShouldDisablePhoneSaveAsync()
    {
        _sut.PhoneSaveCommand.Execute(null);

        await Assert.That(_sut.CanSavePhone()).IsFalse();
    }

    [Test]
    public async Task PhoneSaveCommand_ShouldResetAllPhonePropertiesAsync()
    {
        ArrangeSetAllPhoneProperties();

        _sut.PhoneSaveCommand.Execute(null);

        await AssertResetAllPhonePropertiesAsync();
    }

    [Test]
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

    [Test]
    public async Task ValidateImei_ShouldReturnError_WhenIMEIEmptyOrWhiteSpaceAsync()
    {
        ValidationContext ctx = new(_sut, null, null);

        ValidationResult? actual1 = AddItemViewModel.ValidateImeiAsync("", ctx);
        ValidationResult? actual2 = AddItemViewModel.ValidateImeiAsync("  ", ctx);

        await Assert.That(actual1).IsNotNull();
        await Assert.That(actual1!.ErrorMessage).IsEqualTo("IMEI is required");
        await Assert.That(actual2).IsNotNull();
        await Assert.That(actual2!.ErrorMessage).IsEqualTo("IMEI is required");
    }

    [Test]
    public async Task ValidateImei_ShouldReturnError_WhenIMEINotNumericAsync()
    {
        ValidationContext ctx = new(_sut, null, null);

        ValidationResult? actual = AddItemViewModel.ValidateImeiAsync("abc", ctx);

        await Assert.That(actual).IsNotNull();
        await Assert.That(actual!.ErrorMessage).IsEqualTo("IMEI check digit incorrect");
    }

    [Test]
    public async Task ValidateImei_ShouldReturnError_WhenIMEINotUniqueAsync()
    {
        ValidationContext ctx = new(_sut, null, null);
        Mock<IPhonesRepository> _repository = new Mock<IPhonesRepository>();
        _repository = _mocker.GetMock<IPhonesRepository>();
        _repository.Setup(p => p.ExistsAsync("353427866717729")).ReturnsAsync(true);

        ValidationResult? actual = AddItemViewModel.ValidateImeiAsync("353427866717729", ctx);

        _repository.VerifyAll();
        await Assert.That(actual).IsNotNull();
        await Assert.That(actual!.ErrorMessage).IsEqualTo("IMEI must be unique");
    }

    [Test]
    public async Task ValidateIMEI_ShouldReturnError_WhenIMEIInvalidAsync()
    {
        ValidationContext ctx = new(_sut, null, null);

        ValidationResult? actual = AddItemViewModel.ValidateImeiAsync("355808981132899", ctx); // An invalid 15-digit IMEI

        await Assert.That(actual).IsNotNull();
        await Assert.That(actual!.ErrorMessage).IsEqualTo("IMEI check digit incorrect");
    }

    [Test]
    public async Task ValidateIMEI_ShouldReturnValidResult_WhenIMEIValidAsync()
    {
        ValidationContext ctx = new(_sut, null, null);

        ValidationResult? actual = AddItemViewModel.ValidateImeiAsync("355808981132845", ctx); // A valid 15-digit IMEI

        await Assert.That(actual).IsEqualTo(ValidationResult.Success);
    }

    [Test]
    public async Task ValidatePhoneNumber_ShouldReturnError_WhenPhoneNumberNotUniqueAsync()
    {
        ValidationContext ctx = new(_sut, null, null);
        Mock<IPhonesRepository> _repository = new Mock<IPhonesRepository>();
        _repository = _mocker.GetMock<IPhonesRepository>();
        _repository.Setup(p => p.PhoneNumberExistsAsync("07123456789")).ReturnsAsync(true);

        ValidationResult? actual = AddItemViewModel.ValidatePhoneNumber("07123456789", ctx);

        _repository.VerifyAll();
        await Assert.That(actual).IsNotNull();
        await Assert.That(actual!.ErrorMessage).IsEqualTo("Phone Number already linked to phone");
    }

    [Test]
    public async Task ValidatePhoneNumber_ShouldReturnValidResult_WhenPhoneNumberUniqueAsync()
    {
        ValidationContext ctx = new(_sut, null, null);
        Mock<IPhonesRepository> _repository = new Mock<IPhonesRepository>();
        _repository = _mocker.GetMock<IPhonesRepository>();
        _repository.Setup(p => p.PhoneNumberExistsAsync("07123456789")).ReturnsAsync(false);

        ValidationResult? actual = AddItemViewModel.ValidatePhoneNumber("07123456789", ctx);

        _repository.VerifyAll();
        await Assert.That(actual).IsEqualTo(ValidationResult.Success);
    }

    private void ArrangeSetAllPhoneProperties()
    {
        _sut.AssetTag = "MP00000";
        _sut.Condition = "condition";
        _sut.FormerUser = "former user";
        _sut.Imei = "imei";
        //_sut.Model = "model";
        _sut.PhoneNotes = "notes";
        _sut.PhoneNumber = "07123456789";        
        _sut.SimNumber = "8944125605540324743";
        _sut.Status = "status";
        _sut.Ticket = 7654321.ToString();
    }

    private async Task AssertResetAllPhonePropertiesAsync()
    {
        await Assert.That(_sut.AssetTag).IsNull();
        await Assert.That(_sut.Condition).IsEqualTo(ApplicationConstants.Conditions[1].Substring(0, 1));
        await Assert.That(_sut.FormerUser).IsNull();
        await Assert.That(_sut.Imei).IsEqualTo(string.Empty);
        await Assert.That(_sut.Model).IsEqualTo("iPhone SE 2022");
        await Assert.That(_sut.PhoneNotes).IsNull();
        await Assert.That(_sut.PhoneNumber).IsNull();
        await Assert.That((Manufacturer)_sut.OEM).IsEqualTo(Manufacturer.Apple);
        await Assert.That(_sut.Status).IsEqualTo(ApplicationConstants.Statuses[1]);
        await Assert.That(_sut.SimNumber).IsNull();
        await Assert.That(_sut.Ticket).IsNull();
    }
}

