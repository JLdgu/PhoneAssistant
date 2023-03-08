using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

using PhoneAssistant.WPF.Features.Application;
using PhoneAssistant.WPF.Features.Settings;

namespace PhoneAssistant.Tests.Settings;

[TestClass]
public class SettingsRepositoryTests : DbTestHelper
{

    [TestMethod]
    public async Task GetAsync_ReturnsMinimumVersion()
    {
        DbTestHelper helper = new();
        using SqliteConnection Connection = helper.CreateConnection();
        using PhoneAssistantDbContext dbContext = new PhoneAssistantDbContext(helper.Options!);
        dbContext.Database.EnsureCreated();

        var repository = new SettingsRepository(dbContext);

        string actual = await repository.GetAsync();

        Assert.AreEqual("0.0.0.1", actual);        
    }    
}
