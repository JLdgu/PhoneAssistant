using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Microsoft.Win32;

using PhoneAssistant.WPF.Application;

namespace PhoneAssistant.WPF.Features.Settings;

public sealed partial class SettingsMainViewModel : ObservableObject, ISettingsMainViewModel
{
    private readonly IUserSettings _userSettings;
    private readonly IThemeWrapper _themeWrapper;

#pragma warning disable CS8618
    public SettingsMainViewModel(IUserSettings userSettings, IThemeWrapper themeWrapper)
    {
        _userSettings = userSettings ?? throw new ArgumentNullException(nameof(userSettings));
        _themeWrapper = themeWrapper ?? throw new ArgumentNullException(nameof(themeWrapper));
        Database = _userSettings.Database;
        PrintToFile = _userSettings.PrintToFile;
        PrintToPrinter = !PrintToFile;
        Printer = _userSettings.Printer;
        PrintFile = _userSettings.PrintFile;
        ColourThemeDark = _userSettings.DarkMode;
        ColourThemeLight = !_userSettings.DarkMode;

        VersionDescription = _userSettings.AssemblyVersion?.ToString();

    }
#pragma warning restore CS8618

    #region Database Settings
    [ObservableProperty]
    private string _database;

    partial void OnDatabaseChanged(string value)
    {
        if (_userSettings.Database != value)
        {
            _userSettings.Database = value;
            _userSettings.Save();
        }
    }

    [RelayCommand]
    private void ChangeDatabase() 
    {
        MessageBox.Show($"Select the Phone Assistant database to use.{Environment.NewLine}Application will need to restart.", "Phone Assistant", MessageBoxButton.OK, MessageBoxImage.Question);

        OpenFileDialog openFileDialog = new()
        {
            Filter = "SQLite Database (*.db)|*.db",
            Multiselect = false
        };

        if (openFileDialog.ShowDialog() == true)
        {
            _userSettings.Database = openFileDialog.FileName;
            _userSettings.Save();
            App.Current.Shutdown();
        }
    }
    #endregion

    #region Printer Settings
    [ObservableProperty]
    private bool _printToPrinter;

    [ObservableProperty]
    private bool _printToFile;

    partial void OnPrintToFileChanged(bool value)
    { 
        _userSettings.PrintToFile = value;
        _userSettings.Save();
    }

    [ObservableProperty]
    private string _printer;
    partial void OnPrinterChanged(string value)
    {
        if (_userSettings.Printer == value) return;

        _userSettings.Printer = value;
        _userSettings.Save();
    }

    [ObservableProperty]
    private string _printFile;

    partial void OnPrintFileChanged(string value)
    {
        if (_userSettings.PrintFile == value) return;

        _userSettings.PrintFile = value;
        _userSettings.Save();

    }

    [ObservableProperty]
    private bool canSavePrintFile;
    #endregion

    #region Mode Setting

    [ObservableProperty]
    private bool colourThemeDark;

    partial void OnColourThemeDarkChanged(bool value)
    {
        _userSettings.DarkMode = value;
        _userSettings.Save();

        _themeWrapper.ModifyTheme(value);
    }

    [ObservableProperty]
    private bool colourThemeLight;
    #endregion

    [ObservableProperty]
    private string? _versionDescription;

    public Task LoadAsync()
    {
        return Task.CompletedTask;
    }

    public Task WindowClosingAsync()
    {
        return Task.CompletedTask;
    }
}
