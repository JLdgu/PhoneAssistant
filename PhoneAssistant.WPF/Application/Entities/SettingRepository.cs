using PhoneAssistant.WPF.Features.Application;

namespace PhoneAssistant.WPF.Application.Entities;
public sealed class SettingRepository : ISettingRepository
{
    private readonly PhoneAssistantDbContext _dbContext;

    public SettingRepository(PhoneAssistantDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<string> GetAsync()
    {
        Setting? setting = await _dbContext.Setting.FindAsync(1);
        string minVersion = string.Empty;
        if (setting is not null && setting.MinimumVersion is not null)
            minVersion = setting.MinimumVersion;

        return minVersion;
    }
}
