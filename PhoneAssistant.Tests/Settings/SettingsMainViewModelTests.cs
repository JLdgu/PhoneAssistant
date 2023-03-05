﻿using Moq;

using PhoneAssistant.WPF.Application;
using PhoneAssistant.WPF.Features.Settings;

namespace PhoneAssistant.Tests.Settings;

[TestClass]
public sealed class SettingsMainViewModelTests
{
    [TestMethod]
    public async Task LoadAsync_ReturnsVersionDescription()
    {
        const string versionDescription = "Version Description";
        // Arrange
        IAppRepository app = Mock.Of<IAppRepository>(a =>
            a.VersionDescription == versionDescription);

        var viewModel = new SettingsMainViewModel(app);

        //Act
        await viewModel.LoadAsync();
        string actual = viewModel.VersionDescription;

        // Assert
        Assert.AreEqual(versionDescription, actual);
        Mock.Get(app).Verify(app => app.VersionDescription, Times.Once);
    }

}
