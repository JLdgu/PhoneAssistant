using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

using PhoneAssistant.WPF.Application;
using PhoneAssistant.WPF.Application.Entities;

using Xunit;

namespace PhoneAssistant.Tests.Application;

public class ScriptDBContext
{
    [Fact]    
    [Trait("DBContext","Script")]
    public void GenerateSQLScript()
    {
        DbTestHelper helper = new();
        using SqliteConnection connection = helper.CreateConnection();
        using PhoneAssistantDbContext dbContext = new(helper.Options!);

        dbContext.Database.EnsureCreated();

        string sql = dbContext.Database.GenerateCreateScript();

        File.WriteAllText("c:/temp/DbCreate.sql", sql);
    }
}