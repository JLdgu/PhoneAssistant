using PhoneAssistant.WPF.Features.Application;
using PhoneAssistant.WPF.Features.Settings;

namespace PhoneAssistant.Tests.Settings;

[TestClass]
public class SettingsRepositoryTests : DbTestBase
{

    [TestMethod]
    public async Task GetAsync_ReturnsMinimumVersion()
    {
        // Arrange
        using PhoneAssistantDbContext dbContext = new PhoneAssistantDbContext(Options);

        var repository = new SettingsRepository(dbContext);

        // Act
        string actual = await repository.GetAsync();

        // Assert
        Assert.AreEqual("0.0.0.1", actual);
    }
}
