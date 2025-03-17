using Microsoft.EntityFrameworkCore;

namespace PhoneAssistant.Tests.Application;

public class ScriptDBContext
{
    [Test]
    [Skip("Manual run only")]
    public void GenerateSQLScript()
    {
        
        DbTestHelper helper = new();

        string sql = helper.DbContext.Database.GenerateCreateScript();

        File.WriteAllText("c:/temp/DbCreate.sql", sql);
    }
}