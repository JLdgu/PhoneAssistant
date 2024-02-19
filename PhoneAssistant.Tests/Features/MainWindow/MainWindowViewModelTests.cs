using PhoneAssistant.WPF.Features.Settings;
using PhoneAssistant.WPF.Features.MainWindow;
using Moq;
using PhoneAssistant.WPF.Features.Phones;
using PhoneAssistant.WPF.Features.ServiceRequests;
using PhoneAssistant.WPF.Features.Sims;
using PhoneAssistant.WPF.Features.Dashboard;
using PhoneAssistant.WPF.Features.Users;
using Xunit;
using Moq.AutoMock;

namespace PhoneAssistant.Tests.Features.MainWindow;

public sealed class MainWindowViewModelTests
{
    [Fact]
    public async Task UpdateViewAsync_NullViewModelType_ThrowsArgumentNullException()
    {
        AutoMocker mocker = new AutoMocker();
        MainWindowViewModel vm = mocker.CreateInstance<MainWindowViewModel>();

        var actual = await Assert.ThrowsAsync<ArgumentNullException>(() => vm.UpdateViewCommand.ExecuteAsync(null));        

        Assert.Equal("selectedViewModelType", actual.ParamName);
    }

    [Fact]
    public async Task UpdateViewAsync_StringViewModelType_ThrowsArgumenException()
    {
        AutoMocker mocker = new AutoMocker();
        MainWindowViewModel vm = mocker.CreateInstance<MainWindowViewModel>();

        var actual = await Assert.ThrowsAsync<ArgumentException>(() => vm.UpdateViewCommand.ExecuteAsync("Wrong Type"));

        Assert.Equal("Type System.String is not handled.", actual.Message);
    }

    [Theory]
    [InlineData(ViewModelType.None)]
    [InlineData(ViewModelType.Dashboard)]  
    [InlineData(ViewModelType.Phones)]
    [InlineData(ViewModelType.Sims)]
    //[InlineData(ViewModelType.ServiceRequests)]
    [InlineData(ViewModelType.Settings)]
    [InlineData(ViewModelType.Users)]
    public async Task UpdateViewAsync_WithValidViewModelType_CallUpdateAsync(ViewModelType viewModelType)
    {
        AutoMocker mocker = new AutoMocker();
        Mock<IDashboardMainViewModel> dashboard = mocker.GetMock<IDashboardMainViewModel>();
        Mock<IPhonesMainViewModel> phones = mocker.GetMock<IPhonesMainViewModel>();
        Mock<ISimsMainViewModel> sims =  mocker.GetMock<ISimsMainViewModel>();
        Mock<IServiceRequestsMainViewModel> srs = mocker.GetMock<IServiceRequestsMainViewModel>();
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
