using System.Diagnostics;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Microsoft.Win32;

using PhoneAssistant.WPF.Application;

using Velopack;

namespace PhoneAssistant.WPF.Features.Settings;

public sealed partial class SettingsMainViewModel : ObservableObject, ISettingsMainViewModel
{
    private readonly IUserSettings _userSettings;
    private readonly IThemeWrapper _themeWrapper;
    const string _releaseUrl = @"K:\FITProject\ICTS\Mobile Phones\PhoneAssistant\Velopack";
    private readonly UpdateManager _updateManager;
    private UpdateInfo? _updateInfo;        

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

        DymoPrintToFile = _userSettings.DymoPrintToFile;
        DymoPrintToPrinter = !DymoPrintToFile;
        DymoPrinter = _userSettings.DymoPrinter;
        DymoPrintFile = _userSettings.DymoPrintFile;

        ColourThemeDark = _userSettings.DarkMode;
        ColourThemeLight = !_userSettings.DarkMode;

        CurrentVersion = _userSettings.AssemblyVersion?.ToString();
        _updateManager = new(_releaseUrl);
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
    #endregion

    #region DymoPrinter Settings
    [ObservableProperty]
    private bool _dymoPrintToPrinter;

    [ObservableProperty]
    private bool _dymoPrintToFile;

    partial void OnDymoPrintToFileChanged(bool value)
    {
        _userSettings.DymoPrintToFile = value;
        _userSettings.Save();
    }

    [ObservableProperty]
    private string _dymoPrinter;
    partial void OnDymoPrinterChanged(string value)
    {
        if (_userSettings.DymoPrinter == value) return;

        _userSettings.DymoPrinter = value;
        _userSettings.Save();
    }

    [ObservableProperty]
    private string _dymoPrintFile;

    partial void OnDymoPrintFileChanged(string value)
    {
        if (_userSettings.DymoPrintFile == value) return;

        _userSettings.DymoPrintFile = value;
        _userSettings.Save();
    }
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

    #region Update Application

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CheckForUpdateCommand))]
    [NotifyCanExecuteChangedFor(nameof(DownloadUpdateCommand))]
    [NotifyCanExecuteChangedFor(nameof(UpdateAndRestartCommand))]
    private ApplicationUpdateState _updateState = ApplicationUpdateState.Default;

    [ObservableProperty]
    private string? _currentVersion;

    private bool CanCheckForUpdate() => UpdateState == ApplicationUpdateState.Default;    

    [RelayCommand(CanExecute = nameof(CanCheckForUpdate))]    
    private async Task CheckForUpdate()
    {
        Trace.TraceInformation("CheckForUpdate Started");

        if (_updateManager is null)
            return;

        _updateInfo = await _updateManager.CheckForUpdatesAsync().ConfigureAwait(true);
        Trace.TraceInformation("UpdateCommand CheckForUpdatesAsync() called");
        
        if (_updateInfo is null) return;

        NewVersion = _updateInfo.TargetFullRelease.Version.ToString() ?? "None";
        UpdateState = ApplicationUpdateState.UpdateAvailable;
    }

    [ObservableProperty]
    private string? _newVersion = "None";

    private bool CanDownload() => UpdateState == ApplicationUpdateState.UpdateAvailable;

    [RelayCommand(CanExecute = nameof(CanDownload))]    
    private async Task DownloadUpdate()
    {
        if (_updateManager is null || _updateInfo is null)
            return;

        await _updateManager.DownloadUpdatesAsync(_updateInfo).ConfigureAwait(true);
        Trace.TraceInformation("DownloadUpdate DownloadUpdatesAsync() called");
        UpdateState = ApplicationUpdateState.UpdateDownloaded;
    }

    private bool CanUpdate() => UpdateState == ApplicationUpdateState.UpdateDownloaded;

    [RelayCommand(CanExecute = nameof(CanUpdate))]
    private void UpdateAndRestart()
    {
        if (_updateManager is null || _updateInfo is null)
            return;

        Trace.TraceInformation("UpdateAndRestart Started");

        _updateManager.ApplyUpdatesAndRestart(_updateInfo);

        Trace.TraceInformation("UpdateAndRestart Completed");
    }
    #endregion

    public Task LoadAsync()
    {
        return Task.CompletedTask;
    }
}
public enum ApplicationUpdateState
{
    Default,
    UpdateAvailable,
    UpdateDownloaded
}
