using CommunityToolkit.Mvvm.Messaging;
using FluentValidation;
using Moq;
using Moq.AutoMock;
using PhoneAssistant.Model;
using PhoneAssistant.Tests.Shared;
using PhoneAssistant.WPF.Features.AddItem;
using System.ComponentModel;

namespace PhoneAssistant.Tests.Features.AddItem;
public partial class AddItemViewModelTests
{
    private readonly AutoMocker _mocker = new();

    private Mock<IPhonesRepository> MockValidator()
    {
        Mock<IPhonesRepository> phones = _mocker.GetMock<IPhonesRepository>();
        var validator = new AddItemValidator(phones.Object);
        var serviceProviderMock = _mocker.GetMock<IServiceProvider>();
        serviceProviderMock
            .Setup(sp => sp.GetService(typeof(IValidator<AddItemViewModel>)))
            .Returns(validator);
        return phones;
    }

    [Test]
    public async Task AddItemViewModel_DefaultOEMAndModel()
    {
        _ = MockValidator();
        AddItemViewModel sut = _mocker.CreateInstance<AddItemViewModel>();

        await Assert.That((Manufacturer)sut.OEM).IsEqualTo(Manufacturer.Apple);
        await Assert.That(sut.Model).IsEqualTo("iPhone SE 2022");
    }

    [Test]
    public async Task CanSavePhone_ShouldBeEnabled_WhenNoErrors_WithPhoneHasSim()
    {
        Mock<IBaseReportRepository> sims = _mocker.GetMock<IBaseReportRepository>();
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
        sims.Setup(r => r.GetSimNumberAsync("07123456789")).ReturnsAsync((string)null);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
        _ = MockValidator();
        AddItemViewModel sut = _mocker.CreateInstance<AddItemViewModel>();

        sut.Condition = "condition";
        sut.Imei = "355808981147090";
        sut.Model = "model";
        sut.PhoneNumber = "07123456789";
        sut.Status = "status";

        await Assert.That(sut.HasErrors).IsFalse();
        await Assert.That(sut.CanSavePhone()).IsTrue();
        _mocker.VerifyAll();
    }

    [Test]
    public async Task CanSavePhone_ShouldBeEnabled_WhenNoErrors_WithPhoneInStockAsync()
    {
        var phones = MockValidator();
        phones.Setup(r => r.AssetTagUniqueAsync("MP00001")).ReturnsAsync(true);
        AddItemViewModel sut = _mocker.CreateInstance<AddItemViewModel>();

        sut.AssetTag = "MP00001";
        sut.Condition = "condition";
        sut.Imei = "355808981147090";
        sut.Model = "model";
        sut.Status = ApplicationConstants.StatusInStock;

        await Assert.That(sut.HasErrors).IsFalse();
        await Assert.That(sut.CanSavePhone()).IsTrue();
        _mocker.VerifyAll();
    }

    [Test]
    public async Task CanSavePhone_ShouldBeEnabled_WhenNoErrors_WithPhoneOnly()
    {
        _ = MockValidator();
        AddItemViewModel sut = _mocker.CreateInstance<AddItemViewModel>();

        sut.Condition = "condition";
        sut.Imei = "355808981147090";
        sut.Model = "model";
        sut.Status = "status";

        await Assert.That(sut.HasErrors).IsFalse();
        await Assert.That(sut.CanSavePhone()).IsTrue();
    }

    //[Test]
    //public async Task Imei_should_not_have_Error_when_present()
    //{
    //    AddItemViewModel sut = _mocker.CreateInstance<AddItemViewModel>();

    //    sut.Imei = "355808981147090";

    //    await Assert.That(sut.GetErrors(nameof(sut.Imei))).IsEmpty();
    //}

    //[Test]
    //[Arguments(null)]
    //[Arguments("")]
    //public async Task PhoneNumber_should_not_have_Error_when_Null_or_empty(string? actual)
    //{
    //    AddItemViewModel sut = _mocker.CreateInstance<AddItemViewModel>();

    //    sut.PhoneNumber = actual;

    //    await Assert.That(sut.GetErrors(nameof(sut.PhoneNumber))).IsEmpty();
    //}

    //[Test]
    //public async Task PhoneNumber_should_not_have_Error_when_present()
    //{
    //    AddItemViewModel sut = _mocker.CreateInstance<AddItemViewModel>();

    //    sut.PhoneNumber = "07123456789";

    //    await Assert.That(sut.GetErrors(nameof(sut.PhoneNumber))).IsEmpty();
    //}

