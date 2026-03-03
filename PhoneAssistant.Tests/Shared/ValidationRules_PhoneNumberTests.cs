using FluentValidation;
using FluentValidation.TestHelper;
using PhoneAssistant.WPF.Shared;

namespace PhoneAssistant.Tests.Shared;

internal sealed class ValidationRules_PhoneNumberTests
{
    private sealed class TestModel
    {
        public string? PhoneNumber { get; set; }
    }

    private sealed class TestModelValidator : AbstractValidator<TestModel>
    {
        public TestModelValidator()
        {
            RuleFor(x => x.PhoneNumber).PhoneNumberRules();
        }
    }

    private readonly TestModelValidator _validator = new();

    [Test]
    [Arguments("abc")]
    [Arguments("12345")]
    [Arguments("100000000000")]
    public async Task PhoneNumber_should_have_Error_when_invalid_format_or_out_of_range(string phoneNumber)
    {
        TestModel model = new() { PhoneNumber = phoneNumber };

        var result = await _validator.TestValidateAsync(model);

        result.ShouldHaveValidationErrorFor(model => model.PhoneNumber).WithErrorMessage("Phone Number must be 10 or 11 digits");

    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    public async Task PhoneNumber_should_have_Error_when_NullOrEmpty(string? phoneNumber)
    {
        TestModel model = new() { PhoneNumber = phoneNumber };

        var result = await _validator.TestValidateAsync(model);

        result.ShouldHaveValidationErrorFor(model => model.PhoneNumber).WithErrorMessage("Phone Number required");
    }

    [Test]
    [Arguments("0123456789")]   // 10 digits, starts with 0
    [Arguments("01234567890")]  // 11 digits, starts with 0
    public async Task PhoneNumber_should_not_have_Error_when_valid(string phoneNumber)
    {
        TestModel model = new() { PhoneNumber = phoneNumber };

        var result = await _validator.TestValidateAsync(model);

        result.ShouldNotHaveValidationErrorFor(model => model.PhoneNumber);
    }
}