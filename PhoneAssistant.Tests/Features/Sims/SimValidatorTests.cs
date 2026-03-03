using FluentValidation.TestHelper;
using Moq.AutoMock;
using PhoneAssistant.WPF.Features.Sims;

namespace PhoneAssistant.Tests.Features.Sims;

internal sealed  class SimValidatorTests
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

    // Phone number validation is tested in ValidationRules_PhoneNumberTests
    // Sim number validation is tested in ValidationRules_SimNumberTests
    // Ticket validation is tested in ValidationRules_TicketTests - except for the case when ticket is required
    [Test]
    [Arguments(null)]
    [Arguments("")]
    public async Task Ticket_should_have_Error_when_NullOrEmpty(string? ticket)
    {
        _vm.Ticket = ticket;

        var result = await _validator.TestValidateAsync(_vm);

        await Assert.That(result.Errors.Any(e => e.PropertyName == nameof(_vm.Ticket) && e.ErrorMessage == "Ticket required")).IsTrue();
    }
}
