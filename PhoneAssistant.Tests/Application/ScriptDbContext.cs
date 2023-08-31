using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

using PhoneAssistant.WPF.Application;
using PhoneAssistant.WPF.Application.Entities;

namespace PhoneAssistant.Tests.Application;

[TestClass]
public class ScriptDBContext
{
    [TestMethod]
    [TestCategory("SQLScript")]
    public void GenerateSQLScript()
    {
        DbTestHelper helper = new();
        using SqliteConnection connection = helper.CreateConnection();
        using PhoneAssistantDbContext dbContext = new(helper.Options!);

        dbContext.Database.EnsureCreated();

        string sql = dbContext.Database.GenerateCreateScript();

        File.WriteAllText("c:/temp/DbCreate.sql", sql);
    }

    [TestMethod]
    [TestCategory("SQLScript")]
    public void GenerateV1SQLScript()
    {
        v1DbTestHelper helper = new();
        using SqliteConnection connection = helper.CreateConnection();
        using v1PhoneAssistantDbContext dbContext = new(helper.Options!);

        dbContext.Database.EnsureCreated();

        string sql = dbContext.Database.GenerateCreateScript();

        File.WriteAllText("c:/temp/DbCreateV1.sql", sql);
    }

    //[TestMethod]
    //public void DbSchemaTest()
    //{
    //    v1DbTestHelper helper = new();
    //    //using SqliteConnection connection = helper.CreateConnection();
    //    using SqliteConnection connection = helper.CreateConnection(@"DataSource=c:\temp\phoneassistant.db;");
    //    using v1PhoneAssistantDbContext dbContext = new(helper.Options!);
    //    dbContext.Database.EnsureCreated();

    //    int phones = dbContext.Phones.Count();
    //    Assert.AreEqual(1, phones);
    //}
}