using System.Configuration;

namespace PhoneAssistant.WPF.Application;

public sealed class UserSettings : ApplicationSettingsBase, IUserSettings
{
    [UserScopedSetting()]
    public string Database
    {
        get => (string)this[nameof(Database)];
        set => this[nameof(Database)] = value;
    }

    [UserScopedSetting()]
    [DefaultSettingValue(@"\\DS2CHL283.ds2.devon.gov.uk\Ricoh-C3503-np04304")]
    public string Printer
    {
        get => (string)this[nameof(Printer)];
        set => this[nameof(Printer)] = value;
    }

    [UserScopedSetting()]
    [DefaultSettingValue("false")]
    public bool PrintToFile
    {
        get => (bool)this[nameof(PrintToFile)];
        set => this[nameof(PrintToFile)] = value;
    }

    [UserScopedSetting()]
    public string PrintFile
    {
        get => (string)this[nameof(PrintFile)];
        set => this[nameof(PrintFile)] = value;
    }

    [UserScopedSetting()]
    [DefaultSettingValue("false")]
    public bool DarkMode
    {
        get => (bool)this[nameof(DarkMode)];
        set => this[nameof(DarkMode)] = value;
    }

    [UserScopedSetting()]
    [DefaultSettingValue("true")]
    public bool UpdateUserSettingsRequired
    {
        get => (bool)this[nameof(UpdateUserSettingsRequired)];
        set => this[nameof(UpdateUserSettingsRequired)] = value;
    }

    public override void Save()
    {
        base.Save();
    }

    public Version? AssemblyVersion
    {
        get
        {
            var assembly = typeof(App).Assembly;
            var assemblyName = assembly.GetName();
            return assemblyName.Version;
        }
    }

}

