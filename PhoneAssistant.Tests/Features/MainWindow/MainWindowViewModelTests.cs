using PhoneAssistant.WPF.Features.Settings;
using PhoneAssistant.WPF.Features.MainWindow;
using Moq;
using PhoneAssistant.WPF.Features.Phones;
using PhoneAssistant.WPF.Features.ServiceRequests;
using PhoneAssistant.WPF.Features.Sims;
using PhoneAssistant.WPF.Features.Dashboard;
using PhoneAssistant.WPF.Features.Users;
using Xunit;

namespace PhoneAssistant.Tests.Features.MainWindow;

public sealed class MainWindowViewModelTests
{
    [Fact]
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

        var actual = await Xunit.Assert.ThrowsAsync<ArgumentNullException>(() => command.ExecuteAsync(null));        

        Xunit.Assert.Equal("selectedViewModelType", actual.ParamName);
    }

    [Fact]
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

        var actual = await Xunit.Assert.ThrowsAsync<ArgumentException>(() => command.ExecuteAsync("Wrong Type"));

        Xunit.Assert.Equal("Type System.String is not handled.", actual.Message);
    }

    [Theory]
    //[InlineData(ViewModelType.Dashboard)]
    [InlineData(ViewModelType.None)]
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

        var actual = await Xunit.Assert.ThrowsAsync<NotImplementedException>(() => command.ExecuteAsync(viewModelType));

        Xunit.Assert.Equal("The method or operation is not implemented.", actual.Message);
    }

    [Theory]
    [InlineData(ViewModelType.Dashboard)]
    [InlineData(ViewModelType.Phones)]
    //[InlineData(ViewModelType.Sims)]
    //[InlineData(ViewModelType.ServiceRequests)]
    [InlineData(ViewModelType.Settings)]
    [InlineData(ViewModelType.Users)]
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
                Xunit.Assert.Fail("Unexpected Type");
                break;
        }
    }
}
