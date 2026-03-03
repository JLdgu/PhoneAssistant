using FluentValidation;
using FluentValidation.TestHelper;
using PhoneAssistant.WPF.Shared;

namespace PhoneAssistant.Tests.Shared;

internal sealed class ValidationRules_TicketTests
{
    private sealed class TestModel
    {
        public string? Ticket { get; set; }
    }

    private sealed class TestModelValidator : AbstractValidator<TestModel>
    {
        public TestModelValidator()
        {
            RuleFor(x => x.Ticket).TicketRules();
        }
    }

    private readonly TestModelValidator _validator = new();

    [Test]
    [Arguments("abc")]
    [Arguments("12345")]
    [Arguments("1A345")]
    [Arguments("10000000")]
    public async Task Ticket_should_have_Error_when_invalid_format_or_out_of_range(string ticket)
    {
        TestModel model = new() { Ticket = ticket };

        var result = await _validator.TestValidateAsync(model);

        result.ShouldHaveValidationErrorFor(model => model.Ticket).WithErrorMessage("Ticket must 6 or 7 digits");
    }

    [Test]
    [Arguments("123456")]
    [Arguments("1234567")]
    public async Task Ticket_Should_not_have_Error_when_valid(string ticket)
    {
        TestModel model = new() { Ticket = ticket };

        var result = await _validator.TestValidateAsync(model);

        result.ShouldNotHaveValidationErrorFor(model => model.Ticket);
    }

}
