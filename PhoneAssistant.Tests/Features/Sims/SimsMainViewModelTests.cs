using FluentValidation;

using Moq;
using Moq.AutoMock;

using PhoneAssistant.Model;
using PhoneAssistant.WPF.Features.Sims;

namespace PhoneAssistant.Tests.Features.Sims;

public sealed class SimsMainViewModelTests
{
    [Test]
    public async Task OnPhoneNumberChanged_ShouldSetSimNumber_WhenSimExistsAsync()
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
    public async Task PrintEnvelope_should_be_disabled_when_Errors()
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
    public async Task PrintEnvelope_should_be_enabled_when_all_properties_supplied()
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

        var errs = vm.GetErrors("");
        await Assert.That(vm.HasErrors).IsFalse();
        await Assert.That(vm.PrintEnvelopeCommand.CanExecute(null)).IsTrue();
    }
}
