﻿using PhoneAssistant.WPF.Features.Settings;
using PhoneAssistant.WPF.Features.MainWindow;
using Moq;
using PhoneAssistant.WPF.Features.Phones;
using PhoneAssistant.WPF.Features.Sims;
using PhoneAssistant.WPF.Features.Dashboard;
using PhoneAssistant.WPF.Features.Users;
using Moq.AutoMock;

namespace PhoneAssistant.Tests.Features.MainWindow;

public sealed class MainWindowViewModelTests
{
    [Test]
    public async Task UpdateViewAsync_NullViewModelType_ThrowsArgumentNullException()
    {
        AutoMocker mocker = new();
        MainWindowViewModel vm = mocker.CreateInstance<MainWindowViewModel>();

        await Assert.ThrowsAsync<ArgumentException>(() => vm.UpdateViewCommand.ExecuteAsync(null));               
    }

    [Test]
    public async Task UpdateViewAsync_StringViewModelType_ThrowsArgumenException()
    {
        AutoMocker mocker = new AutoMocker();
        MainWindowViewModel vm = mocker.CreateInstance<MainWindowViewModel>();

        await Assert.ThrowsAsync<ArgumentException>(() => vm.UpdateViewCommand.ExecuteAsync("Wrong Type"));
    }

    [Test]
    [Arguments(ViewModelType.None)]
    [Arguments(ViewModelType.Dashboard)]  
    [Arguments(ViewModelType.Phones)]
    [Arguments(ViewModelType.Sims)]
    [Arguments(ViewModelType.Settings)]
    [Arguments(ViewModelType.Users)]
    public async Task UpdateViewAsync_WithValidViewModelType_CallUpdateAsync(ViewModelType viewModelType)
    {
        AutoMocker mocker = new AutoMocker();
        Mock<IDashboardMainViewModel> dashboard = mocker.GetMock<IDashboardMainViewModel>();
        Mock<IPhonesMainViewModel> phones = mocker.GetMock<IPhonesMainViewModel>();
        Mock<ISimsMainViewModel> sims =  mocker.GetMock<ISimsMainViewModel>();
        Mock<ISettingsMainViewModel> settings = mocker.GetMock<ISettingsMainViewModel>();
        Mock<IUsersMainViewModel> users = mocker.GetMock<IUsersMainViewModel>();
        MainWindowViewModel vm = mocker.CreateInstance<MainWindowViewModel>();

        await vm.UpdateViewCommand.ExecuteAsync(viewModelType);

        switch (viewModelType)
        {
            case ViewModelType.None:
                dashboard.Verify(vm => vm.LoadAsync(), Times.Exactly(2));
                break;
            case ViewModelType.Dashboard:
                dashboard.Verify(vm => vm.LoadAsync(), Times.Exactly(2));
                break;
            case ViewModelType.Phones:
                phones.Verify(vm => vm.LoadAsync(), Times.Once);
                break;
            case ViewModelType.Sims:
                sims.Verify(vm => vm.LoadAsync(), Times.Once);
                break;
            //case ViewModelType.ServiceRequests:
            //    Mock.Get(srs).Verify(vm => vm.LoadAsync(), Times.Once);
            //    break;
            case ViewModelType.Settings:
                settings.Verify(vm => vm.LoadAsync(), Times.Once);
                break;
            case ViewModelType.Users:
                users.Verify(vm => vm.LoadAsync(), Times.Once);
                break;
            default: 
                Assert.Fail("Unexpected Type");
                break;
        }
    }
}
