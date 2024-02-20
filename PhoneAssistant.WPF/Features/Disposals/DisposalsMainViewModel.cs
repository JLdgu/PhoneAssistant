using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using Microsoft.Extensions.Logging;
using Microsoft.Win32;

using PhoneAssistant.WPF.Application.Repositories;
using PhoneAssistant.WPF.Features.Phones;

namespace PhoneAssistant.WPF.Features.Disposals;
public partial class DisposalsMainViewModel : ObservableObject, IRecipient<LogMessage>, IDisposalsMainViewModel
{
    private readonly DisposalsRepository _disposalsRepository;
    private readonly IPhonesRepository _phonesRepository;
    private readonly IMessenger _messenger;
    private readonly ILogger<DisposalsMainViewModel> _logger;

    public ObservableCollection<string> LogItems { get; } = new();

    public DisposalsMainViewModel(DisposalsRepository disposalsRepository,
                                  IPhonesRepository phonesRepository,
                                  IMessenger messenger,
                                  ILogger<DisposalsMainViewModel> logger)
    {
        _disposalsRepository = disposalsRepository ?? throw new ArgumentNullException(nameof(disposalsRepository));
        _phonesRepository = phonesRepository ?? throw new ArgumentNullException(nameof(phonesRepository));
        _messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        messenger.RegisterAll(this);
    }

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
    private bool _importingFiles;

    private bool CanImportMyScomis() => ScomisFile is not null && !ImportingFiles;
    [RelayCommand(CanExecute = nameof(CanImportMyScomis))]
    private async Task ExecuteMyScomisImport()
    {
        ImportingFiles = true;

        ImportMyScomis import = new(ScomisFile!,
                                    _disposalsRepository,
                                    _messenger);
        await import.Execute();

        ScomisFile = string.Empty;
        ImportingFiles = false;
    }

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

    private bool CanImportSCC() => SCCFile is not null && !ImportingFiles;
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

    public Task LoadAsync()
    {
        return Task.CompletedTask;
    }

    public void Receive(LogMessage message)
    {
        LogItems.Add($"{DateTime.Now}: {message.Text}");
        _logger.LogInformation(message.Text);
    }
}