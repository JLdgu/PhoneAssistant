using System.Text.Json;
using TUnit.Assertions.AssertConditions.Throws;

namespace PhoneAssistant.Model.Tests.Repositories;

public sealed class ApplicationSettingsRepositoryTests
{
    private readonly string _appSettingsPath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
    private readonly JsonSerializerOptions _options = new() { WriteIndented = true };

    [Test]
    [NotInParallel]
    public async Task Constructor_ShouldCreateAppSettings_WhenItDoesNotExist()
    {
        if (File.Exists(_appSettingsPath))
            File.Delete(_appSettingsPath);        

        ApplicationSettingsRepository repository = new();

        await Assert.That(File.Exists(_appSettingsPath)).IsTrue();
        string json = File.ReadAllText(_appSettingsPath);
        var appSettings = JsonSerializer.Deserialize<ApplicationSettings>(json) ?? new ApplicationSettings();

        await Assert.That(appSettings.DefaultDecommissionedTicket).IsEqualTo(repository.ApplicationSettings.DefaultDecommissionedTicket);
    }

    [Test]
    [NotInParallel]
    public async Task Constructor_ShouldNotCreateAppSettings_WhenItAlreadyExists()
    {
        if (File.Exists(_appSettingsPath))
            File.Delete(_appSettingsPath);

        ApplicationSettings applicationSettings = new() { DefaultDecommissionedTicket = 9999999 };
        string json = JsonSerializer.Serialize(applicationSettings, _options);
        File.WriteAllText(_appSettingsPath, json);

        ApplicationSettingsRepository repository = new();

        await Assert.That(repository.ApplicationSettings.DefaultDecommissionedTicket).IsEqualTo(9999999);
    }

    [Test]
    [NotInParallel]
    public async Task Constructor_ShouldThrowException_WhenAppSettingsMalformed()
    {
        if (File.Exists(_appSettingsPath))
            File.Delete(_appSettingsPath);

        string json = JsonSerializer.Serialize("applicationSettings", _options);
        File.WriteAllText(_appSettingsPath, json);

        await Assert.That(() => { ApplicationSettingsRepository repository = new(); }).Throws<JsonException>();        
    }

}
