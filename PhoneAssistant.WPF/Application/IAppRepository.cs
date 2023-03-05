namespace PhoneAssistant.WPF.Application;

public interface IAppRepository
{
    string VersionDescription { get; init; }

    Task<bool> InvalidVersionAsync();
}