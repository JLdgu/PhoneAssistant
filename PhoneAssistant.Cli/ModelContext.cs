using Microsoft.EntityFrameworkCore;
using PhoneAssistant.Model;
using Serilog;

namespace PhoneAssistant.Cli;

internal static class ModelContext
{
    internal static PhoneAssistantDbContext Create()
    {
        ApplicationSettingsRepository settingsRepository = new();
        Log.Information("Applying update to {0}", settingsRepository.ApplicationSettings.Database);

        string connectionString = $@"DataSource={settingsRepository.ApplicationSettings.Database};";
        var options = new DbContextOptionsBuilder<PhoneAssistantDbContext>()
            .UseSqlite(connectionString)
            .Options;
        PhoneAssistantDbContext dbContext = new(options);
        return dbContext;
    }
}
