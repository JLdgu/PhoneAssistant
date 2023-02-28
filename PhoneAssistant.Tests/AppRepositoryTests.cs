using Moq;
using PhoneAssistant.WPF;
using PhoneAssistant.WPF.Application;
using PhoneAssistant.WPF.Application.Entities;

namespace PhoneAssistant.Tests;

[TestClass]
public class AppRepositoryTests
{
    [TestMethod]
    public async Task InvalidVersion_False_WhenAssemblyVersionEqualsDBVersion()
    {
        // Arrange
        var ver = typeof(App).Assembly.GetName().Version;
        var mock = new Mock<ISettingRepository>();
        mock.Setup(s => s.GetAsync().Result).Returns(ver!.ToString());

        var app = new AppRepository(mock.Object);

        //Act
        bool actual = await app.InvalidVersionAsync();

        // Assert
        Assert.IsFalse(actual);
    }

    [TestMethod]
    public async Task InvalidVersion_False_WhenAssemblyVersionGreaterThanDBVersion()
    {
        // Arrange
        var mock = new Mock<ISettingRepository>();
        mock.Setup(s => s.GetAsync().Result).Returns("0.0.0.1");

        var app = new AppRepository(mock.Object);

        //Act
        bool actual = await app.InvalidVersionAsync();

        // Assert
        Assert.IsFalse(actual);
    }

    [TestMethod]
    public async Task InvalidVersion_True_WhenAssemblyVersionLessThanDBVersion()
    {
        // Arrange
        var mock = new Mock<ISettingRepository>();
        mock.Setup(s => s.GetAsync().Result).Returns("99.99.99.99");

        var app = new AppRepository(mock.Object);

        //Act
        bool actual = await app.InvalidVersionAsync();

        // Assert
        Assert.IsTrue(actual);
    }

}