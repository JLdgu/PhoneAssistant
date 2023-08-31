using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Microsoft.Win32;

using PhoneAssistant.WPF.Application;

namespace PhoneAssistant.WPF.Features.Settings;

public sealed partial class SettingsMainViewModel : ObservableObject, ISettingsMainViewModel
{
    private readonly IUserSettings _userSettings;

#pragma warning disable CS8618
    public SettingsMainViewModel(IUserSettings userSettings)
    {
        _userSettings = userSettings ?? throw new ArgumentNullException(nameof(userSettings));
        Database = _userSettings.Database;
        PrintToFile = _userSettings.PrintToFile;
        PrintToPrinter = !PrintToFile;
        Printer = _userSettings.Printer;
        PrintFile = _userSettings.PrintFile;
        VersionDescription = _userSettings.AssemblyVersion?.ToString();
    }
#pragma warning restore CS8618

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
    [RelayCommand]
    private void SavePrinter()
    {
    }

    [ObservableProperty]
    private bool canSavePrinter;

    [ObservableProperty]
    private string _printFile;

    partial void OnPrintFileChanged(string value)
    {
        if (_userSettings.PrintFile == value) return;

        _userSettings.PrintFile = value;
        _userSettings.Save();

    }

    [RelayCommand]
    private void SavePrintFile()
    {
    }

    [ObservableProperty]
    private bool canSavePrintFile;

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
