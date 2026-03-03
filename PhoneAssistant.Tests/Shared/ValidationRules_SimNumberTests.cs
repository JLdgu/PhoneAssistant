using FluentValidation;
using FluentValidation.TestHelper;
using PhoneAssistant.WPF.Shared;

namespace PhoneAssistant.Tests.Shared;

internal sealed class ValidationRules_SimNumberTests
{
    private sealed class TestModel
    {
        public string? SimNumber { get; set; }
    }

    private sealed class TestModelValidator : AbstractValidator<TestModel>
    {
        public TestModelValidator()
        {
            RuleFor(x => x.SimNumber).SimNumberRules();
        }
    }

    private readonly TestModelValidator _validator = new();

    [Test]
    [Arguments("abc")]
    [Arguments("12345")]
    [Arguments("100000000000")]
    [Arguments("894412560556")]
    [Arguments("2933428026631111111")]
    [Arguments("4753344946372222222")]
    public async Task SimNumber_should_have_Error_when_invalid_format_or_out_of_range(string simNumber)
    {
        var model = new TestModel { SimNumber = simNumber };

        var result = await _validator.TestValidateAsync(model);

        result.ShouldHaveValidationErrorFor(model => model.SimNumber).WithErrorMessage("SIM Number must be 12 or 19 digits");
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    public async Task SimNumber_should_have_Error_when_NullOrEmpty(string? simNumber)
    {
        var model = new TestModel { SimNumber = simNumber };

        var result = await _validator.TestValidateAsync(model);

        result.ShouldHaveValidationErrorFor(model => model.SimNumber).WithErrorMessage("SIM Number required");
    }

    [Test]
    [Arguments("293342802663")]
    [Arguments("475334494637")]
    [Arguments("8944125605569171710")]
    public async Task SimNumber_should_not_have_Error_valid(string simNumber)
    {
        var model = new TestModel { SimNumber = simNumber };

        var result = await _validator.TestValidateAsync(model);

        result.ShouldNotHaveValidationErrorFor(model => model.SimNumber);
    }
}
