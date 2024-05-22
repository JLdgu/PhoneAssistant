using System.Collections.ObjectModel;
using System.IO;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using Microsoft.Extensions.Logging;
using Microsoft.Win32;

using PhoneAssistant.WPF.Application.Entities;
using PhoneAssistant.WPF.Application.Repositories;

namespace PhoneAssistant.WPF.Features.Disposals;
public partial class DisposalsMainViewModel : ObservableObject, IRecipient<LogMessage>, IDisposalsMainViewModel
{
    private readonly IDisposalsRepository _disposalsRepository;
    private readonly IImportHistoryRepository _importHistory;
    private readonly IPhonesRepository _phonesRepository;
    private readonly IMessenger _messenger;

    public ObservableCollection<string> LogItems { get; } = new();

    public DisposalsMainViewModel(IDisposalsRepository disposalsRepository,
                                  IImportHistoryRepository importHistory,
                                  IPhonesRepository phonesRepository,
                                  IMessenger messenger)
    {
        _disposalsRepository = disposalsRepository ?? throw new ArgumentNullException(nameof(disposalsRepository));
        _importHistory = importHistory ?? throw new ArgumentNullException(nameof(importHistory));
        _phonesRepository = phonesRepository ?? throw new ArgumentNullException(nameof(phonesRepository));
        _messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));

        messenger.RegisterAll(this);
    }

    [ObservableProperty]
    private bool _importingFiles;

    #region myScomis Import    
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ExecuteMyScomisImportCommand))]
    private string? _scomisFile;

    [RelayCommand]
    private void SelectMyScomisFile()
    {
        OpenFileDialog openFileDialog = new()
        {
            Filter = "Excel Workbook (*.xlsx)|*.xlsx",
            Multiselect = false
        };

        if (openFileDialog.ShowDialog() == true)
        {
            ScomisFile = openFileDialog.FileName;
        }
    }

    [ObservableProperty]
    private string? _latestMSImport;

    [ObservableProperty]
    private Visibility _showMSLatestImport = Visibility.Visible;

    private bool CanImportMyScomis() => !string.IsNullOrEmpty(ScomisFile) && !ImportingFiles;
    [RelayCommand(CanExecute = nameof(CanImportMyScomis))]
    private async Task ExecuteMyScomisImport()
    {
        ImportingFiles = true;
        ShowMSLatestImport = Visibility.Collapsed;
        ShowMSProgress = Visibility.Visible;

        ImportMyScomis import = new(ScomisFile!,
                                    _disposalsRepository,
                                    _messenger);
        await import.Execute();

        ImportHistory importHistory = await _importHistory.CreateAsync(ImportType.DisposalMS, Path.GetFileName(ScomisFile!));
        LatestMSImport = $"Latest Import: {importHistory.File} ({importHistory.ImportDate})";
        ScomisFile = null;

        ShowMSProgress = Visibility.Collapsed;
        ShowMSLatestImport = Visibility.Visible;
        ImportingFiles = false;
    }

    [ObservableProperty]
    private Visibility _showMSProgress = Visibility.Collapsed;

    [ObservableProperty]
    private int _mSMaxProgress = 100;

    [ObservableProperty]
    private int _mSProgress = 0;
    #endregion

    #region SCC Import
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ExecuteSCCImportCommand))]
    private string? _sCCFile;

    [RelayCommand]
    private void SelectSCCFile()
    {
        OpenFileDialog openFileDialog = new()
        {
            Filter = "Excel 97-2003 Workbook (*.xls)|*.xls",
            Multiselect = false
        };

        if (openFileDialog.ShowDialog() == true)
        {
            SCCFile = openFileDialog.FileName;
        }
    }

    [ObservableProperty]
    private string? _latestSCCImport;

    [ObservableProperty]
    private Visibility _showSCCLatestImport = Visibility.Visible;

    private bool CanImportSCC() => !string.IsNullOrEmpty(SCCFile) && !ImportingFiles;
    [RelayCommand(CanExecute = nameof(CanImportSCC))]
    private async Task ExecuteSCCImport()
    {
        ImportingFiles = true;
        ImportSCC import = new(SCCFile!,
                              _disposalsRepository,
                              _messenger);
        await import.Execute();

        ImportHistory importHistory = await _importHistory.CreateAsync(ImportType.DisposalSCC, Path.GetFileName(SCCFile!));
        LatestSCCImport = $"Latest Import: {importHistory.File} ({importHistory.ImportDate})";
        SCCFile = null;

        ImportingFiles = false;
    }
    
    [ObservableProperty]
    private Visibility _showSCCProgress = Visibility.Collapsed;

    [ObservableProperty]
    private int _sCCMaxProgress = 100;

    [ObservableProperty]
    private int _sCCProgress = 0;
    #endregion

    #region PhoneAssistant import
    [ObservableProperty]
    private string? _latestPAImport;

    [ObservableProperty]
    private bool _showPALatestImport = true;

    private bool CanImportPA() => !ImportingFiles;
    [RelayCommand(CanExecute = nameof(CanImportPA))]
    private async Task ExecutePAImport()
    {
        ImportingFiles = true;
        ImportPhoneAssistant import = new(_disposalsRepository,
                                          _phonesRepository,
                                          _messenger);
        await import.Execute();

        ImportHistory importHistory = await _importHistory.CreateAsync(ImportType.DisposalPA, "PhoneAssistant Database");
        LatestPAImport = $"Latest Import: {importHistory.File} ({importHistory.ImportDate})";

        ImportingFiles = false;
    }

    [ObservableProperty]
    private bool _showPAProgress = false;

    [ObservableProperty]
    private int _pAMaxProgress = 100;

    [ObservableProperty]
    private int _pAProgress = 0;

    [RelayCommand]
    private async Task Reconcile()
    {
        IEnumerable<Disposal> disposals = await _disposalsRepository.GetAllDisposalsAsync();

        string? lastAction;
        foreach (Disposal disposal in disposals)
        {
            lastAction = disposal.Action;
            Reconciliation.Execute(disposal);
            if (disposal.Action != lastAction)
                await _disposalsRepository.UpdateAsync(disposal);
        }
    }

    [ObservableProperty]
    private bool _showReconcileProgress = false;

    [ObservableProperty]
    private int _reconcileMaxProgress = 100;

    [ObservableProperty]
    private int _reconcileProgress = 0;
    #endregion

    [ObservableProperty]
    private string? _log;

    public async Task LoadAsync()
    {
        ImportHistory? importHistory = await _importHistory.GetLatestImportAsync(ImportType.DisposalMS);
        if (importHistory is null)
            LatestMSImport = $"Latest Import: None";
        else
            LatestMSImport = $"Latest Import: {importHistory.File} ({importHistory.ImportDate})";

        importHistory = await _importHistory.GetLatestImportAsync(ImportType.DisposalPA);
        if (importHistory is null)
            LatestPAImport = $"Latest Import: None";
        else
            LatestPAImport = $"Latest Import: {importHistory.File} ({importHistory.ImportDate})";

        importHistory = await _importHistory.GetLatestImportAsync(ImportType.DisposalSCC);
        if (importHistory is null)
            LatestSCCImport = $"Latest Import: None";
        else
            LatestSCCImport = $"Latest Import: {importHistory.File} ({importHistory.ImportDate})";
    }

    public void Receive(LogMessage message)
    {
        switch (message.Type)
        {
            case MessageType.Default:
                LogItems.Add($"{DateTime.Now}: {message.Text}");
                break;
            case MessageType.MSMaxProgress:
                MSMaxProgress = message.Progress;
                LogItems.Add($"{DateTime.Now}: Processing {message.Progress} rows");
                break;
            case MessageType.MSProgress:
                MSProgress = message.Progress;
                break;
            case MessageType.PAMaxProgress:
                PAMaxProgress = message.Progress;
                LogItems.Add($"{DateTime.Now}: Processing {message.Progress} rows");
                break;
            case MessageType.PAProgress:
                PAProgress = message.Progress;
                break;
            case MessageType.SCCMaxProgress:
                SCCMaxProgress = message.Progress;
                LogItems.Add($"{DateTime.Now}: Processing {message.Progress} rows");
                break;
            case MessageType.SCCProgress:
                SCCProgress = message.Progress;
                break;
        }
    }
}