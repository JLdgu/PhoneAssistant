namespace PhoneAssistant.WPF.Application.Entities;

public class Setting
{
    public int Id { get; set; }

    public string MinimumVersion { get; set; }

    public Setting(int id, string minimumVersion)
    {
        Id = id;
        MinimumVersion = minimumVersion;
    }
}
