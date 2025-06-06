using System.Configuration;
using System.Windows;

using Microsoft.Win32;

using PhoneAssistant.WPF.Features.MainWindow;

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
    [DefaultSettingValue("Dymo LabelWriter 450")]
    public string DymoPrinter
    {
        get => (string)this[nameof(DymoPrinter)];
        set => this[nameof(DymoPrinter)] = value;
    }

    [UserScopedSetting()]
    [DefaultSettingValue("false")]
    public bool DymoPrintToFile
    {
        get => (bool)this[nameof(DymoPrintToFile)];
        set => this[nameof(DymoPrintToFile)] = value;
    }

    [UserScopedSetting()]
    public string DymoPrintFile
    {
        get => (string)this[nameof(DymoPrintFile)];
        set => this[nameof(DymoPrintFile)] = value;
    }


    [UserScopedSetting()]
    [DefaultSettingValue("false")]
    public bool DarkMode
    {
        get => (bool)this[nameof(DarkMode)];
        set => this[nameof(DarkMode)] = value;
    }

    [UserScopedSetting()]
    [DefaultSettingValue("263323")]
    public int DefaultDecommissionedTicket
    {
        get => (int)this[nameof(DefaultDecommissionedTicket)];
        set => this[nameof(DefaultDecommissionedTicket)] = value;
    }

    [UserScopedSetting()]
    [DefaultSettingValue("0")]
    public ViewModelType CurrentView
    {
        get => (ViewModelType)this[nameof(CurrentView)];
        set => this[nameof(CurrentView)] = value;
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

    public static bool DatabaseFullPathRetrieved()
    {

        UserSettings userSettings = new();
        if (userSettings.UpdateUserSettingsRequired)
        {
            userSettings.Upgrade();
            userSettings.UpdateUserSettingsRequired = false;
            userSettings.Save();
        }

        if (userSettings.Database is not null)
            return true;

        MessageBox.Show($"Select the Phone Assistant database to use.", "Phone Assistant", MessageBoxButton.OK, MessageBoxImage.Question);

        OpenFileDialog openFileDialog = new()
        {
            Filter = "SQLite Database (*.db)|*.db",
            Multiselect = false
        };

        if (openFileDialog.ShowDialog() == false)
            return false;

        userSettings.Database = openFileDialog.FileName;
        userSettings.Save();
        return true;
    }
}

