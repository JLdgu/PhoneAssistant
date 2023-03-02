using PhoneAssistant.WPF.Application.Entities;
using System;
using System.Threading.Tasks;

namespace PhoneAssistant.WPF.Application;
public class AppRepository
{
    private readonly ISettingRepository _settingRepository;

    public AppRepository(ISettingRepository settingRepository)
    {
        _settingRepository = settingRepository;

        AssemblyVersion();
    }

    public string? VersionDescription;
    private Version? _assemblyVersion;

    public async Task<bool> InvalidVersionAsync()
    {
        string version = await _settingRepository.GetAsync();
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
