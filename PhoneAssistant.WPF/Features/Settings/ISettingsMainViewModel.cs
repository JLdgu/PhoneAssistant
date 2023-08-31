using PhoneAssistant.WPF.Shared;

namespace PhoneAssistant.WPF.Features.Settings;

public interface ISettingsMainViewModel : IViewModel
{
    public string Database { get; set; }

    public string Printer { get; set; }
    
    public string VersionDescription { get; set; }
}