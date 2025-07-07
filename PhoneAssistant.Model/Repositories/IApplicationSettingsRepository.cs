namespace PhoneAssistant.Model;

public interface IApplicationSettingsRepository
{
    ApplicationSettings ApplicationSettings { get; init; }

    void Save();
}