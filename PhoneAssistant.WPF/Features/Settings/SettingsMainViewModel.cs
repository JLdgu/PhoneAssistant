using CommunityToolkit.Mvvm.ComponentModel;
using PhoneAssistant.WPF.Application;
using PhoneAssistant.WPF.Application.Entities;
using PhoneAssistant.WPF.Shared;
using System;
using System.Threading.Tasks;

namespace PhoneAssistant.WPF.Features.Settings;

internal sealed partial class SettingsMainViewModel : ObservableObject, IViewModel
{
    private readonly AppRepository _appRepository;

    [ObservableProperty]
    private string _versionDescription = "Phone Assistant - Version : Unknown";

    public SettingsMainViewModel(AppRepository appRepository)
    {
        _appRepository = appRepository;
    }

    public Task LoadAsync()
    {
        VersionDescription = _appRepository.VersionDescription;
        return Task.CompletedTask;
    }
}
