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
    private readonly IMessenger _messenger;
    private readonly ILogger<DisposalsMainViewModel> _logger;

    public ObservableCollection<string> LogItems { get; } = new();

    public DisposalsMainViewModel(DisposalsRepository disposalsRepository,
                                  IMessenger messenger,
                                  ILogger<DisposalsMainViewModel> logger)
    {
        messenger.RegisterAll(this);
        _disposalsRepository = disposalsRepository;
        _messenger = messenger;
        _logger = logger;
#if DEBUG
        ImportmyScomis = @"C:\Users\Jonathan.Linstead\OneDrive - Devon County Council\Phones\Disposals\CI List2024_17_1_13_42_55.xlsx";
#endif
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ExecuteMyScomisImportCommand))]
    private string? _importmyScomis;

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
            ImportmyScomis = openFileDialog.FileName;
        }
        _logger.LogInformation(ImportmyScomis);        
    }

    [ObservableProperty]
    //[NotifyCanExecuteChangedFor(nameof(ExecuteSCCImportCommand))]
    private string? _importSCC;

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
            ImportSCC = openFileDialog.FileName;
        }
    }

    [RelayCommand(CanExecute = nameof(CanImportMyScomis))]
    private void ExecuteMyScomisImport()
    {
        ImportMyScomis import = new(ImportmyScomis!, 
                                    _disposalsRepository, 
                                    _messenger);
        import.Execute();
        
    }
    private bool CanImportMyScomis() => ImportmyScomis is not null;

    [ObservableProperty]
    private string? _log;

    public Task LoadAsync()
    {
        return Task.CompletedTask;
    }

    public void Receive(LogMessage message)
    {
        LogItems.Add($"{DateTime.Now}: {message.text}");
        _logger.LogInformation(message.text);        
    }
}