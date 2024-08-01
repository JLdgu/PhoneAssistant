using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Data;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using Microsoft.Win32;

using PhoneAssistant.WPF.Application.Repositories;

namespace PhoneAssistant.WPF.Features.Disposals;
public partial class DisposalsMainViewModel : ObservableObject, IRecipient<LogMessage>, IDisposalsMainViewModel
{
    private readonly IDisposalsRepository _disposalsRepository;
    private readonly IPhonesRepository _phonesRepository;
    private readonly IMessenger _messenger;

    public ObservableCollection<string> LogItems { get; } = new();

    public DisposalsMainViewModel(IDisposalsRepository disposalsRepository,
                                  IPhonesRepository phonesRepository,
                                  IMessenger messenger)
    {
        _disposalsRepository = disposalsRepository ?? throw new ArgumentNullException(nameof(disposalsRepository));
        _phonesRepository = phonesRepository ?? throw new ArgumentNullException(nameof(phonesRepository));
        _messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));

        messenger.RegisterAll(this);

        BindingOperations.EnableCollectionSynchronization(LogItems, new object());
    }

    [ObservableProperty]
    private bool _importingFiles;

    #region myScomis Import    
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ReconcileCommand))]
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
    #endregion

    #region SCC Import
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ReconcileCommand))]
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
    #endregion

    #region PhoneAssistant import and reconiliation
    private bool CanReconcile() => !ImportingFiles;
    [RelayCommand(CanExecute = nameof(CanReconcile))]
    private async Task Reconcile()
    {
        Importing(true);
        ShowProgress = Visibility.Visible;

        await _disposalsRepository.TruncateAsync();

        LatestReconiliation = "Importing from myScomis spreadsheet";
        ImportMyScomis ms = new(ScomisFile!,
            _disposalsRepository,
            _messenger);
        await ms.Execute();
        ScomisFile = null;

        LatestReconiliation = "Importing from SCC spreadsheet";
        bool validSCC = true;
        try
        {

            ImportSCC scc = new(SCCFile!,
                _disposalsRepository,
                _messenger);
            await scc.Execute();
        }
        catch (IOException ex)
        {
            if (ex.Message.StartsWith("Duplicate"))
            {
                Receive(new LogMessage(MessageType.Default, $"Cannot read SCC spreadsheet"));
                Receive(new LogMessage(MessageType.Default, $"Try opening and saving a copy"));
                validSCC = false;
            }
            else
                throw;
        }
        SCCFile = null;
        if (validSCC)
        {
            LatestReconiliation = "Importing from Phones";
            ImportPhoneAssistant import = new(_disposalsRepository, _phonesRepository, _messenger);
            await import.Execute();

            LatestReconiliation = "Reconciling imports";
            Reconciliation reconcile = new(_disposalsRepository, _phonesRepository, _messenger);                
            await reconcile.Execute();

            LatestReconiliation = "Reconciliation complete";
        }

        ShowProgress = Visibility.Collapsed;
        Importing(false);
    }

    [ObservableProperty]
    private string? _latestReconiliation;

    [ObservableProperty]
    private Visibility _showProgress = Visibility.Collapsed;

    [ObservableProperty]
    private int _maxProgress = 100;

    [ObservableProperty]
    private int _progress = 0;
    #endregion

    [ObservableProperty]
    private string? _log;
    public async Task LoadAsync()
    {
        await Task.CompletedTask;
    }

    private void Importing(bool running)
    {
        ImportingFiles = running;
        SelectMSFileCommand.NotifyCanExecuteChanged();
        SelectSCCFileCommand.NotifyCanExecuteChanged();
        ReconcileCommand.NotifyCanExecuteChanged();
    }

    public void Receive(LogMessage message)
    {
        switch (message.Type)
        {
            case MessageType.Default:
                LogItems.Add($"{DateTime.Now}: {message.Text}");
                break;
            case MessageType.MaxProgress:
                MaxProgress = message.Progress;
                LogItems.Add($"{DateTime.Now}: Processing {message.Progress} rows");
                break;
            case MessageType.Progress:
                Progress = message.Progress;
                break;
        }
    }
}