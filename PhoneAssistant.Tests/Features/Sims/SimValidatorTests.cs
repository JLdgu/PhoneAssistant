using FluentValidation.TestHelper;
using Moq.AutoMock;
using PhoneAssistant.WPF.Features.Sims;

namespace PhoneAssistant.Tests.Features.Sims;

public class SimValidatorTests
{
    private readonly AutoMocker _mocker;
    private readonly SimValidator _validator;
    private readonly SimsMainViewModel _vm;

    public SimValidatorTests()
    {
        _mocker = new AutoMocker();
        _validator = _mocker.CreateInstance<SimValidator>();
        _vm = _mocker.CreateInstance<SimsMainViewModel>();
    }
    [Test]
    [Arguments(null)]
    [Arguments("")]
    public async Task NewUser_should_have_Error_when_NullOrEmpty(string? newUser)
    {
        _vm.NewUser = newUser;

        var result = await _validator.TestValidateAsync(_vm);       

        await Assert.That(result.Errors.Any(e => e.PropertyName == nameof(_vm.NewUser) && e.ErrorMessage == "New User required")).IsTrue();
    }

    [Test]
    public async Task NewUser_should_not_have_Error_when_Present()
    {
        _vm.NewUser = "Alice";

        var result = await _validator.TestValidateAsync(_vm);

        await Assert.That(result.Errors.All(e => e.PropertyName != nameof(_vm.NewUser))).IsTrue();
    }

    [Test]
    [Arguments("abc")]
    [Arguments("12345")]
    [Arguments("100000000000")]
    public async Task PhoneNumber_should_have_Error_when_invalid_format_or_out_of_range(string phoneNumber)
    {
        _vm.PhoneNumber = phoneNumber;

        var result = await _validator.TestValidateAsync(_vm);

        await Assert.That(result.Errors.Any(e => e.PropertyName == nameof(_vm.PhoneNumber) && e.ErrorMessage == "Phone Number must be 10 or 11 digits")).IsTrue();
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    public async Task PhoneNumber_should_have_Error_when_NullOrEmpty(string? phoneNumber)
    {
        _vm.PhoneNumber = phoneNumber;

        var result = await _validator.TestValidateAsync(_vm);

        await Assert.That(result.Errors.Any(e => e.PropertyName == nameof(_vm.PhoneNumber) && e.ErrorMessage == "Phone Number required")).IsTrue();
    }

    [Test]
    [Arguments("abc")]
    [Arguments("12345")]
    [Arguments("100000000000")]
    [Arguments("894412560556")]
    [Arguments("2933428026631111111")]
    [Arguments("4753344946372222222")]
    public async Task SimNumber_should_have_Error_when_invalid_format_or_out_of_range(string simNumber)
    {
        _vm.SimNumber = simNumber;

        var result = await _validator.TestValidateAsync(_vm);

        await Assert.That(result.Errors.Any(e => e.PropertyName == nameof(_vm.SimNumber) && e.ErrorMessage == "SIM Number must be 12 or 19 digits")).IsTrue();
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    public async Task SimNumber_should_have_Error_when_NullOrEmpty(string? simNumber)
    {
        _vm.SimNumber = simNumber;

        var result = await _validator.TestValidateAsync(_vm);

        await Assert.That(result.Errors.Any(e => e.PropertyName == nameof(_vm.SimNumber) && e.ErrorMessage == "SIM Number required")).IsTrue();
    }

    [Test]
    [Arguments("293342802663")]
    [Arguments("475334494637")]
    [Arguments("8944125605569171710")]
    public async Task SimNumber_should_not_have_Error_valid(string simNumber)
    {
        _vm.SimNumber = simNumber;

        var result = await _validator.TestValidateAsync(_vm);

        await Assert.That(result.Errors.All(e => e.PropertyName != nameof(_vm.SimNumber))).IsTrue();
    }

    [Test]
    [Arguments("abc")]
    [Arguments("12345")]
    [Arguments("1A345")]
    [Arguments("10000000")]
    public async Task Ticket_should_have_Error_when_invalid_format_or_out_of_range(string ticket)
    {
        _vm.Ticket = ticket;

        var result = await _validator.TestValidateAsync(_vm);

        await Assert.That(result.Errors.Any(e => e.PropertyName == nameof(_vm.Ticket) && e.ErrorMessage == "Ticket must 6 or 7 digits")).IsTrue();
    }


    [Test]
    [Arguments(null)]
    [Arguments("")]
    public async Task Ticket_should_have_Error_when_NullOrEmpty(string? ticket)
    {
        _vm.Ticket = ticket;

        var result = await _validator.TestValidateAsync(_vm);

        await Assert.That(result.Errors.Any(e => e.PropertyName == nameof(_vm.Ticket) && e.ErrorMessage == "Ticket required")).IsTrue();
    }

    [Test]
    [Arguments("123456")]
    [Arguments("1234567")]
    public async Task Ticket_Should_not_have_Error_when_valid(string ticket)
    {
        _vm.Ticket = ticket;

        var result = await _validator.TestValidateAsync(_vm);

        await Assert.That(result.Errors.All(e => e.PropertyName != nameof(_vm.Ticket))).IsTrue();
    }
}
