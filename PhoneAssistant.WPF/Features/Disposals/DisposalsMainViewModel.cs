using System.Collections.ObjectModel;

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
    private readonly DisposalsRepository _disposalsRepository;
    private readonly ImportHistoryRepository _importHistory;
    private readonly IPhonesRepository _phonesRepository;
    private readonly IMessenger _messenger;
    private readonly ILogger<DisposalsMainViewModel> _logger;

    public ObservableCollection<string> LogItems { get; } = new();

    public DisposalsMainViewModel(DisposalsRepository disposalsRepository,
                                  ImportHistoryRepository importHistory,
                                  IPhonesRepository phonesRepository,
                                  IMessenger messenger,
                                  ILogger<DisposalsMainViewModel> logger)
    {
        _disposalsRepository = disposalsRepository ?? throw new ArgumentNullException(nameof(disposalsRepository));
        _importHistory = importHistory ?? throw new ArgumentNullException(nameof(importHistory));
        _phonesRepository = phonesRepository ?? throw new ArgumentNullException(nameof(phonesRepository));
        _messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

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
        _logger.LogInformation(ScomisFile);
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

        ImportHistory importHistory = await _importHistory.CreateAsync(ImportType.DisposalMS, ScomisFile!);

        LatestMSImport = $"Latest Import: {importHistory.File} ({importHistory.ImportDate})";

        ScomisFile = string.Empty;
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

        SCCFile = string.Empty;
        ImportingFiles = false;
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
    }

    public void Receive(LogMessage message)
    {
        LogItems.Add($"{DateTime.Now}: {message.Text}");
        _logger.LogInformation(message.Text);
    }
}