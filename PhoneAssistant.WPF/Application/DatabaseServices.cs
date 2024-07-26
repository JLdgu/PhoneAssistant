using System.IO;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace PhoneAssistant.WPF.Application;

internal static class DatabaseServices
{
    internal static void ConfigureDatabase(IHost host)
    {
        PhoneAssistantDbContext dbContext = host.Services.GetRequiredService<PhoneAssistantDbContext>();

        dbContext.Database.EnsureCreated();
    }

    internal static void BackupDatabase(IHost host)
    {
        IUserSettings settings = host.Services.GetRequiredService<IUserSettings>();
        
        if (!Path.Exists(settings.Database)) return;

        DirectoryInfo dbPath = new DirectoryInfo(Path.Combine( Path.GetDirectoryName(settings.Database)!,"Backup"));
        string dbName = new FileInfo(settings.Database).Name;

        bool recent = dbPath.GetFiles(dbName.Replace(".", "*."))
            .Where(f => f.CreationTime > DateTime.Now.AddDays(-7))
            .Any();

        if (recent) return;

        string newBackup = Path.Combine(dbPath.FullName, dbName.Replace(".", $"{DateTime.Now.ToString("yyy-MM-dd")}."));
        try
        {
            File.Copy(settings.Database, newBackup);        
        }
        catch (Exception)
        { }
    }
}
