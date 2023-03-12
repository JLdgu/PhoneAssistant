using PhoneAssistant.WPF.Features.Settings;
using PhoneAssistant.WPF.Features.MainWindow;
using Moq;
using PhoneAssistant.WPF.Features.Phones;
using PhoneAssistant.WPF.Features.ServiceRequests;
using PhoneAssistant.WPF.Features.Sims;

namespace PhoneAssistant.Tests.Features.MainWindow;

[TestClass]
public sealed class MainWindowViewModelTests
{
    [TestMethod]
    public async Task UpdateViewAsync_NullViewModelType_ThrowsArgumentNullException()
    {
        var phonesVM = Mock.Of<IPhonesMainViewModel>();
        var simsVM = Mock.Of<ISimsMainViewModel>();
        var srsVM = Mock.Of<IServiceRequestsMainViewModel>();
        var settingsVM = Mock.Of<ISettingsMainViewModel>();
        var viewModel = new MainWindowViewModel(phonesVM, simsVM, srsVM, settingsVM);
        var command = viewModel.UpdateViewCommand;

        var actual = await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => command.ExecuteAsync(null));

        Assert.AreEqual("selectedViewModelType", actual.ParamName);
    }


    [TestMethod]
    public async Task UpdateViewAsync_StringViewModelType_ThrowsArgumenException()
    {
        var phonesVM = Mock.Of<IPhonesMainViewModel>();
        var simsVM = Mock.Of<ISimsMainViewModel>();
        var srsVM = Mock.Of<IServiceRequestsMainViewModel>();
        var settingsVM = Mock.Of<ISettingsMainViewModel>();
        var viewModel = new MainWindowViewModel(phonesVM, simsVM, srsVM, settingsVM);
        var command = viewModel.UpdateViewCommand;

        var actual = await Assert.ThrowsExceptionAsync<ArgumentException>(() => command.ExecuteAsync("Wrong Type"));

        Assert.AreEqual("Type System.String is not handled.", actual.Message);
    }

    [TestMethod]
    [DataRow(ViewModelType.Dashboard)]
    [DataRow(ViewModelType.None)]
    public async Task UpdateViewAsync_InvalidViewModelType_ThrowsNotImplementedException(ViewModelType viewModelType)
    {
        var phonesVM = Mock.Of<IPhonesMainViewModel>();
        var simsVM = Mock.Of<ISimsMainViewModel>();
        var srsVM = Mock.Of<IServiceRequestsMainViewModel>();
        var settingsVM = Mock.Of<ISettingsMainViewModel>();
        var viewModel = new MainWindowViewModel(phonesVM, simsVM, srsVM, settingsVM);
        var command = viewModel.UpdateViewCommand;

        var actual = await Assert.ThrowsExceptionAsync<NotImplementedException>(() => command.ExecuteAsync(viewModelType));

        Assert.AreEqual("The method or operation is not implemented.", actual.Message);
    }

    [TestMethod]
    [DataRow(ViewModelType.Phones)]
    [DataRow(ViewModelType.Sims)]
    [DataRow(ViewModelType.ServiceRequests)]
    [DataRow(ViewModelType.Settings)]
    public async Task UpdateViewAsync_WithValidViewModelType_CallUpdateAsync(ViewModelType viewModelType)
    {
        var phonesVM = new Mock<IPhonesMainViewModel>();
        phonesVM.Setup(vm => vm.LoadAsync());
        var simsVM = new Mock<ISimsMainViewModel>();
        simsVM.Setup(vm => vm.LoadAsync());
        var srsVM = new Mock<IServiceRequestsMainViewModel>();
        srsVM.Setup(vm => vm.LoadAsync());
        var settingsVM = new Mock<ISettingsMainViewModel>();
        settingsVM.Setup(vm => vm.LoadAsync());
        var viewModel = new MainWindowViewModel(phonesVM.Object, simsVM.Object, srsVM.Object, settingsVM.Object);
        var command = viewModel.UpdateViewCommand;

        await command.ExecuteAsync(viewModelType);

        switch (viewModelType)
        {
            case ViewModelType.Phones:
                phonesVM.Verify(vm => vm.LoadAsync(), Times.Once);
                break;
            case ViewModelType.Sims:
                simsVM.Verify(vm => vm.LoadAsync(), Times.Once);
                break;
            case ViewModelType.ServiceRequests:
                srsVM.Verify(vm => vm.LoadAsync(), Times.Once);
                break;
            case ViewModelType.Settings:
                settingsVM.Verify(vm => vm.LoadAsync(), Times.Once);
                break;
            default: 
                Assert.Fail("Unexpected Type");
                break;
        }
    }
}
