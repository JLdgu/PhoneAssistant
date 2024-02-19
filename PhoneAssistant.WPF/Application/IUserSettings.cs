using PhoneAssistant.WPF.Features.MainWindow;

namespace PhoneAssistant.WPF.Application;

public interface IUserSettings
{
    ViewModelType CurrentView { get; set; }
    
    bool DarkMode { get; set; }

    string Database { get; set; }

    bool PrintToFile { get; set; }

    string Printer { get; set; }

    string PrintFile { get; set; }    

    void Save() { }

    Version? AssemblyVersion { get; }
}