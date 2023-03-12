using Microsoft.Data.Sqlite;

using PhoneAssistant.WPF.Application;
using PhoneAssistant.WPF.Application.Entities;
using PhoneAssistant.WPF.Features.Application;
using PhoneAssistant.WPF.Features.Phones;

namespace PhoneAssistant.Tests.Application;

[TestClass]
public sealed class StateRepositoryTests
{
    [TestMethod]
    public async Task GetStatesAsync_ReturnsAllStatesAsync()
    {
        DbTestHelper helper = new();
        using SqliteConnection Connection = helper.CreateConnection();
        using PhoneAssistantDbContext dbContext = new PhoneAssistantDbContext(helper.Options!);
        dbContext.Database.EnsureCreated();

        StateEntity[] testStates = new StateEntity[]{
                new StateEntity { Status = "S1" },
                new StateEntity { Status = "S2" },
                new StateEntity { Status = "S3" }
        };
        dbContext.States.AddRange(testStates);
        dbContext.SaveChanges();

        StateRepository repository = new (dbContext);

        IEnumerable<WPF.Models.State>? actual = await repository.GetStatesAsync();

        Assert.IsNotNull(actual);
        Assert.AreEqual(3, actual.Count());
        Assert.AreEqual("S1", actual.ElementAt(0).Status);
        Assert.AreEqual("S2", actual.ElementAt(1).Status);
        Assert.AreEqual("S3", actual.ElementAt(2).Status);
    }
}
