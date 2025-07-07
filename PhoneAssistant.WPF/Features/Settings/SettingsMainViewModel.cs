using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using PhoneAssistant.Model;
using Serilog;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Windows;
using Velopack;

namespace PhoneAssistant.WPF.Features.Settings;

public sealed partial class SettingsMainViewModel : ObservableValidator, ISettingsMainViewModel
{
    private readonly IApplicationSettingsRepository _appSettings;
    private readonly IThemeWrapper _themeWrapper;
    private readonly UpdateManager _updateManager;
    private UpdateInfo? _updateInfo;

    public SettingsMainViewModel(IApplicationSettingsRepository appSettings, IThemeWrapper themeWrapper, UpdateManager updateManager)
    {
        _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
        _themeWrapper = themeWrapper ?? throw new ArgumentNullException(nameof(themeWrapper));
        _updateManager = updateManager ?? throw new ArgumentNullException(nameof(updateManager));

        ColourThemeDark = _appSettings.ApplicationSettings.DarkMode;
        ColourThemeLight = !_appSettings.ApplicationSettings.DarkMode;

        Database = _appSettings.ApplicationSettings.Database;

        DefaultDecommissionedTicket = _appSettings.ApplicationSettings.DefaultDecommissionedTicket.ToString();

        DymoPrintToFile = _appSettings.ApplicationSettings.DymoPrintToFile;
        DymoPrintToPrinter = !DymoPrintToFile;
        DymoPrinter = _appSettings.ApplicationSettings.DymoPrinter;
        DymoPrintFile = _appSettings.ApplicationSettings.DymoPrintFile;

        PrintToFile = _appSettings.ApplicationSettings.PrintToFile;
        PrintToPrinter = !PrintToFile;
        Printer = _appSettings.ApplicationSettings.Printer;
        PrintFile = _appSettings.ApplicationSettings.PrintFile;

        Log.Verbose("SettingsMainViewModel constructor called");
        CurrentVersion = AssemblyVersion()?.ToString();
    }

    private static Version? AssemblyVersion()
    {
        Assembly assembly = typeof(App).Assembly;
        AssemblyName assemblyName = assembly.GetName();
        return assemblyName.Version;
    }

    #region Database Settings
    [ObservableProperty]
    private string _database;

    partial void OnDatabaseChanged(string value)
    {
        if (_appSettings.ApplicationSettings.Database != value)
        {
            _appSettings.ApplicationSettings.Database = value;
            _appSettings.Save();
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
            _appSettings.ApplicationSettings.Database = openFileDialog.FileName;
            _appSettings.Save();
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
        _appSettings.ApplicationSettings.PrintToFile = value;
        _appSettings.Save();
    }

    [ObservableProperty]
    private string _printer;
    partial void OnPrinterChanged(string value)
    {
        if (_appSettings.ApplicationSettings.Printer == value) return;

        _appSettings.ApplicationSettings.Printer = value;
        _appSettings.Save();
    }

    [ObservableProperty]
    private string _printFile;

    partial void OnPrintFileChanged(string value)
    {
        if (_appSettings.ApplicationSettings.PrintFile == value) return;

        _appSettings.ApplicationSettings.PrintFile = value;
        _appSettings.Save();
    }
    #endregion

    #region DymoPrinter Settings
    [ObservableProperty]
    private bool _dymoPrintToPrinter;

    [ObservableProperty]
    private bool _dymoPrintToFile;

    partial void OnDymoPrintToFileChanged(bool value)
    {
        _appSettings.ApplicationSettings.DymoPrintToFile = value;
        _appSettings.Save();
    }

    [ObservableProperty]
    private string _dymoPrinter;
    partial void OnDymoPrinterChanged(string value)
    {
        if (_appSettings.ApplicationSettings.DymoPrinter == value) return;

        _appSettings.ApplicationSettings.DymoPrinter = value;
        _appSettings.Save();
    }

    [ObservableProperty]
    private string _dymoPrintFile;

    partial void OnDymoPrintFileChanged(string value)
    {
        if (_appSettings.ApplicationSettings.DymoPrintFile == value) return;

        _appSettings.ApplicationSettings.DymoPrintFile = value;
        _appSettings.Save();
    }
    #endregion

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Range(100000,999999, ErrorMessage = "Ticket must be 6 digits")]
    private string _defaultDecommissionedTicket;

    partial void OnDefaultDecommissionedTicketChanged(string value)
    {
        if (_appSettings.ApplicationSettings.DefaultDecommissionedTicket.ToString() == value) return;

        int ticket = int.Parse(value);
        _appSettings.ApplicationSettings.DefaultDecommissionedTicket = ticket;
        _appSettings.Save();
    }

    #region Mode Setting
    [ObservableProperty]
    private bool colourThemeDark;

    partial void OnColourThemeDarkChanged(bool value)
    {
        _appSettings.ApplicationSettings.DarkMode = value;
        _appSettings.Save();

        _themeWrapper.ModifyTheme(value);
    }

    [ObservableProperty]
    private bool colourThemeLight;
    #endregion

    #region Update Application

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(UpdateAndRestartCommand))]
    private ApplicationUpdateState _updateState = ApplicationUpdateState.Default;

    [ObservableProperty]
    private string? _currentVersion;

    private async Task CheckForUpdate()
    {
        Log.Information("CheckForUpdate Started");

        if (_updateManager is null)
            return;
            
        if (!_updateManager.IsInstalled)
        {
            NewVersion = $" Version 9.9.999 available";
            UpdateState = ApplicationUpdateState.UpdateAvailable;
            return;
        }

        _updateInfo = await _updateManager.CheckForUpdatesAsync().ConfigureAwait(true);
        Log.Information("UpdateCommand CheckForUpdatesAsync() called");

        if (_updateInfo is null)
        {
            UpdateState = ApplicationUpdateState.NoUpdateAvailable;
            return;
        }

        NewVersion = $" Version {_updateInfo.TargetFullRelease.Version} available" ?? "No updates outstanding";
        UpdateState = ApplicationUpdateState.UpdateAvailable;
    }

    [ObservableProperty]
    private string? _newVersion = "No updates outstanding";

    private bool CanUpdate() => UpdateState == ApplicationUpdateState.UpdateAvailable;

    [RelayCommand(CanExecute = nameof(CanUpdate))]
    private async Task UpdateAndRestart()
    {
        if (_updateManager is null || _updateInfo is null)
            return;

        Log.Information("UpdateAndRestart Started");
        
        await _updateManager.DownloadUpdatesAsync(_updateInfo).ConfigureAwait(true);
        Log.Information("DownloadUpdate DownloadUpdatesAsync() called");
        UpdateState = ApplicationUpdateState.UpdateDownloaded;

        _updateManager.ApplyUpdatesAndRestart(_updateInfo);
        Log.Information("UpdateAndRestart Completed");
    }
    #endregion

    public async Task LoadAsync()
    {
        if (UpdateState == ApplicationUpdateState.Default)
            await CheckForUpdate();
    }
}
public enum ApplicationUpdateState
{
    Default,
    NoUpdateAvailable,
    UpdateAvailable,
    UpdateDownloaded
}
