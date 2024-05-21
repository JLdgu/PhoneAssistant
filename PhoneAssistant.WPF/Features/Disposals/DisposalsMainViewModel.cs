using System.Collections.ObjectModel;
using System.IO;

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
    private string? _latestMSImport;

    private bool CanImportMyScomis() => !string.IsNullOrEmpty(ScomisFile)  && !ImportingFiles;
    [RelayCommand(CanExecute = nameof(CanImportMyScomis))]
    private async Task ExecuteMyScomisImport()
    {
        ImportingFiles = true;

        ImportMyScomis import = new(ScomisFile!,
                                    _disposalsRepository,
                                    _messenger);
        await import.Execute();

        ImportHistory importHistory = await _importHistory.CreateAsync(ImportType.DisposalMS, Path.GetFileName(ScomisFile!));
        LatestMSImport = $"Latest Import: {importHistory.File} ({importHistory.ImportDate})";
        ScomisFile = null;

        ImportingFiles = false;
    }

    [ObservableProperty]
    private string? _latestPAImport;

    private bool CanImportPA() => !ImportingFiles;
    [RelayCommand(CanExecute=nameof(CanImportPA))]
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
    private string? _latestSCCImport;

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

    [RelayCommand]
    private async Task Reconcile()
    {
        IEnumerable<Disposal> disposals = await _disposalsRepository.GetAllDisposalsAsync();

        string? lastAction;
        foreach(Disposal disposal in disposals)
        {
            lastAction = disposal.Action;
            Reconciliation.Execute(disposal);
            if (disposal.Action != lastAction)
                await _disposalsRepository.UpdateAsync(disposal);
        }
    }

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
        LogItems.Add($"{DateTime.Now}: {message.Text}");
    }
}