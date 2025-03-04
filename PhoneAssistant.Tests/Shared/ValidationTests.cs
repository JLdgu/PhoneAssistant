using System.ComponentModel.DataAnnotations;

using Moq.AutoMock;

using PhoneAssistant.WPF.Features.AddItem;
using PhoneAssistant.WPF.Features.Sims;
using PhoneAssistant.WPF.Shared;

using Xunit;

namespace PhoneAssistant.Tests.Shared;

public sealed class ValidationTests
{
    [Fact]
    public void ValidateSimNumber_ShouldReturnError_WhenSimNumberNotNumeric()
    {
        object model = new();
        ValidationContext context = new(model);

        ValidationResult? actual = Validation.ValidateSimNumber("abc", context);

        Assert.NotNull(actual);
        Assert.Equal("SIM Number must be 19 digits", actual.ErrorMessage);
    }

    [Fact]
    public void ValidateSimNumber_ShouldReturnError_WhenSimNumberInvalid()
    {
        object model = new();
        ValidationContext context = new(model);

        ValidationResult? actual = Validation.ValidateSimNumber("8944125605540324744", context); // An invalid 15-digit SIM Number

        Assert.NotNull(actual);
        Assert.Equal("SIM Number check digit incorrect", actual.ErrorMessage);
    }

    [Fact]
    public void ValidateSimNumber_ShouldReturnSuccess_WhenSimNumberEmptyOrWhiteSpace()
    {
        object model = new();
        ValidationContext context = new(model);

        ValidationResult? actual1 = Validation.ValidateSimNumber(null, context);
        ValidationResult? actual2 = Validation.ValidateSimNumber("", context);

        Assert.Null(actual1);
        Assert.Null(actual2);
    }

    [Fact]
    public void ValidateSimNumber_ShouldReturnSuccess_WhenSimNumberValid()
    {
        object model = new();
        ValidationContext context = new(model);

        ValidationResult? actual = Validation.ValidateSimNumber("8944125605540324743", context); // A valid 19-digit SimNumber

        Assert.Equal(ValidationResult.Success, actual);
    }

    [Theory]
    [InlineData(" ")]
    [InlineData("12345")]
    [InlineData("12345678")]
    [InlineData("1A345")]
    public void ValidateTicket_ShouldReturnFailure_WhenTicketInvalid(string ticket)
    {
        object model = new();
        ValidationContext context = new(model);

        ValidationResult? result = Validation.ValidateTicket(ticket, context);

        Assert.NotNull(result);
        Assert.Equal("Ticket must 6 or 7 digits", result.ErrorMessage);
    }

    [Theory]
    [InlineData("123456")]
    [InlineData("1234567")]
    public void ValidateTicket_ShouldReturnSuccess_WhenTicketValid(string ticket)
    {
        object model = new();
        ValidationContext context = new(model);

        ValidationResult? result = Validation.ValidateTicket(ticket, context);

        Assert.Equal(ValidationResult.Success, result);
    }

    [Theory]
    [InlineData("Decommissioned")]
    [InlineData("Disposed")]
    public void ValidateTicket_ShouldReturnFailure_WithAddItemVM_WhenDisposalAndTicketNotPresent(string status)
    {
        AutoMocker mocker = new();
        AddItemViewModel model = mocker.CreateInstance<AddItemViewModel>();
        model.Status = status;
        ValidationContext context = new(model);

        ValidationResult? result = Validation.ValidateTicket(null, context);

        Assert.NotNull(result);
        Assert.Equal("Ticket required when disposal", result.ErrorMessage);
    }

    [Fact]
    public void ValidateTicket_ShouldReturnFailure_WithSimsMainVM_WhenTicketNotPresent()
    {
        AutoMocker mocker = new();
        SimsMainViewModel model = mocker.CreateInstance<SimsMainViewModel>();
        ValidationContext context = new(model);

        ValidationResult? result = Validation.ValidateTicket(null, context);

        Assert.NotNull(result);
        Assert.Equal("Ticket required", result.ErrorMessage);
    }
}
