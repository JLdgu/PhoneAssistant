using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

using PhoneAssistant.WPF.Features.Application;

namespace PhoneAssistant.Tests;

public interface IDbTestBase
{
    public void Initialize();
    public void Cleanup();
}

public class DbTestBase : IDbTestBase
{
    internal SqliteConnection? Connection;
    internal DbContextOptions<PhoneAssistantDbContext> Options = new DbContextOptions<PhoneAssistantDbContext>();    

    [TestInitialize]
    public void Initialize()
    {
        Connection = new SqliteConnection("DataSource=:memory:");
        Connection.Open();
        Options = new DbContextOptionsBuilder<PhoneAssistantDbContext>().UseSqlite(Connection!).Options;

        using PhoneAssistantDbContext context = new PhoneAssistantDbContext(Options);
        {
            context.Database.EnsureCreated();
        }

    }

    public void Cleanup()
    {
        if (Connection is not null)
        {
            Connection.Close();
            Connection.Dispose();
        }
    }
}