    //[Test]
    //public async Task GetErrors_ShouldBeEmpty_WhenSimNumberNullAsync()
    //{
    //    AddItemViewModel sut = _mocker.CreateInstance<AddItemViewModel>();

    //    sut.SimNumber = null;

    //    await Assert.That(sut.GetErrors(nameof(sut.SimNumber))).IsEmpty();
    //}

    //[Test]
    //public async Task GetErrors_ShouldBeEmpty_WhenSimNumberSetAsync()
    //{
    //    AddItemViewModel sut = _mocker.CreateInstance<AddItemViewModel>();

    //    sut.SimNumber = "8944122605566849402";

    //    await Assert.That(sut.GetErrors(nameof(sut.SimNumber))).IsEmpty();
    //}

    [Test]
    public async Task LoadAsync_ShouldReturn_TaskCompleted()
    {
        _ = MockValidator();
        AddItemViewModel sut = _mocker.CreateInstance<AddItemViewModel>();

        Task result =  sut.LoadAsync();

        await Assert.That(result).IsCompleted();
    }

    [Test]
    [Arguments(Manufacturer.Apple, "iPhone SE 2022")]
    [Arguments(Manufacturer.Nokia, "110 4G")]
    [Arguments(Manufacturer.Other, "")]
    [Arguments(Manufacturer.Samsung, "A32")]
    public async Task OEM_should_change_Model(Manufacturer oem, string model)
    {
        _ = MockValidator();
        AddItemViewModel sut = _mocker.CreateInstance<AddItemViewModel>();

        sut.OEM = oem;

        await Assert.That(sut.Model).IsEqualTo(model);
    }

    [Test]
    public async Task OnPhoneNumberChanged_ShouldSetSimNumber_WhenSimExistsAsync()
    {
        Mock<IBaseReportRepository> repository = _mocker.GetMock<IBaseReportRepository>();
        repository.Setup(r => r.GetSimNumberAsync("07123456789")).ReturnsAsync("sim number");
        _ = MockValidator();
        AddItemViewModel sut = _mocker.CreateInstance<AddItemViewModel>();

        sut.PhoneNumber = "07123456789";

        _mocker.VerifyAll();
        await Assert.That(sut.SimNumber).IsEqualTo("sim number");
    }

    [Test]
    public async Task PhoneClearCommand_ShouldDisablePhoneSaveAsync()
    {
        _ = MockValidator();
        AddItemViewModel sut = _mocker.CreateInstance<AddItemViewModel>();

        sut.PhoneClearCommand.Execute(null);

        await Assert.That(sut.CanSavePhone()).IsFalse();
    }

    [Test]
    public async Task PhoneClearCommand_ShouldResetAllPropertiesAsync()
    {
        _ = MockValidator();
        AddItemViewModel sut = _mocker.CreateInstance<AddItemViewModel>();

        ArrangeSetAllPhoneProperties(sut);

        sut.PhoneClearCommand.Execute(null);

        await AssertResetAllPhonePropertiesAsync(sut);
    }

    [Test]
    [Description("Issue #65")]
    public async Task PhoneSaveCommand_should_Log_New_when_Condition_N()
    {
        _ = MockValidator();
        AddItemViewModel sut = _mocker.CreateInstance<AddItemViewModel>();

        sut.Condition = ApplicationConstants.Conditions[0][..1];
        sut.Imei = "355808981147090";
        sut.Model = "model";
        sut.OEM = Manufacturer.Apple;
        sut.Status = "status";

        sut.PhoneSaveCommand.Execute(null);

        var actual = sut.LogItems.First();
        await Assert.That(actual).Contains("New");
    }
       
