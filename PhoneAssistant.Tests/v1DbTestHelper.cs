using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

using PhoneAssistant.WPF.Application;

namespace PhoneAssistant.Tests;

public class v1DbTestHelper : IDisposable
{
    private readonly SqliteConnection _connection;
    public v1PhoneAssistantDbContext DbContext { get; }


    public DbContextOptions<v1PhoneAssistantDbContext>? Options;
    private bool _disposedValue;
    public v1DbTestHelper(string datasource = "DataSource=:memory:;")
    {
        _connection = CreateConnection();
        DbContext = new(Options!);
        DbContext.Database.EnsureCreated();
    }

    internal SqliteConnection CreateConnection(string datasource = "DataSource=:memory:;")
    {
        var connection = new SqliteConnection(datasource);
        connection.Open();
        
        Options = new DbContextOptionsBuilder<v1PhoneAssistantDbContext>().UseSqlite(connection!).Options;
        return connection;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                DbContext.Dispose();
                _connection.Close();
                _connection.Dispose();
            }

            // TO DO: free unmanaged resources (unmanaged objects) and override finalizer
            // TO DO: set large fields to null
            _disposedValue = true;
        }
    }

    // TO DO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~DbTestHelper()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}