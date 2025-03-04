using System.ComponentModel.DataAnnotations;

using Moq.AutoMock;

using PhoneAssistant.WPF.Features.AddItem;
using PhoneAssistant.WPF.Features.Sims;
using PhoneAssistant.WPF.Shared;

namespace PhoneAssistant.Tests.Shared;

public sealed class ValidationTests
{
    [Test]
    public async Task ValidateSimNumber_ShouldReturnError_WhenSimNumberNotNumericAsync()
    {
        object model = new();
        ValidationContext context = new(model);

        ValidationResult? actual = Validation.ValidateSimNumber("abc", context);

        await Assert.That(actual).IsNotNull();
        await Assert.That(actual!.ErrorMessage).IsEqualTo("SIM Number must be 19 digits");
    }

    [Test]
    public async Task ValidateSimNumber_ShouldReturnError_WhenSimNumberInvalidAsync()
    {
        object model = new();
        ValidationContext context = new(model);

        ValidationResult? actual = Validation.ValidateSimNumber("8944125605540324744", context); // An invalid 15-digit SIM Number

        await Assert.That(actual).IsNotNull();
        await Assert.That(actual!.ErrorMessage).IsEqualTo("SIM Number check digit incorrect");
    }

    [Test]
    public async Task ValidateSimNumber_ShouldReturnSuccess_WhenSimNumberEmptyOrWhiteSpaceAsync()
    {
        object model = new();
        ValidationContext context = new(model);

        ValidationResult? actual1 = Validation.ValidateSimNumber(null, context);
        ValidationResult? actual2 = Validation.ValidateSimNumber("", context);

        await Assert.That(actual1).IsNull();
        await Assert.That(actual2).IsNull();
    }

    [Test]
    public async Task ValidateSimNumber_ShouldReturnSuccess_WhenSimNumberValidAsync()
    {
        object model = new();
        ValidationContext context = new(model);

        ValidationResult? actual = Validation.ValidateSimNumber("8944125605540324743", context); // A valid 19-digit SimNumber

        await Assert.That(actual).IsEqualTo(ValidationResult.Success);
    }

    [Test]
    [Arguments(" ")]
    [Arguments("12345")]
    [Arguments("12345678")]
    [Arguments("1A345")]
    public async Task ValidateTicket_ShouldReturnFailure_WhenTicketInvalidAsync(string ticket)
    {
        object model = new();
        ValidationContext context = new(model);

        ValidationResult? actual = Validation.ValidateTicket(ticket, context);

        await Assert.That(actual).IsNotNull();
        await Assert.That(actual!.ErrorMessage).IsEqualTo("Ticket must 6 or 7 digits");
    }

    [Test]
    [Arguments("123456")]
    [Arguments("1234567")]
    public async Task ValidateTicket_ShouldReturnSuccess_WhenTicketValidAsync(string ticket)
    {
        object model = new();
        ValidationContext context = new(model);

        ValidationResult? actual = Validation.ValidateTicket(ticket, context);

        await Assert.That(actual).IsEqualTo(ValidationResult.Success);
    }

    [Test]
    [Arguments("Decommissioned")]
    [Arguments("Disposed")]
    public async Task ValidateTicket_ShouldReturnFailure_WithAddItemVM_WhenDisposalAndTicketNotPresentAsync(string status)
    {
        AutoMocker mocker = new();
        AddItemViewModel model = mocker.CreateInstance<AddItemViewModel>();
        model.Status = status;
        ValidationContext context = new(model);

        ValidationResult? actual = Validation.ValidateTicket(null, context);

        await Assert.That(actual).IsNotNull();
        await Assert.That(actual!.ErrorMessage).IsEqualTo("Ticket required when disposal");
    }

    [Test]
    public async Task ValidateTicket_ShouldReturnFailure_WithSimsMainVM_WhenTicketNotPresentAsync()
    {
        AutoMocker mocker = new();
        SimsMainViewModel model = mocker.CreateInstance<SimsMainViewModel>();
        ValidationContext context = new(model);

        ValidationResult? actual = Validation.ValidateTicket(null, context);

        await Assert.That(actual).IsNotNull();
        await Assert.That(actual!.ErrorMessage).IsEqualTo("Ticket required");
    }
}
