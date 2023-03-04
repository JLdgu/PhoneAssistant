using PhoneAssistant.WPF.Application.Entities;
using PhoneAssistant.WPF.Features.Application;

namespace PhoneAssistant.WPF.Features.Settings;

public sealed class SettingsRepository : ISettingsRepository
{
    private readonly PhoneAssistantDbContext _dbContext;

    public SettingsRepository(PhoneAssistantDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<string> GetAsync()
    {
        SettingEntity? setting = await _dbContext.Setting.FindAsync(1);
        string minVersion = string.Empty;
        if (setting is not null && setting.MinimumVersion is not null)
            minVersion = setting.MinimumVersion;

        return minVersion;
    }
}
