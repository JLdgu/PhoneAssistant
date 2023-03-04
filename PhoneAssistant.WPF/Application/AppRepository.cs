using PhoneAssistant.WPF.Features.Settings;

namespace PhoneAssistant.WPF.Application;
public class AppRepository
{
    private readonly ISettingsRepository _settingsRepository;

    public AppRepository(ISettingsRepository settingRepository)
    {
        _settingsRepository = settingRepository;

        AssemblyVersion();
    }

    public string? VersionDescription;
    private Version? _assemblyVersion;

    public async Task<bool> InvalidVersionAsync()
    {
        string version = await _settingsRepository.GetAsync();
        var dbMinVersion = new Version(version);


        var result = _assemblyVersion!.CompareTo(dbMinVersion);
        if (result < 0) return true;

        return false;
    }

    private void AssemblyVersion()
    {
        var assembly = typeof(App).Assembly;
        var assemblyName = assembly.GetName();
        _assemblyVersion = assemblyName.Version;

        VersionDescription = $"Phone Assistant - Version : {_assemblyVersion}";
    }
}
