using CommunityToolkit.Mvvm.ComponentModel;
using PhoneAssistant.WPF.Shared;
using System;
using System.Threading.Tasks;

namespace PhoneAssistant.WPF.Features.Settings;

internal sealed partial class SettingsMainViewModel : ObservableObject, IViewModel
{
    [ObservableProperty]
    private string _versionDescription = "Some number";

    public Task LoadAsync()
    {                
        var assembly = typeof(App).Assembly;
        var assemblyName = assembly.GetName();

        VersionDescription = $"Phone Assistant - Version : {assemblyName.Version}";        

        return Task.CompletedTask;
    }
}
