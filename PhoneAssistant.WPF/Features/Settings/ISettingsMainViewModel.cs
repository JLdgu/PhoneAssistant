using PhoneAssistant.WPF.Shared;

namespace PhoneAssistant.WPF.Features.Settings;

public interface ISettingsMainViewModel : IViewModel
{
    public string VersionDescription { get; set; }
}