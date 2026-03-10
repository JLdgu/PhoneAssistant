using System.Collections;
using System.Linq;

using FluentValidation;

using Moq;
using Moq.AutoMock;

using PhoneAssistant.Model;
using PhoneAssistant.WPF.Features.Sims;
using PhoneAssistant.WPF.Shared;

namespace PhoneAssistant.Tests.Features.Sims;

public sealed class SimsMainViewModelTests
{
    [Test]
    public async Task HasErrors_should_be_false_when_required_fields_supplied()
    {
        AutoMocker mocker = new();
        var phonesRepository = mocker.GetMock<IPhonesRepository>();
        var validator = new SimValidator(phonesRepository.Object);
        mocker.Use<IValidator<SimsMainViewModel>>(validator);
        var serviceProviderMock = mocker.GetMock<IServiceProvider>();
        serviceProviderMock
            .Setup(sp => sp.GetService(typeof(IValidator<SimsMainViewModel>)))
            .Returns(validator);
        var vm = mocker.CreateInstance<SimsMainViewModel>();

        vm.NewUser = "Rosie Lane";
        vm.PhoneNumber = "07814209742";
        vm.SimNumber = "8944122605563572205";
        vm.Ticket = "262281";

        await Assert.That(vm.HasErrors).IsFalse();
    }
    [Test]
    public async Task HasErrors_should_be_true_when_required_fields_missing()
    {
        AutoMocker mocker = new();
        var phonesRepository = mocker.GetMock<IPhonesRepository>();
        var validator = new SimValidator(phonesRepository.Object);
        mocker.Use<IValidator<SimsMainViewModel>>(validator);
        var serviceProviderMock = mocker.GetMock<IServiceProvider>();
        serviceProviderMock
            .Setup(sp => sp.GetService(typeof(IValidator<SimsMainViewModel>)))
            .Returns(validator);

        var vm = mocker.CreateInstance<SimsMainViewModel>();

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
    public async Task PhoneNumber_changed_should_set_SimNumber_when_SIM_exists()
    {
        AutoMocker mocker = new();
        Mock<IBaseReportRepository> baseRepository = mocker.GetMock<IBaseReportRepository>();
        baseRepository.Setup(r => r.GetSimNumberAsync("01234567890")).ReturnsAsync("sim number");
        var phonesRepository = mocker.GetMock<IPhonesRepository>();
        var validator = new SimValidator(phonesRepository.Object);
        var serviceProviderMock = mocker.GetMock<IServiceProvider>();
        serviceProviderMock
            .Setup(sp => sp.GetService(typeof(IValidator<SimsMainViewModel>)))
            .Returns(validator);

        var vm = mocker.CreateInstance<SimsMainViewModel>();
        vm.NewUser = "Alice";
        vm.PhoneNumber = "01234567890";
        vm.Ticket = "654321";
        
        mocker.VerifyAll();
        await Assert.That(vm.SimNumber).IsEqualTo("sim number");
    }

    [Test]
    public async Task PrintEnvelopeCommand_should_be_disabled_when_Errors()
    {
        AutoMocker mocker = new();
        var phonesRepository = mocker.GetMock<IPhonesRepository>();
        var validator = new SimValidator(phonesRepository.Object);
        mocker.Use<IValidator<SimsMainViewModel>>(validator);
        var serviceProviderMock = mocker.GetMock<IServiceProvider>();
        serviceProviderMock
            .Setup(sp => sp.GetService(typeof(IValidator<SimsMainViewModel>)))
            .Returns(validator);

        var vm = mocker.CreateInstance<SimsMainViewModel>();

        await Assert.That(vm.HasErrors).IsTrue();
        await Assert.That(vm.PrintEnvelopeCommand.CanExecute(null)).IsFalse();
    }

    [Test]
    public async Task PrintEnvelopeCommand_should_be_enabled_when_all_properties_supplied()
    {
        AutoMocker mocker = new();
        var phonesRepository = mocker.GetMock<IPhonesRepository>();
        var validator = new SimValidator(phonesRepository.Object);
        var serviceProviderMock = mocker.GetMock<IServiceProvider>();
        serviceProviderMock
            .Setup(sp => sp.GetService(typeof(IValidator<SimsMainViewModel>)))
            .Returns(validator);
        var vm = mocker.CreateInstance<SimsMainViewModel>();

        vm.NewUser = "Alice";
        vm.PhoneNumber = "01234567890";
        vm.SimNumber = "475334494637";
        vm.Ticket = "654321";

        await Assert.That(vm.HasErrors).IsFalse();
        await Assert.That(vm.PrintEnvelopeCommand.CanExecute(null)).IsTrue();
    }

    [Test]
    public async Task PrintEnvelopeCommand_should_call_PrintEnvelope_Execute_with_OrderDetails()
    {
        AutoMocker mocker = new();
        OrderDetails? actual = null;
        var vm = mocker.CreateInstance<SimsMainViewModel>();
        vm.NewUser = "Rosie Lane";
        vm.PhoneNumber = "07814209742";
        vm.SimNumber = "8944122605563572205";
        vm.Ticket = "262281";
        var printEnvelope = mocker.GetMock<IPrintEnvelope>();
        printEnvelope
            .Setup(p => p.Execute(It.IsAny<OrderDetails>()))
            .Callback<OrderDetails>(o => actual = o);

        await vm.PrintEnvelopeCommand.ExecuteAsync(null);

        printEnvelope.Verify(p => p.Execute(It.IsAny<OrderDetails>()), Times.Once);
        await Assert.That(actual).IsNotNull();
        await Assert.That(actual.Phone.NewUser).IsEqualTo("Rosie Lane");
        await Assert.That(actual.Phone.PhoneNumber).IsEqualTo("07814209742");
        await Assert.That(actual.Phone.SimNumber).IsEqualTo("8944122605563572205");
        await Assert.That(actual.Phone.Ticket).IsEqualTo(262281);
    }
}
