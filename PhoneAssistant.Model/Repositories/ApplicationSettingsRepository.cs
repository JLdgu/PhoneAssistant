namespace PhoneAssistant.Model;

public sealed class ApplicationSettingsRepository : IApplicationSettingsRepository
{
    private readonly string _appSettingsPath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");

    public ApplicationSettings ApplicationSettings { get; init; }

    public ApplicationSettingsRepository()
    {
        if (!File.Exists(_appSettingsPath))
        {
            ApplicationSettings = new ApplicationSettings();
            Save();
        }
        else
        {
            string json = File.ReadAllText(_appSettingsPath);
            ApplicationSettings = System.Text.Json.JsonSerializer.Deserialize<ApplicationSettings>(json) ?? throw new InvalidOperationException();
        }
    }

    public void Save()
    {
        string json = System.Text.Json.JsonSerializer.Serialize(ApplicationSettings, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_appSettingsPath, json);
    }
}
