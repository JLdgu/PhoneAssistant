using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Moq;
using PhoneAssistant.WPF.Application;
using PhoneAssistant.WPF.Features.Settings;

namespace PhoneAssistant.WPF.Features.Phones;

[TestClass]
public sealed class PhonesMainViewModelTests
{
    [TestMethod]
    public void OnSelectedPhoneChanging_SaveOutstandingChanges()
    {
        //IAppRepository app = Mock.Of<IAppRepository>(a => a.VersionDescription == versionDescription);
        //var viewModel = new SettingsMainViewModel(app);

        //await viewModel.LoadAsync();

        //string actual = viewModel.VersionDescription;
        //Assert.AreEqual(versionDescription, actual);
        //Mock.Get(app).Verify(app => app.VersionDescription, Times.Once);

    }

}
