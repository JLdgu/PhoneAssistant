using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

using PhoneAssistant.WPF.Features.Application;

namespace PhoneAssistant.Tests;

public class DbTestHelper : IDisposable
{
    //internal SqliteConnection? Connection;
    public DbContextOptions<PhoneAssistantDbContext>? Options;
    private bool _disposedValue;

    internal SqliteConnection CreateConnection()
    {
        const string IN_MEMORY = "DataSource=:memory:";
        var Connection = new SqliteConnection(IN_MEMORY);
        Connection.Open();
        
        Options = new DbContextOptionsBuilder<PhoneAssistantDbContext>().UseSqlite(Connection!).Options;
        return Connection;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            _disposedValue = true;
        }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
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