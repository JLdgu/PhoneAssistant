using Microsoft.EntityFrameworkCore;

using Xunit;

namespace PhoneAssistant.Tests.Application;

public class ScriptDBContext
{
    [Fact]    
    [Trait("DBContext","Script")]
    public void GenerateSQLScript()
    {
        
        DbTestHelper helper = new();

        string sql = helper.DbContext.Database.GenerateCreateScript();

        File.WriteAllText("c:/temp/DbCreate.sql", sql);
    }
}