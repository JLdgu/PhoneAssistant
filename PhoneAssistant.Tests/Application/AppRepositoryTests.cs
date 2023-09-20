using Moq;

using PhoneAssistant.WPF;
using PhoneAssistant.WPF.Application;
using PhoneAssistant.WPF.Features.Settings;

namespace PhoneAssistant.Tests.Application;

public sealed class AppRepositoryTests
{
    //[TestMethod]
    //public async Task InvalidVersion_False_WhenAssemblyVersionEqualsDBVersion()
    //{
    //    // Arrange        
    //    string ver = typeof(App).Assembly.GetName().Version!.ToString();
    //    ISettingsRepository settings = Mock.Of<ISettingsRepository>(
    //        settings => settings.GetAsync() == Task.FromResult(ver));

    //    var app = new AppRepository(settings);

    //    //Act
    //    bool actual = await app.InvalidVersionAsync();

    //    // Assert
    //    Assert.IsFalse(actual);
    //    Mock.Get(settings).Verify(settings => settings.GetAsync(), Times.Once);
    //}

    //[TestMethod]
    //public async Task InvalidVersion_False_WhenAssemblyVersionGreaterThanDBVersion()
    //{
    //    // Arrange        
    //    ISettingsRepository settings = Mock.Of<ISettingsRepository>(
    //        settings => settings.GetAsync() == Task.FromResult("0.0.0.1"));

    //    var app = new AppRepository(settings);

    //    //Act
    //    bool actual = await app.InvalidVersionAsync();

    //    // Assert
    //    Assert.IsFalse(actual);
    //    Mock.Get(settings).Verify(settings => settings.GetAsync(), Times.Once);
    //}

    //[TestMethod]
    //public async Task InvalidVersion_True_WhenAssemblyVersionLessThanDBVersion()
    //{
    //    // Arrange        
    //    ISettingsRepository settings = Mock.Of<ISettingsRepository>(
    //        settings => settings.GetAsync() == Task.FromResult("99.99.99.99"));

    //    var app = new AppRepository(settings);

    //    //Act
    //    bool actual = await app.InvalidVersionAsync();

    //    // Assert
    //    Assert.IsTrue(actual);
    //    Mock.Get(settings).Verify(settings => settings.GetAsync(), Times.Once);
    //}
}