    [Test]
    [Description("Issue #65")]
    public async Task PhoneSaveCommand_should_Log_Repurposed_when_Condition_R()
    {
        _ = MockValidator();
        AddItemViewModel sut = _mocker.CreateInstance<AddItemViewModel>();

        sut.Condition = ApplicationConstants.Conditions[1].Substring(0, 1);
        sut.Imei = "355808981147090";
        sut.Model = "model";
        sut.OEM = Manufacturer.Apple;
        sut.Status = "status";

        sut.PhoneSaveCommand.Execute(null);

        var actual = sut.LogItems.First();
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
        var repository = MockValidator();
        repository.Setup(r => r.AssetTagUniqueAsync("MP00001")).ReturnsAsync(true);
        repository.Setup(r => r.CreateAsync(It.IsAny<Phone>())).Callback<Phone>((p) => actual = p);
        AddItemViewModel sut = _mocker.CreateInstance<AddItemViewModel>();

        sut.AssetTag = expectedAssetTag;
        sut.Imei = expectedImei;
        sut.Model = expectedModel;
        sut.PhoneNumber = expectedPhoneNumber;
        sut.SimNumber = expectedSimNumber;

        sut.PhoneSaveCommand.Execute(null);

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
        var repository  = MockValidator();
        repository.Setup(r => r.AssetTagUniqueAsync("MP00001")).ReturnsAsync(true);
        repository.Setup(r => r.CreateAsync(It.IsAny<Phone>())).Callback<Phone>((p) => actual = p);
        AddItemViewModel sut = _mocker.CreateInstance<AddItemViewModel>();

        sut.AssetTag = expectedAssetTag;
        sut.Imei = expectedImei;
        sut.Model = expectedModel;

        sut.PhoneSaveCommand.Execute(null);

        await Assert.That(actual.AssetTag).IsEqualTo(expectedAssetTag);
        await Assert.That(actual.Imei).IsEqualTo(expectedImei);
        await Assert.That(actual.Model).IsEqualTo(expectedModel);
        await Assert.That(actual.PhoneNumber).IsNull();
        await Assert.That(actual.SimNumber).IsNull();
        _mocker.VerifyAll();
    }

    [Test]
    public async Task PhoneSaveCommand_hould_disable_PhoneSaveCommand()
    {
        _ = MockValidator();
        AddItemViewModel sut = _mocker.CreateInstance<AddItemViewModel>();

        sut.PhoneSaveCommand.Execute(null);

        await Assert.That(sut.CanSavePhone()).IsFalse();
    }

    [Test]
    public async Task PhoneSaveCommand_should_reset_all_Phone_properties()
    {
        _ = MockValidator();
        AddItemViewModel sut = _mocker.CreateInstance<AddItemViewModel>();
        ArrangeSetAllPhoneProperties(sut);

        sut.PhoneSaveCommand.Execute(null);

        await AssertResetAllPhonePropertiesAsync(sut);
    }

    [Test]
    public void PhoneSaveCommand_ShouldSendPhoneMessage()
    {
        Mock<IMessenger> message = _mocker.GetMock<IMessenger>();
        message.Setup(m => m.Send(It.IsAny<Phone>(), It.IsAny<IsAnyToken>()));
        _ = MockValidator();
        AddItemViewModel sut = _mocker.CreateInstance<AddItemViewModel>();

        sut.AssetTag = "MP00001";
        sut.Imei = "355808981147090";
        sut.Model = "model";

        sut.PhoneSaveCommand.Execute(null);

        message.Verify(x => x.Send(It.IsAny<Phone>(), It.IsAny<IsAnyToken>()), Times.Once);
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    public async Task Ticket_should_not_have_Error_when_Null_or_empty(string? actual)
    {
        _ = MockValidator();
        AddItemViewModel sut = _mocker.CreateInstance<AddItemViewModel>();

        sut.Ticket = actual;

        await Assert.That(sut.GetErrors(nameof(sut.Ticket))).IsEmpty();
    }

    private static void ArrangeSetAllPhoneProperties(AddItemViewModel sut)
    {
        sut.AssetTag = "MP00000";
        sut.Condition = "condition";
        sut.FormerUser = "former user";
        sut.Imei = "imei";
        sut.PhoneNotes = "notes";
        sut.PhoneNumber = "07123456789";        
        sut.SimNumber = "8944125605540324743";
        sut.Status = "status";
        sut.Ticket = 7654321.ToString();
    }

    private static async Task AssertResetAllPhonePropertiesAsync(AddItemViewModel sut)
    {
        await Assert.That(sut.AssetTag).IsNull();
        await Assert.That(sut.Condition).IsEqualTo(ApplicationConstants.Conditions[1].Substring(0, 1));
        await Assert.That(sut.FormerUser).IsNull();
        await Assert.That(sut.Imei).IsEqualTo(string.Empty);
        await Assert.That(sut.Model).IsEqualTo("iPhone SE 2022");
        await Assert.That(sut.PhoneNotes).IsNull();
        await Assert.That(sut.PhoneNumber).IsNull();
        await Assert.That((Manufacturer)sut.OEM).IsEqualTo(Manufacturer.Apple);
        await Assert.That(sut.Status).IsEqualTo(ApplicationConstants.Statuses[1]);
        await Assert.That(sut.SimNumber).IsNull();
        await Assert.That(sut.Ticket).IsNull();
    }
}

