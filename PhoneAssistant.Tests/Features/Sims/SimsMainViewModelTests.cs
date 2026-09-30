using FluentValidation;
using Moq;
using Moq.AutoMock;
using PhoneAssistant.Model;
using PhoneAssistant.WPF.Features.Sims;
using PhoneAssistant.WPF.Shared;

namespace PhoneAssistant.Tests.Features.Sims;

internal sealed class SimsMainViewModelTests
{
    private readonly AutoMocker _mocker = new();

    private void MockValidator()
    {     
        var phonesRepository = _mocker.GetMock<IPhonesRepository>();
        var validator = new SimValidator(phonesRepository.Object);
        _mocker.Use<IValidator<SimsMainViewModel>>(validator);
        var serviceProviderMock = _mocker.GetMock<IServiceProvider>();
        serviceProviderMock
            .Setup(sp => sp.GetService(typeof(IValidator<SimsMainViewModel>)))
            .Returns(validator);
    }

    internal SimsMainViewModelTests()
    {
        MockValidator();
    }

    [Test]
    internal async Task ClearCommand_should_set_all_properties_to_null() 
    {
        var vm = _mocker.CreateInstance<SimsMainViewModel>();
        vm.Esim = true;
        vm.NewUser = "Alice";
        vm.PhoneNumber = "01234567890";
        vm.SimNumber = "475334494637";
        vm.Ticket = "654321";

        vm.ClearCommand.Execute(null);

        await Assert.That(vm.Esim).IsFalse();
        await Assert.That(vm.NewUser).IsNull();
        await Assert.That(vm.PhoneNumber).IsNull();
        await Assert.That(vm.SimNumber).IsNull();
        await Assert.That(vm.Ticket).IsNull();
    }

    [Test]
    [Arguments(123456,"Service Request")]
    [Arguments(7654321, "Issue")]
    internal async Task EmailHtml_should_be_generated_when_HasError_is_false(int ticket, string heading)
    {
        var vm = _mocker.CreateInstance<SimsMainViewModel>();
        vm.Esim = true;
        vm.NewUser = "Alice";
        vm.PhoneNumber = "01234567890";
        vm.SimNumber = "475334494637";
        vm.Ticket = "654321";

        await Assert.That(vm.HasErrors).IsFalse();
        await Assert.That(vm.EmailHtml).Contains($"{heading}:\t# {ticket}");
    }

    [Test]
    internal async Task HasErrors_should_be_false_when_required_fields_supplied()
    {
        var vm = _mocker.CreateInstance<SimsMainViewModel>();
        vm.NewUser = "Rosie Lane";
        vm.PhoneNumber = "07814209742";
        vm.SimNumber = "8944122605563572205";
        vm.Ticket = "262281";

        await Assert.That(vm.HasErrors).IsFalse();
    }

    [Test]
    internal async Task HasErrors_should_be_true_when_required_fields_missing()
    {
        var vm = _mocker.CreateInstance<SimsMainViewModel>();

        await Assert.That(vm.HasErrors).IsTrue();

        IEnumerable<string> errors = vm.GetErrors(nameof(vm.NewUser)).Cast<string>();
        await Assert.That(errors.First()).IsEqualTo("New User required");
        errors = vm.GetErrors(nameof(vm.PhoneNumber)).Cast<string>();
        await Assert.That(errors.First()).IsEqualTo("Phone Number required");
        errors = vm.GetErrors(nameof(vm.SimNumber)).Cast<string>();
        await Assert.That(errors.First()).IsEqualTo("SIM Number required");
        errors = vm.GetErrors(nameof(vm.Ticket)).Cast<string>();
        await Assert.That(errors.First()).IsEqualTo("Ticket required");
    }

    [Test]
    internal async Task PhoneNumber_changed_should_set_SimNumber_when_SIM_exists()
    {
        Mock<ISimRepository> baseRepository = _mocker.GetMock<ISimRepository>();
        baseRepository.Setup(r => r.GetSimNumber("01234567890")).ReturnsAsync("sim number");
        var vm = _mocker.CreateInstance<SimsMainViewModel>();

        vm.NewUser = "Alice";
        vm.PhoneNumber = "01234567890";
        vm.Ticket = "654321";
        
        _mocker.VerifyAll();
        await Assert.That(vm.SimNumber).IsEqualTo("sim number");
    }

    [Test]
    internal async Task PrintEnvelopeCommand_should_be_disabled_when_Errors()
    {
        var vm = _mocker.CreateInstance<SimsMainViewModel>();

        await Assert.That(vm.HasErrors).IsTrue();
        await Assert.That(vm.PrintEnvelopeCommand.CanExecute(null)).IsFalse();
    }

    [Test]
    internal async Task PrintEnvelopeCommand_should_be_enabled_when_all_properties_supplied()
    {
        var vm = _mocker.CreateInstance<SimsMainViewModel>();

        vm.NewUser = "Alice";
        vm.PhoneNumber = "01234567890";
        vm.SimNumber = "475334494637";
        vm.Ticket = "654321";

        await Assert.That(vm.HasErrors).IsFalse();
        await Assert.That(vm.PrintEnvelopeCommand.CanExecute(null)).IsTrue();
    }

    [Test]
    internal async Task PrintEnvelopeCommand_should_call_PrintEnvelope_Execute_with_OrderDetails()
    {
        var vm = _mocker.CreateInstance<SimsMainViewModel>();
        OrderDetails? actual = null;
        var printEnvelope = _mocker.GetMock<IPrintEnvelope>();
        printEnvelope.Setup(p => p.Execute(It.IsAny<OrderDetails>()))
                     .Callback<OrderDetails>(o => actual = o);
        vm.Esim = true;
        vm.NewUser = "Rosie Lane";
        vm.PhoneNumber = "07814209742";
        vm.SimNumber = "8944122605563572205";
        vm.Ticket = "262281";

        await vm.PrintEnvelopeCommand.ExecuteAsync(null);

        printEnvelope.Verify(p => p.Execute(It.IsAny<OrderDetails>()), Times.Once);
        await Assert.That(actual).IsNotNull();
        await Assert.That(actual.Phone.Esim).IsTrue();
        await Assert.That(actual.Phone.NewUser).IsEqualTo("Rosie Lane");
        await Assert.That(actual.Phone.PhoneNumber).IsEqualTo("07814209742");
        await Assert.That(actual.Phone.SimNumber).IsEqualTo("8944122605563572205");
        await Assert.That(actual.Phone.Ticket).IsEqualTo(262281);
    }
}
