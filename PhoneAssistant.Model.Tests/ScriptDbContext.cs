using Microsoft.EntityFrameworkCore;

namespace PhoneAssistant.Model.Tests;

public class ScriptDBContext
{
    //dotnet run --project PhoneAssistant.Model.Tests -- --treenode-filter /*/*/*/*[Category=Schema]
    [Test]
    [Explicit]
    [Category("Schema")]
    public void GenerateSQLScript()
    {
        
        DbTestHelper helper = new();

        string sql = helper.DbContext.Database.GenerateCreateScript();

        File.WriteAllText("c:/temp/DbCreate.sql", sql);
    }
}