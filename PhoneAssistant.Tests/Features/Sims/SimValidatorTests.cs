using FluentValidation.TestHelper;
using Moq.AutoMock;
using PhoneAssistant.WPF.Features.Sims;

namespace PhoneAssistant.Tests.Features.Sims;

public class SimValidatorTests
{
    [Test]
    [Arguments(null)]
    [Arguments("")]
    public async Task NewUser_should_have_Error_when_NullOrEmpty(string? newUser)
    {
        AutoMocker mocker = new();
        var validator = mocker.CreateInstance<SimValidator>();
        var vm = mocker.CreateInstance<SimsMainViewModel>();
        vm.NewUser = newUser;

        var result = await validator.TestValidateAsync(vm);       

        await Assert.That(result.Errors.Any(e => e.PropertyName == nameof(vm.NewUser) && e.ErrorMessage == "New User required")).IsTrue();
    }

    [Test]
    public async Task NewUser_should_not_have_Error_when_Present()
    {
        AutoMocker mocker = new();
        var validator = mocker.CreateInstance<SimValidator>();
        var vm = mocker.CreateInstance<SimsMainViewModel>();
        vm.NewUser = "Alice";

        var result = await validator.TestValidateAsync(vm);

        await Assert.That(result.Errors.All(e => e.PropertyName != nameof(vm.NewUser))).IsTrue();
    }

    [Test]
    [Arguments("abc")]
    [Arguments("12345")]
    [Arguments("100000000000")]
    public async Task PhoneNumber_should_have_Error_when_invalid_format_or_out_of_range(string phoneNumber)
    {
        AutoMocker mocker = new();
        var validator = mocker.CreateInstance<SimValidator>();
        var vm = mocker.CreateInstance<SimsMainViewModel>();
        vm.PhoneNumber = phoneNumber;

        var result = await validator.TestValidateAsync(vm);

        await Assert.That(result.Errors.Any(e => e.PropertyName == nameof(vm.PhoneNumber) && e.ErrorMessage == "Phone Number must be 10 or 11 digits")).IsTrue();
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    public async Task PhoneNumber_should_have_Error_when_NullOrEmpty(string? phoneNumber)
    {
        AutoMocker mocker = new();
        var validator = mocker.CreateInstance<SimValidator>();
        var vm = mocker.CreateInstance<SimsMainViewModel>();
        vm.PhoneNumber = phoneNumber;

        var result = await validator.TestValidateAsync(vm);

        await Assert.That(result.Errors.Any(e => e.PropertyName == nameof(vm.PhoneNumber) && e.ErrorMessage == "Phone Number required")).IsTrue();
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
        AutoMocker mocker = new();
        var validator = mocker.CreateInstance<SimValidator>();
        var vm = mocker.CreateInstance<SimsMainViewModel>();
        vm.SimNumber = simNumber;

        var result = await validator.TestValidateAsync(vm);

        await Assert.That(result.Errors.Any(e => e.PropertyName == nameof(vm.SimNumber) && e.ErrorMessage == "SIM Number must be 12 or 19 digits")).IsTrue();
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    public async Task SimNumber_should_have_Error_when_NullOrEmpty(string? simNumber)
    {
        AutoMocker mocker = new();
        var validator = mocker.CreateInstance<SimValidator>();
        var vm = mocker.CreateInstance<SimsMainViewModel>();
        vm.SimNumber = simNumber;

        var result = await validator.TestValidateAsync(vm);

        await Assert.That(result.Errors.Any(e => e.PropertyName == nameof(vm.SimNumber) && e.ErrorMessage == "SIM Number required")).IsTrue();
    }

    [Test]
    [Arguments("293342802663")]
    [Arguments("475334494637")]
    [Arguments("8944125605569171710")]
    public async Task SimNumber_should_not_have_Error_valid(string simNumber)
    {
        AutoMocker mocker = new();
        var validator = mocker.CreateInstance<SimValidator>();
        var vm = mocker.CreateInstance<SimsMainViewModel>();
        vm.SimNumber = simNumber;

        var result = await validator.TestValidateAsync(vm);

        await Assert.That(result.Errors.All(e => e.PropertyName != nameof(vm.SimNumber))).IsTrue();
    }

    [Test]
    [Arguments("abc")]
    [Arguments("12345")]
    [Arguments("10000000")]
    public async Task Ticket_should_have_Error_when_invalid_format_or_out_of_range(string ticket)
    {
        AutoMocker mocker = new();
        var validator = mocker.CreateInstance<SimValidator>();
        var vm = mocker.CreateInstance<SimsMainViewModel>();
        vm.Ticket = ticket;

        var result = await validator.TestValidateAsync(vm);

        await Assert.That(result.Errors.Any(e => e.PropertyName == nameof(vm.Ticket) && e.ErrorMessage == "Ticket must 6 or 7 digits")).IsTrue();
    }


    [Test]
    [Arguments(null)]
    [Arguments("")]
    public async Task Ticket_should_have_Error_when_NullOrEmpty(string? ticket)
    {
        AutoMocker mocker = new();
        var validator = mocker.CreateInstance<SimValidator>();
        var vm = mocker.CreateInstance<SimsMainViewModel>();
        vm.Ticket = ticket;

        var result = await validator.TestValidateAsync(vm);

        await Assert.That(result.Errors.Any(e => e.PropertyName == nameof(vm.Ticket) && e.ErrorMessage == "Ticket required")).IsTrue();
    }

    [Test]
    [Arguments("123456")]
    [Arguments("1234567")]
    public async Task Ticket_Should_not_have_Error_when_valid(string ticket)
    {
        AutoMocker mocker = new();
        var validator = mocker.CreateInstance<SimValidator>();
        var vm = mocker.CreateInstance<SimsMainViewModel>();
        vm.Ticket = ticket;

        var result = await validator.TestValidateAsync(vm);

        await Assert.That(result.Errors.All(e => e.PropertyName != nameof(vm.Ticket))).IsTrue();
    }
}
