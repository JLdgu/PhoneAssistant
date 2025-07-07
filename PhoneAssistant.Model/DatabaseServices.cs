using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace PhoneAssistant.Model;

public static class DatabaseServices
{
    public static void BackupDatabase(IHost host)
    {
        IApplicationSettingsRepository repo =  host.Services.GetRequiredService<IApplicationSettingsRepository>();

        string database = repo.ApplicationSettings.Database;
        if (!File.Exists(database)) throw new FileNotFoundException();

        DirectoryInfo dbPath = new(Path.Combine(Path.GetDirectoryName(database)!, "Backup"));
        string dbName = new FileInfo(database).Name;
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
            File.Copy(database, newBackup);
        }
        catch (Exception)
        { }
    }
}
