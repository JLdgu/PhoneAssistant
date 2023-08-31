namespace PhoneAssistant.WPF.Application;

public interface IUserSettings
{
    string Database { get; set; }

    bool PrintToFile { get; set; }

    string Printer { get; set; }

    string PrintFile { get; set; }
    
    void Save() { }

    Version? AssemblyVersion { get; }
}