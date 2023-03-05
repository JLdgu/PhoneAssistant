using PhoneAssistant.WPF.Features.Settings;

namespace PhoneAssistant.WPF.Application;

public class AppRepository : IAppRepository
{
    private readonly ISettingsRepository _settingsRepository;
    private Version? _assemblyVersion;

    public AppRepository(ISettingsRepository settingRepository)
    {
        _settingsRepository = settingRepository;

        AssemblyVersion();

        VersionDescription = $"Phone Assistant - Version : {_assemblyVersion}";
    }

    public string VersionDescription { get; init; }

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
    }
}
