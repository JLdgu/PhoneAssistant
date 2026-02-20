using FluentValidation.TestHelper;

using Moq;
using Moq.AutoMock;

using PhoneAssistant.Model;
using PhoneAssistant.WPF.Features.AddItem;

namespace PhoneAssistant.Tests.Features.AddItem;

public sealed class AddItemValidatorTests
{
    private readonly AutoMocker _mocker;
    private readonly AddItemValidator _validator;
    private readonly AddItemViewModel _sut;

    public AddItemValidatorTests()
    {
        _mocker = new AutoMocker();
        _validator = _mocker.CreateInstance<AddItemValidator>();
        _sut = _mocker.CreateInstance<AddItemViewModel>();
    }

    [Test]
    [Arguments("C000001")]
    [Arguments("PC0001")]
    [Arguments("M0P0002")]
    [Arguments("MP000022")]
    public async Task AssetTag_should_have_Error_when_invalid_format(string actual)
    {
        _sut.AssetTag = actual;

        var result = await _validator.TestValidateAsync(_sut);

        result.ShouldHaveValidationErrorFor(model => model.AssetTag).WithErrorMessage("Invalid format");
    }

    [Test]
    public async Task AssetTag_should_have_Error_when_empty_and_Status_InStock()
    {
        Mock<IPhonesRepository> repository = _mocker.GetMock<IPhonesRepository>();
        repository.Setup(r => r.AssetTagUniqueAsync(It.IsAny<string>())).ReturnsAsync(true);
        _sut.Status = ApplicationConstants.StatusInStock;

        var result = await _validator.TestValidateAsync(_sut);

        result.ShouldHaveValidationErrorFor(model => model.AssetTag).WithErrorMessage("Asset Tag required");
    }

    [Test]
    public async Task AssetTag_should_have_Error_when_not_unique()
    {
        Mock<IPhonesRepository> repository = _mocker.GetMock<IPhonesRepository>();
        repository.Setup(r => r.AssetTagUniqueAsync("MP99999")).ReturnsAsync(false);
        _sut.AssetTag = "MP99999";

        var result = await _validator.TestValidateAsync(_sut);

        _mocker.VerifyAll();
        result.ShouldHaveValidationErrorFor(model => model.AssetTag).WithErrorMessage("Asset Tag must be unique");
    }
    
    [Test]
    [Arguments("PC00001")]
    [Arguments("MP00002")]
    public async Task AssetTag_should_not_have_Error_when_format_valid(string actual)
    {
        Mock<IPhonesRepository> repository = _mocker.GetMock<IPhonesRepository>();
        repository.Setup(r => r.AssetTagUniqueAsync(It.IsAny<string>())).ReturnsAsync(true);
        _sut.AssetTag = actual;

        var result = await _validator.TestValidateAsync(_sut);

        _mocker.VerifyAll();
        result.ShouldNotHaveValidationErrorFor(model => model.AssetTag);
    }

    [Test]
    public async Task AssetTag_should_not_have_Error_when_unique()
    {
        Mock<IPhonesRepository> repository = _mocker.GetMock<IPhonesRepository>();
        repository.Setup(r => r.AssetTagUniqueAsync("MP99999")).ReturnsAsync(true);
        _sut.AssetTag = "MP99999";

        var result = await _validator.TestValidateAsync(_sut);

        _mocker.VerifyAll();
        result.ShouldNotHaveValidationErrorFor(model => model.AssetTag);
    }

    [Test]
    public async Task Imei_should_have_Error_when_check_digit_invalid()
    {
        _sut.Imei = "355808981132899";

        var result = await _validator.TestValidateAsync(_sut);

        result.ShouldHaveValidationErrorFor(model => model.Imei).WithErrorMessage("IMEI check digit incorrect");
    }

    [Test]
    [Arguments("A23456789012345")]
    [Arguments("12345")]
    [Arguments("1000000000000000")]
    public async Task Imei_should_have_Error_when_invalid_format_or_out_of_range(string actual)
    {
        _sut.Imei = actual;

        var result = await _validator.TestValidateAsync(_sut);

        result.ShouldHaveValidationErrorFor(r => r.Imei).WithErrorMessage("IMEI must be 15 digits");
    }

    [Test]
    public async Task Imei_should_have_Error_when_not_unique()
    {
        Mock<IPhonesRepository> repository = _mocker.GetMock<IPhonesRepository>();
        repository.Setup(p => p.ExistsAsync("353427866717729")).ReturnsAsync(true);
        _sut.Imei = "353427866717729";

        var result = await _validator.TestValidateAsync(_sut);

        repository.VerifyAll();
        result.ShouldHaveValidationErrorFor(r => r.Imei).WithErrorMessage("IMEI must be unique");
    }


    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("  ")]
    public async Task Imei_should_have_Error_when_NullOrEmpty(string? actual)
    {
#pragma warning disable CS8601 // Possible null reference assignment.
        _sut.Imei = actual;
#pragma warning restore CS8601 // Possible null reference assignment.

        var result = await _validator.TestValidateAsync(_sut);

        result.ShouldHaveValidationErrorFor(r => r.Imei).WithErrorMessage("IMEI required");
    }

    [Test]
    public async Task Imei_should_not_have_Error_when_format_valid_and_unique()
    {
        _sut.Imei = "355808981147090";

        var result = await _validator.TestValidateAsync(_sut);

        await Assert.That(result.Errors.All(e => e.PropertyName != nameof(_sut.Imei))).IsTrue();
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("  ")]
    public async Task Model_should_have_Error_when_NullOrEmpty(string? actual)
    {
#pragma warning disable CS8601 // Possible null reference assignment.
        _sut.Model = actual;
#pragma warning restore CS8601 // Possible null reference assignment.

        var result = await _validator.TestValidateAsync(_sut);

        result.ShouldHaveValidationErrorFor(model => model.Model).WithErrorMessage("Model required");
    }

    [Test]
    public async Task Model_should_not_have_Error_when_present()
    {
        _sut.Model = "model";

        var result = await _validator.TestValidateAsync(_sut);

        result.ShouldNotHaveValidationErrorFor(model => model.Model);
    }
}
