using PhoneAssistant.WPF.Features.Settings;
using PhoneAssistant.WPF.Features.MainWindow;
using Moq;
using PhoneAssistant.WPF.Features.Phones;
using PhoneAssistant.WPF.Features.ServiceRequests;
using PhoneAssistant.WPF.Features.Sims;
using PhoneAssistant.WPF.Features.Dashboard;
using PhoneAssistant.WPF.Features.Users;

namespace PhoneAssistant.Tests.Features.MainWindow;

[TestClass]
public sealed class MainWindowViewModelTests
{
    [TestMethod]
    public async Task UpdateViewAsync_NullViewModelType_ThrowsArgumentNullException()
    {
        var dashboardVM = Mock.Of<IDashboardMainViewModel>();
        var phonesVM = Mock.Of<IPhonesMainViewModel>();
        //var simsVM = Mock.Of<ISimsMainViewModel>();
        //var srsVM = Mock.Of<IServiceRequestsMainViewModel>();
        var settingsVM = Mock.Of<ISettingsMainViewModel>();
        var usersVM = Mock.Of<IUsersMainViewModel>();
        var viewModel = new MainWindowViewModel(dashboardVM,phonesVM, settingsVM,usersVM);
        var command = viewModel.UpdateViewCommand;

        var actual = await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => command.ExecuteAsync(null));

        Assert.AreEqual("selectedViewModelType", actual.ParamName);
    }

    [TestMethod]
    public async Task UpdateViewAsync_StringViewModelType_ThrowsArgumenException()
    {
        var dashboardVM = Mock.Of<IDashboardMainViewModel>();
        var phonesVM = Mock.Of<IPhonesMainViewModel>();
        //var simsVM = Mock.Of<ISimsMainViewModel>();
        //var srsVM = Mock.Of<IServiceRequestsMainViewModel>();
        var settingsVM = Mock.Of<ISettingsMainViewModel>();
        var usersVM = Mock.Of<IUsersMainViewModel>();
        var viewModel = new MainWindowViewModel(dashboardVM, phonesVM, settingsVM, usersVM);
        var command = viewModel.UpdateViewCommand;

        var actual = await Assert.ThrowsExceptionAsync<ArgumentException>(() => command.ExecuteAsync("Wrong Type"));

        Assert.AreEqual("Type System.String is not handled.", actual.Message);
    }

    [TestMethod]
    //[DataRow(ViewModelType.Dashboard)]
    [DataRow(ViewModelType.None)]
    public async Task UpdateViewAsync_InvalidViewModelType_ThrowsNotImplementedException(ViewModelType viewModelType)
    {
        var dashboardVM = Mock.Of<IDashboardMainViewModel>();
        var phonesVM = Mock.Of<IPhonesMainViewModel>();
        //var simsVM = Mock.Of<ISimsMainViewModel>();
        //var srsVM = Mock.Of<IServiceRequestsMainViewModel>();
        var settingsVM = Mock.Of<ISettingsMainViewModel>();
        var usersVM = Mock.Of<IUsersMainViewModel>();
        var viewModel = new MainWindowViewModel(dashboardVM, phonesVM, settingsVM, usersVM);

        var command = viewModel.UpdateViewCommand;

        var actual = await Assert.ThrowsExceptionAsync<NotImplementedException>(() => command.ExecuteAsync(viewModelType));

        Assert.AreEqual("The method or operation is not implemented.", actual.Message);
    }

    [TestMethod]
    [DataRow(ViewModelType.Dashboard)]
    [DataRow(ViewModelType.Phones)]
    //[DataRow(ViewModelType.Sims)]
    //[DataRow(ViewModelType.ServiceRequests)]
    [DataRow(ViewModelType.Settings)]
    [DataRow(ViewModelType.Users)]
    public async Task UpdateViewAsync_WithValidViewModelType_CallUpdateAsync(ViewModelType viewModelType)
    {
        IDashboardMainViewModel dashboard = Mock.Of<IDashboardMainViewModel>();
        IPhonesMainViewModel phones = Mock.Of<IPhonesMainViewModel>();
        ISimsMainViewModel sims =  Mock.Of<ISimsMainViewModel>();
        IServiceRequestsMainViewModel srs = Mock.Of<IServiceRequestsMainViewModel>();
        ISettingsMainViewModel settings = Mock.Of<ISettingsMainViewModel>();
        IUsersMainViewModel users = Mock.Of<IUsersMainViewModel>();

        var viewModel = new MainWindowViewModel(dashboard, phones, settings, users);
        var command = viewModel.UpdateViewCommand;

        await command.ExecuteAsync(viewModelType);
                
        switch (viewModelType)
        {
            case ViewModelType.Dashboard:
                Mock.Get(dashboard).Verify(vm => vm.LoadAsync(), Times.Once);
                break;
            case ViewModelType.Phones:
                Mock.Get(phones).Verify(vm => vm.LoadAsync(), Times.Once);
                break;
            //case ViewModelType.Sims:
            //    Mock.Get(sims).Verify(vm => vm.LoadAsync(), Times.Once);
            //    break;
            //case ViewModelType.ServiceRequests:
            //    Mock.Get(srs).Verify(vm => vm.LoadAsync(), Times.Once);
            //    break;
            case ViewModelType.Settings:
                Mock.Get(settings).Verify(vm => vm.LoadAsync(), Times.Once);
                break;
            case ViewModelType.Users:
                Mock.Get(users).Verify(vm => vm.LoadAsync(), Times.Once);
                break;
            default: 
                Assert.Fail("Unexpected Type");
                break;
        }
    }
}
