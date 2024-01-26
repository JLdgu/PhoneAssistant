using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Microsoft.Extensions.Logging;
using Microsoft.Win32;

namespace PhoneAssistant.WPF.Features.Disposals;
public partial class DisposalsMainViewModel : ObservableObject, IDisposalsMainViewModel
{
    private readonly ILogger<DisposalsMainViewModel> _logger;

    public DisposalsMainViewModel(ILogger<DisposalsMainViewModel> logger)
    {
        _logger = logger;
    }

    [ObservableProperty]
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

    [ObservableProperty]
    private string? _log;

    public Task LoadAsync()
    {
        return Task.CompletedTask;
    }

}