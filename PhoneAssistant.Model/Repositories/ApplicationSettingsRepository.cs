namespace PhoneAssistant.Model;

public sealed class ApplicationSettingsRepository : IApplicationSettingsRepository
{
    private readonly string _appSettingsPath;
    private const string AppSettingsJSON = "appsettings.json";

    public ApplicationSettings ApplicationSettings { get; init; }

    public ApplicationSettingsRepository()
    {
        string currentPath = Path.Combine(Directory.GetCurrentDirectory(), AppSettingsJSON);
#if DEBUG
        _appSettingsPath = currentPath;

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
#else
        string parentDir = Directory.GetParent(Directory.GetCurrentDirectory())?.FullName ?? Directory.GetCurrentDirectory();
        string parentPath = Path.Combine(parentDir, AppSettingsJSON);

        if (File.Exists(parentPath))
        {
            _appSettingsPath = parentPath;
            string json = File.ReadAllText(_appSettingsPath);
            ApplicationSettings = System.Text.Json.JsonSerializer.Deserialize<ApplicationSettings>(json) ?? throw new InvalidOperationException();
        }
        else if (File.Exists(currentPath))
        {
            // Read from current directory and persist to parent directory
            string json = File.ReadAllText(currentPath);
            ApplicationSettings = System.Text.Json.JsonSerializer.Deserialize<ApplicationSettings>(json) ?? throw new InvalidOperationException();
            _appSettingsPath = parentPath;
            Save();
        }
        else
        {
            ApplicationSettings = new ApplicationSettings();
            _appSettingsPath = parentPath;
            Save();
        }
#endif
    }

    public void Save()
    {
        string json = System.Text.Json.JsonSerializer.Serialize(ApplicationSettings, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_appSettingsPath, json);
    }
}
