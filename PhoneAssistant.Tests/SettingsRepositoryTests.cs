using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

using PhoneAssistant.WPF.Features.Application;
using PhoneAssistant.WPF.Features.Settings;

namespace PhoneAssistant.Tests;

[TestClass]
public class SettingsRepositoryTests
{

    [TestMethod]
    public async Task GetAsync_ReturnsMinimumVersion()
    {
        SqliteConnection? Connection;
        DbContextOptions<PhoneAssistantDbContext> Options = new DbContextOptions<PhoneAssistantDbContext>();

        Connection = new SqliteConnection("DataSource=:memory:");
        Connection.Open();
        Options = new DbContextOptionsBuilder<PhoneAssistantDbContext>().UseSqlite(Connection!).Options;

        using PhoneAssistantDbContext context = new PhoneAssistantDbContext(Options);
        {
            context.Database.EnsureCreated();
        }

        var repository = new SettingsRepository(context);
        string actual = await repository.GetAsync();

        Assert.AreEqual("0.0.0.1", actual);

        if (Connection is not null)
        {
            Connection.Close();
            Connection.Dispose();
        }
    }
}
