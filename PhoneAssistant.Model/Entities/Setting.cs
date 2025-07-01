namespace PhoneAssistant.Model;

public class Setting(int id, string minimumVersion)
{
    public int Id { get; set; } = id;

    public string MinimumVersion { get; set; } = minimumVersion;
}
