using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using PhoneAssistant.WPF.Application;

using Serilog;

using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Windows;
using Velopack;

namespace PhoneAssistant.WPF.Features.Settings;

public sealed partial class SettingsMainViewModel : ObservableValidator, ISettingsMainViewModel
{
    private readonly IUserSettings _userSettings;
    private readonly IThemeWrapper _themeWrapper;
    const string ReleaseUrl = @"\\countyhall.ds2.devon.gov.uk\docs\exeter, county hall\FITProject\ICTS\Mobile Phones\PhoneAssistant\Application";
    private UpdateManager _updateManager;
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

        DefaultDecommissionedTicket = _userSettings.DefaultDecommissionedTicket.ToString();

        ColourThemeDark = _userSettings.DarkMode;
        ColourThemeLight = !_userSettings.DarkMode;

        Log.Verbose("SettingsMainViewModel constructor called");
        CurrentVersion = _userSettings.AssemblyVersion?.ToString();
        _updateManager = new(ReleaseUrl, 
            new UpdateOptions() { AllowVersionDowngrade = true });        
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

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Range(100000,999999, ErrorMessage = "Ticket must be 6 digits")]
    private string _defaultDecommissionedTicket;

    partial void OnDefaultDecommissionedTicketChanged(string value)
    {
        if (_userSettings.DefaultDecommissionedTicket.ToString() == value) return;

        int ticket = int.Parse(value);
        _userSettings.DefaultDecommissionedTicket = ticket;
        _userSettings.Save();
    }

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
