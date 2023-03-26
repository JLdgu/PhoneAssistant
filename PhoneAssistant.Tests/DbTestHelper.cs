using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

using PhoneAssistant.WPF.Features.Application;

namespace PhoneAssistant.Tests;

public class DbTestHelper : IDisposable
{
    //internal SqliteConnection? Connection;
    public DbContextOptions<PhoneAssistantDbContext>? Options;
    private bool _disposedValue;

    internal SqliteConnection CreateConnection(string datasource = "DataSource=:memory:;")
    {
        var connection = new SqliteConnection(datasource);
        connection.Open();
        
        Options = new DbContextOptionsBuilder<PhoneAssistantDbContext>().UseSqlite(connection!).Options;
        return connection;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                // TO DO: dispose managed state (managed objects)
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