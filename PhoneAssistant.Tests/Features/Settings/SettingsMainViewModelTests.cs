using Moq;

using PhoneAssistant.WPF.Application;
using PhoneAssistant.WPF.Features.Settings;

namespace PhoneAssistant.Tests.Features.Settings;

[TestClass]
public sealed class SettingsMainViewModelTests
{
    [TestMethod]
    public async Task LoadAsync_ReturnsVersionDescription()
    {
            
        IAppRepository app = Mock.Of<IAppRepository>(a => a.VersionDescription == versionDescription);
        var viewModel = new SettingsMainViewModel(app);

        await viewModel.LoadAsync();

        string actual = viewModel.VersionDescription;
        Assert.AreEqual(versionDescription, actual);
        Mock.Get(app).Verify(app => app.VersionDescription, Times.Once);
    }
}
