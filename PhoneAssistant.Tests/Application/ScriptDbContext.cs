using Microsoft.EntityFrameworkCore;

using Xunit;

namespace PhoneAssistant.Tests.Application;

public class ScriptDBContext
{
    [Fact(Skip ="Manual run only")]
    //[Fact]
    public void GenerateSQLScript()
    {
        
        DbTestHelper helper = new();

        string sql = helper.DbContext.Database.GenerateCreateScript();

        File.WriteAllText("c:/temp/DbCreate.sql", sql);
    }
}