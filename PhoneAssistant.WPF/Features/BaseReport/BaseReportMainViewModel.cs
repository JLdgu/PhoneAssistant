using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PhoneAssistant.Model;
using PhoneAssistant.WPF.Shared;
using System.Collections.ObjectModel;

namespace PhoneAssistant.WPF.Features.BaseReport;

public interface IBaseReportMainViewModel : IViewModel
{
}

public partial class BaseReportMainViewModel(IImportHistoryRepository importHistory, ISimRepository repository) : ViewModelBase, IBaseReportMainViewModel
{
    private readonly IImportHistoryRepository _import = importHistory ?? throw new ArgumentNullException(nameof(importHistory));
    private readonly ISimRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));

    public ObservableCollection<Sim> Sims { get; } = [];

    [ObservableProperty]
    public partial string LatestImport {  get; set; } = string.Empty;

    [ObservableProperty]
    public partial string PhoneNumber { get; set; } = string.Empty;

    [RelayCommand]
    private async Task EnterKey()
    {
        if (string.IsNullOrEmpty(PhoneNumber)) return;

        Sims.Clear();

        var sims = await _repository.GetSim(PhoneNumber);
        foreach (var sim in sims)
        { 
            Sims.Add(sim);
        }
        
    }

    public override async Task LoadAsync()
    {
        ImportHistory? importHistory = await _import.GetLatestImportAsync(ImportType.BaseReport);
        LatestImport = importHistory is null ? $"Latest Import: None" : $"Latest Import: {importHistory.Run} ({importHistory.ImportDate})";
    }
}