using System.IO;
using System.Windows;

using Microsoft.Win32;

namespace PhoneAssistant.WPF.Application;

public static class ApplicationUpdate
{
    public static void UpdateUserSettings()
    {
        UserSettings userSettings = new();
        if (userSettings.UpdateUserSettingsRequired)
        {
            userSettings.Upgrade();
            userSettings.UpdateUserSettingsRequired = false;
            userSettings.Save();
        }
    }

    public static bool FirstRun()
    {
        UserSettings userSettings = new();
        if (userSettings.Database is not null && File.Exists(userSettings.Database))
            return false;

        MessageBox.Show($"Select the Phone Assistant database to use.{Environment.NewLine}Application will need to restart.", "Phone Assistant", MessageBoxButton.OK, MessageBoxImage.Question);

        OpenFileDialog openFileDialog = new()
        {
            Filter = "SQLite Database (*.db)|*.db",
            Multiselect = false
        };

        if (openFileDialog.ShowDialog() == true)
        {
            userSettings.Database = openFileDialog.FileName;
            userSettings.Save();
        }        
        return true;
    }
}