using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Data;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

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

        BindingOperations.EnableCollectionSynchronization(LogItems, new object());
    }

    [ObservableProperty]
    private bool _importingFiles;

    #region myScomis Import    
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ExecuteMyScomisImportCommand))]
    private string? _scomisFile;

    private bool CanSelectMSFile() => !ImportingFiles;
    [RelayCommand(CanExecute = nameof(CanSelectMSFile))]
    private void SelectMSFile()
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
        Importing(true);

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

        Importing(false);
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

    private bool CanSelectSCCFile() => !ImportingFiles;
    [RelayCommand(CanExecute = nameof(CanSelectSCCFile))]
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
        Importing(true);

        ShowSCCLatestImport = Visibility.Collapsed;
        ShowSCCProgress = Visibility.Visible;

        ImportSCC import = new(SCCFile!,
                          _disposalsRepository,
                          _messenger);
        await import.Execute();

        ImportHistory importHistory = await _importHistory.CreateAsync(ImportType.DisposalSCC, Path.GetFileName(SCCFile!));
        LatestSCCImport = $"Latest Import: {importHistory.File} ({importHistory.ImportDate})";
        SCCFile = null;

        ShowSCCProgress = Visibility.Collapsed;
        ShowSCCLatestImport = Visibility.Visible;
        
        Importing(false);
    }

    [ObservableProperty]
    private Visibility _showSCCProgress = Visibility.Collapsed;

    [ObservableProperty]
    private int _sCCMaxProgress = 100;

    [ObservableProperty]
    private int _sCCProgress = 0;
    #endregion

    #region PhoneAssistant import and reconiliation
    [ObservableProperty]
    private string? _latestPAImport;

    [ObservableProperty]
    private Visibility _showPALatestImport = Visibility.Visible;

    private bool CanImportPA() => !ImportingFiles;
    [RelayCommand(CanExecute = nameof(CanImportPA))]
    private async Task ExecutePAImport()
    {
        Importing(true);

        ShowPALatestImport = Visibility.Collapsed;
        ShowPAProgress = Visibility.Visible;

        ImportPhoneAssistant import = new(_disposalsRepository,
                                          _phonesRepository,
                                          _messenger);
        await import.Execute();

        ImportHistory importHistory = await _importHistory.CreateAsync(ImportType.DisposalPA, "PhoneAssistant Database");
        LatestPAImport = $"Latest Import: {importHistory.File} ({importHistory.ImportDate})";

        ShowPAProgress = Visibility.Collapsed;
        ShowPALatestImport = Visibility.Visible;

        Importing(false);
    }

    [ObservableProperty]
    private Visibility _showPAProgress = Visibility.Collapsed;

    [ObservableProperty]
    private int _pAMaxProgress = 100;

    [ObservableProperty]
    private int _pAProgress = 0;

    private bool CanReconcile() => !ImportingFiles;
    [RelayCommand(CanExecute = nameof(CanReconcile))]
    private async Task Reconcile()
    {
        Importing(true);

        ShowReconiliation = Visibility.Collapsed;
        ShowReconiliationProgress = Visibility.Visible;

        Reconciliation reconcile = new(_disposalsRepository,
                                        _messenger);
        await reconcile.Execute();

        ImportHistory importHistory = await _importHistory.CreateAsync(ImportType.Reconiliation, "None");
        LatestReconiliation = $"Latest Reconiliation: {importHistory.ImportDate}";

        ShowReconiliationProgress = Visibility.Collapsed;
        ShowReconiliation = Visibility.Visible;

        Importing(false);
    }

    [ObservableProperty]
    private string? _latestReconiliation;

    [ObservableProperty]
    private Visibility _showReconiliation = Visibility.Visible;

    [ObservableProperty]
    private Visibility _showReconiliationProgress = Visibility.Collapsed;

    [ObservableProperty]
    private int _reconiliationMaxProgress = 100;

    [ObservableProperty]
    private int _reconiliationProgress = 0;
    #endregion

    [ObservableProperty]
    private string? _log;
    public async Task LoadAsync()
    {
        ImportHistory? importHistory = await _importHistory.GetLatestImportAsync(ImportType.DisposalMS);
        LatestMSImport = importHistory is null ? $"Latest Import: None" : $"Latest Import: {importHistory.File} ({importHistory.ImportDate})";

        importHistory = await _importHistory.GetLatestImportAsync(ImportType.DisposalPA);
        LatestPAImport = importHistory is null ? $"Latest Import: None" : $"Latest Import: {importHistory.File} ({importHistory.ImportDate})";

        importHistory = await _importHistory.GetLatestImportAsync(ImportType.DisposalSCC);
        LatestSCCImport = importHistory is null ? $"Latest Import: None" : $"Latest Import: {importHistory.File} ({importHistory.ImportDate})";

        importHistory = await _importHistory.GetLatestImportAsync(ImportType.Reconiliation);
        LatestReconiliation = importHistory is null ? $"Latest Reconiliation: None" : $"Latest Reconiliation: {importHistory.ImportDate}";
    }

    private void Importing(bool running)
    {
        ImportingFiles = running;
        SelectMSFileCommand.NotifyCanExecuteChanged();
        SelectSCCFileCommand.NotifyCanExecuteChanged();
        ExecuteSCCImportCommand.NotifyCanExecuteChanged();
        ExecutePAImportCommand.NotifyCanExecuteChanged();
        ReconcileCommand.NotifyCanExecuteChanged();
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
            case MessageType.ReconciliationMaxProgress:
                ReconiliationMaxProgress = message.Progress;
                LogItems.Add($"{DateTime.Now}: Reconciling {message.Progress} rows");
                break;
            case MessageType.ReconciliationProgress:
                ReconiliationProgress = message.Progress;
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