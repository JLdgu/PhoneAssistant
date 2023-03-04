namespace PhoneAssistant.WPF.Features.Settings;

public interface ISettingsRepository
{
    Task<string> GetAsync();
}