using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using PhoneAssistant.Model;
using PhoneAssistant.WPF.Shared;
using Serilog;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using Velopack;

namespace PhoneAssistant.WPF.Features.Settings;

public sealed partial class SettingsMainViewModel : ViewModelValidatorBase, ISettingsMainViewModel
{
    private readonly IApplicationSettingsRepository _appSettings;
    private readonly IThemeWrapper _themeWrapper;
    private readonly UpdateManager _updateManager;
    private readonly ILogger _logger;
    private UpdateInfo? _updateInfo;

    public SettingsMainViewModel(IApplicationSettingsRepository appSettings, 
                                 IThemeWrapper themeWrapper, 
                                 UpdateManager updateManager,
                                 Serilog.ILogger logger)
    {
        _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
        _themeWrapper = themeWrapper ?? throw new ArgumentNullException(nameof(themeWrapper));
        _updateManager = updateManager ?? throw new ArgumentNullException(nameof(updateManager));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        ColourThemeDark = _appSettings.ApplicationSettings.DarkMode;
        ColourThemeLight = !_appSettings.ApplicationSettings.DarkMode;
        _themeWrapper.ModifyTheme(ColourThemeDark);

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

        CurrentVersion = AssemblyVersion()?.ToString();
    }

    private static Version? AssemblyVersion()
    {
        Assembly assembly = typeof(App).Assembly;
        AssemblyName assemblyName = assembly.GetName();
        return assemblyName.Version;
    }

    private static int DotNetDesktopVersion()
    {
        var p = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = "--list-runtimes",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        p.Start();
        string output = p.StandardOutput.ReadToEnd();
        p.WaitForExit();

        var dotNetDesktop = output.Split(Environment.NewLine)
            .Select(line => line.Trim())
            .OrderBy(line => line)  // 10.0.0 comes before 8.0.0, so order alphabetically to ensure correct version is picked 
            .FirstOrDefault(line => line.StartsWith("Microsoft.WindowsDesktop.App"));

        var dotNetDesktopVersion = dotNetDesktop?.Split(' ')[1].Split('.')[0]; // Get the major version number

        return int.TryParse(dotNetDesktopVersion, out int majorVersion) ? majorVersion : 8;
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
        _logger.Debug("CheckForUpdate Started");

        if (_updateManager is null)
            return;
            
        if (!_updateManager.IsInstalled)
        {
            NewVersion = $" Version 9.9.999 available";
            UpdateState = ApplicationUpdateState.UpdateAvailable;
            return;
        }

        _updateInfo = await _updateManager.CheckForUpdatesAsync().ConfigureAwait(true);
        _logger.Debug("UpdateCommand CheckForUpdatesAsync() called");

        if (_updateInfo is null)
        {
            UpdateState = ApplicationUpdateState.NoUpdateAvailable;
            return;
        }

        NewVersion = $" Version {_updateInfo.TargetFullRelease.Version} available" ?? "No updates outstanding";
        UpdateState = ApplicationUpdateState.UpdateAvailable;
    }

    private readonly int _desktopVersion = DotNetDesktopVersion();
    private const int MinimumDesktopVersion = 10;

    [ObservableProperty]
    private Visibility _dotNetDesktopVersionWarning = Visibility.Collapsed;

    [ObservableProperty]
    private string? _newVersion = "No updates outstanding";

    private bool CanUpdate() => UpdateState == ApplicationUpdateState.UpdateAvailable && _desktopVersion == MinimumDesktopVersion;

    [RelayCommand(CanExecute = nameof(CanUpdate))]
    private async Task UpdateAndRestart()
    {
        if (_updateManager is null || _updateInfo is null)
            return;

        _logger.Debug("UpdateAndRestart Started");
        
        await _updateManager.DownloadUpdatesAsync(_updateInfo).ConfigureAwait(true);
        _logger.Debug("DownloadUpdate DownloadUpdatesAsync() called");
        UpdateState = ApplicationUpdateState.UpdateDownloaded;

        _updateManager.ApplyUpdatesAndRestart(_updateInfo);
        _logger.Debug("UpdateAndRestart Completed");
    }
    #endregion

    public override async Task LoadAsync()
    {
        if (_desktopVersion < MinimumDesktopVersion)
            DotNetDesktopVersionWarning = Visibility.Visible;

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
