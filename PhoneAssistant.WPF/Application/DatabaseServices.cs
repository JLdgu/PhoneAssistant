using System.IO;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace PhoneAssistant.WPF.Application;

internal static class DatabaseServices
{
    internal static void BackupDatabase(IHost host)
    {
        IUserSettings settings = host.Services.GetRequiredService<IUserSettings>();

        if (!Path.Exists(settings.Database)) return;

        DirectoryInfo dbPath = new(Path.Combine(Path.GetDirectoryName(settings.Database)!, "Backup"));
        string dbName = new FileInfo(settings.Database).Name;
        string[] dbNameSplit = dbName.Split('.');
        string filter = dbNameSplit[0] + "*." + dbNameSplit[1];

        bool recent = false;
        int backupCount = 0;
        try
        {
            foreach (FileInfo oldBackup in dbPath.GetFiles(filter).OrderByDescending(b => b.LastWriteTime))
            {
                if (oldBackup.LastWriteTime > DateTime.Now.AddDays(-7))
                    recent = true;

                backupCount++;
                if (backupCount > 6 && oldBackup.LastWriteTime < DateTime.Now.AddDays(-31))
                    oldBackup.Delete();
            }

            if (recent) return;
            string newBackup = Path.Combine(dbPath.FullName, dbName.Replace(".", $"{DateTime.Now.ToString("yyy-MM-dd")}."));
            File.Copy(settings.Database, newBackup);
        }
        catch (Exception)
        { }
    }
}
