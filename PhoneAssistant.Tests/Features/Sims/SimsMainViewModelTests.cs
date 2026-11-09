using FluentValidation;
using Moq;
using Moq.AutoMock;
using PhoneAssistant.Model;
using PhoneAssistant.WPF.Features.Sims;
using PhoneAssistant.WPF.Shared;

using System.ComponentModel;

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
    public async Task SimType_should_default_to_Data_and_accept_other_values()
    {
        MockValidator();
        var vm = _mocker.CreateInstance<SimsMainViewModel>();

        await Assert.That(vm.SimType).IsEqualTo(SimCardType.Data);

        vm.SimType = SimCardType.VoiceText;

        await Assert.That(vm.SimType).IsEqualTo(SimCardType.VoiceText);
    }

    [Test]
    internal async Task ClearCommand_should_set_all_properties_to_default() 
    {
        var vm = _mocker.CreateInstance<SimsMainViewModel>();
        vm.Esim = true;
        vm.NewUser = "Alice";
        vm.PhoneNumber = "01234567890";
        vm.SimNumber = "475334494637";
        vm.Ticket = "654321";
        vm.DeliveryAddress = "123 Main St";

        vm.ClearCommand.Execute(null);

        await Assert.That(vm.HasErrors).IsTrue();

        await Assert.That(vm.DeliveryAddress).IsEqualTo(string.Empty);

        await Assert.That(vm.Esim).IsFalse();
        await Assert.That(vm.NewUser).IsNull();
        await Assert.That(vm.PhoneNumber).IsNull();
        await Assert.That(vm.SimNumber).IsNull();
        await Assert.That(vm.Ticket).IsNull();
    }

    [Test]
    internal async Task ClearCommand_should_disable_buttons()
    {
        var vm = _mocker.CreateInstance<SimsMainViewModel>();
        vm.Esim = true;
        vm.NewUser = "Alice";
        vm.PhoneNumber = "01234567890";
        vm.SimNumber = "475334494637";
        vm.Ticket = "654321";

        vm.ClearCommand.Execute(null);
        

        await Assert.That(vm.PrintDymoLabelCommand.CanExecute(null)).IsFalse();
        await Assert.That(vm.PrintEnvelopeCommand.CanExecute(null)).IsFalse();
    }

    [Test]
    internal async Task EmailHtml_should_include_delivery_address()
    {
        var vm = _mocker.CreateInstance<SimsMainViewModel>();
        vm.Esim = true;
        vm.NewUser = "Alice";
        vm.PhoneNumber = "01234567890";
        vm.SimNumber = "475334494637";
        vm.Ticket = "1234567";

        vm.DeliveryAddress = "123 High Street";

        await Assert.That(vm.EmailHtml).Contains("123 High Street");
    }

    [Test]
    internal async Task EmailHtml_should_include_ticket_when_required_fields_are_valid()
    {
        var vm = _mocker.CreateInstance<SimsMainViewModel>();

        vm.Esim = true;
        vm.NewUser = "Alice";
        vm.PhoneNumber = "01234567890";
        vm.SimNumber = "475334494637";
        vm.Ticket = "123456";
        vm.DeliveryAddress = "123 High Street";

        await Assert.That(vm.HasErrors).IsFalse();
        await Assert.That(vm.EmailHtml).Contains("123456");
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

        await Assert.That(vm.GetErrors(nameof(vm.NewUser)).Cast<string>()).Contains("New User required");
        await Assert.That(vm.GetErrors(nameof(vm.PhoneNumber)).Cast<string>()).Contains("Phone Number required");
        await Assert.That(vm.GetErrors(nameof(vm.SimNumber)).Cast<string>()).Contains("SIM Number required");
        await Assert.That(vm.GetErrors(nameof(vm.Ticket)).Cast<string>()).Contains("Ticket required");
    }

    [Test]
    internal async Task HasErrors_should_notify_when_validation_errors_are_added_or_cleared()
    {
        var vm = _mocker.CreateInstance<SimsMainViewModel>();

        await AssertHasErrorsChangedAsync(vm, false, () =>
        {
            vm.NewUser = "Alice";
            vm.PhoneNumber = "01234567890";
            vm.SimNumber = "475334494637";
            vm.Ticket = "654321";
        });

        await AssertHasErrorsChangedAsync(vm, true, () => vm.Ticket = null);
    }

    private static async Task AssertHasErrorsChangedAsync(SimsMainViewModel viewModel, bool expected, Action update)
    {
        TaskCompletionSource notification = new(TaskCreationOptions.RunContinuationsAsynchronously);
        PropertyChangedEventHandler handler = (_, args) =>
        {
            if (args.PropertyName == nameof(SimsMainViewModel.HasErrors) && viewModel.HasErrors == expected)
                notification.TrySetResult();
        };

        viewModel.PropertyChanged += handler;
        try
        {
            update();
            if (viewModel.HasErrors == expected)
                notification.TrySetResult();
            await notification.Task.WaitAsync(TimeSpan.FromSeconds(5));
        }
        finally
        {
            viewModel.PropertyChanged -= handler;
        }
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
    internal async Task PrintEnvelopeCommand_should_call_PrintEnvelope_Execute()
    {
        var vm = _mocker.CreateInstance<SimsMainViewModel>();
        var printEnvelope = _mocker.GetMock<IPrintEnvelope>();
        printEnvelope.Setup(p => p.Execute(It.IsAny<string>(), It.IsAny<string>()));
        vm.Esim = true;
        vm.NewUser = "Rosie Lane";
        vm.PhoneNumber = "07814209742";
        vm.SimNumber = "8944122605563572205";
        vm.Ticket = "262281";

        await vm.PrintEnvelopeCommand.ExecuteAsync(null);


        printEnvelope.Verify(p => p.Execute(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }
}
