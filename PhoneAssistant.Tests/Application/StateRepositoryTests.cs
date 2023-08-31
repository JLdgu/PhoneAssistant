using Microsoft.Data.Sqlite;

using PhoneAssistant.WPF.Application;
using PhoneAssistant.WPF.Application.Entities;

namespace PhoneAssistant.Tests.Application;

[TestClass]
public sealed class StateRepositoryTests
{
    [TestMethod]
    public async Task GetStatesAsync_ReturnsAllStatesAsync()
    {
        DbTestHelper helper = new();
        using SqliteConnection connection = helper.CreateConnection();
        using PhoneAssistantDbContext dbContext = new(helper.Options!);
        dbContext.Database.EnsureCreated();

        State[] testStates = new State[]{
                new State ( "S1" ),
                new State ( "S2" ),
                new State ( "S3" )
        };
        dbContext.States.AddRange(testStates);
        dbContext.SaveChanges();

        StateRepository repository = new (dbContext);

        IEnumerable<State>? actual = await repository.GetStatesAsync();

        Assert.IsNotNull(actual);
        Assert.AreEqual(3, actual.Count());
        Assert.AreEqual("S1", actual.ElementAt(0).Status);
        Assert.AreEqual("S2", actual.ElementAt(1).Status);
        Assert.AreEqual("S3", actual.ElementAt(2).Status);
    }
}
