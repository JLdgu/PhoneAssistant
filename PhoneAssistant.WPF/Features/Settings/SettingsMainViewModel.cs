using CommunityToolkit.Mvvm.ComponentModel;

using PhoneAssistant.WPF.Application;
using PhoneAssistant.WPF.Shared;

namespace PhoneAssistant.WPF.Features.Settings;

public sealed partial class SettingsMainViewModel : ObservableObject, IViewModel
{
    private readonly IAppRepository _appRepository;

    [ObservableProperty]
    private string _versionDescription = "Phone Assistant - Version : Unknown";

    public SettingsMainViewModel(IAppRepository appRepository)
    {
        _appRepository = appRepository;
    }

    public Task LoadAsync()
    {
        VersionDescription = _appRepository.VersionDescription;

        return Task.CompletedTask;
    }
}
