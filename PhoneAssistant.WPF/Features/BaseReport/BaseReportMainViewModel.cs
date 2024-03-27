using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Data;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Microsoft.Win32;

using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

using PhoneAssistant.WPF.Application.Entities;
using PhoneAssistant.WPF.Application.Repositories;

namespace PhoneAssistant.WPF.Features.BaseReport;

public partial class BaseReportMainViewModel : ObservableObject, IBaseReportMainViewModel
{
    private readonly BaseReportRepository _repository;
    private readonly ImportHistoryRepository _import;
    private bool _loaded;
    
    private readonly ICollectionView _filterView;

    public BaseReportMainViewModel(BaseReportRepository repository, ImportHistoryRepository importHistory)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _import = importHistory ?? throw new ArgumentNullException(nameof(importHistory));
        _filterView = CollectionViewSource.GetDefaultView(BaseReport);
        _filterView.Filter = new Predicate<object>(FilterView);
        ImportViewVisibility = Visibility.Collapsed;
        ReportViewVisibility = Visibility.Visible;
    }

    public ObservableCollection<string> LogItems { get; } = new();

    public ObservableCollection<EEBaseReport> BaseReport { get; } = new();

    [RelayCommand]
    private async Task Refresh()
    {
        await LoadAsync();
        _filterView.Refresh();
    }

    [ObservableProperty]
    private Visibility _importViewVisibility;
    
    [ObservableProperty]
    private Visibility _reportViewVisibility;

    [ObservableProperty]
    private string _LatestImport;


    #region Filter
    public bool FilterView(object item)
    {
        if (item is not EEBaseReport vm) return false;

        if (FilterOutItem(FilterConnectedIMEI, vm.ConnectedIMEI)) return false;

        if (FilterOutItem(FilterContractEndDate, vm.ContractEndDate)) return false;
        
        if (FilterOutItem(FilterHandset, vm.Handset)) return false;
        
        if (FilterOutItem(FilterLastUsedIMEI, vm.LastUsedIMEI)) return false;
        
        if (FilterOutItem(FilterPhoneNumber, vm.PhoneNumber)) return false; 
        
        if (FilterOutItem(FilterSIMNumber, vm.SIMNumber)) return false;
        
        if (FilterOutItem(FilterTalkPlan, vm.TalkPlan)) return false;

        if (FilterOutItem(FilterUserName, vm.UserName)) return false;

        return true;
    }

    public bool FilterOutItem(string? filter, string item)
    {
        if (filter is not null && filter.Length > 0)
            if (item is null)
                return true;
            else if (!item.Contains(filter, StringComparison.InvariantCultureIgnoreCase))
                return true;

        return false;
    }

    [ObservableProperty]
    private string? filterConnectedIMEI;
    partial void OnFilterConnectedIMEIChanged(string? value)
    {
        _filterView.Refresh();
    }

    [ObservableProperty]
    private string? filterContractEndDate;
    partial void OnFilterContractEndDateChanged(string? value)
    {
        _filterView.Refresh();
    }

    [ObservableProperty]
    private string? filterHandset;
    partial void OnFilterHandsetChanged(string? value)
    {
        _filterView.Refresh();
    }

    [ObservableProperty]
    private string? filterLastUsedIMEI;
    partial void OnFilterLastUsedIMEIChanged(string? value)
    {
        _filterView.Refresh();
    }

    [ObservableProperty]
    private string? filterPhoneNumber;
    partial void OnFilterPhoneNumberChanged(string? value)
    {
        _filterView.Refresh();
    }

    [ObservableProperty]
    private string? filterSIMNumber;
    partial void OnFilterSIMNumberChanged(string? value)
    {
        _filterView.Refresh();
    }

    [ObservableProperty]
    private string? filterTalkPlan;
    partial void OnFilterTalkPlanChanged(string? value)
    {
        _filterView.Refresh();
    }

    [ObservableProperty]
    private string? filterUserName;
    partial void OnFilterUserNameChanged(string? value)
    {
        _filterView.Refresh();
    }
    #endregion

    [RelayCommand]
    private void ShowImport()
    {
        ImportViewVisibility = Visibility.Visible;
        ReportViewVisibility = Visibility.Collapsed;
    }

    [RelayCommand]
    private void SelectBaseReportFile()
    {
        OpenFileDialog openFileDialog = new()
        {
            Filter = "Devon Base Report (*.xls)|*.xls",
            Multiselect = false
        };

        if (openFileDialog.ShowDialog() == true)
        {
            DevonBaseReport = openFileDialog.FileName;
        }
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ImportCommand))]
    private string? _devonBaseReport;

    private bool CanImport()
    {
        if (_loaded && string.IsNullOrWhiteSpace(DevonBaseReport))
            return false;

        return true;
    } 

    [RelayCommand(CanExecute = nameof(CanImport))]
    private async Task Import()
    {
        _loaded = false;
        
        await Task.Delay(100);

        using FileStream? stream = new FileStream(DevonBaseReport!, FileMode.Open, FileAccess.Read);
        using HSSFWorkbook workbook = new HSSFWorkbook(stream);

        ISheet sheet = workbook.GetSheetAt(1);

        IRow header = sheet.GetRow(0);
        ICell cell = header.GetCell(0);
        if (cell is null || cell.StringCellValue != "Group Id")
        {
            LogItems.Add($"Unable to find Group Id in cell A1, check you are importing the correct file.");
            return;
        }

        await _repository.TruncateAsync();

        int added = 0;
        for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
        {
            IRow row = sheet.GetRow(i);
            if (row == null) continue;
            if (row.Cells.Count == 4) break;

            EEBaseReport item = new()
            {
                PhoneNumber = row.GetCell(3).StringCellValue,
                UserName = row.GetCell(4).StringCellValue,
                ContractEndDate = row.GetCell(7).DateCellValue.ToShortDateString(),
                TalkPlan = row.GetCell(8).StringCellValue,
                Handset = row.GetCell(9).StringCellValue,
                SIMNumber = row.GetCell(10).StringCellValue,
                ConnectedIMEI = row.GetCell(11).StringCellValue,
                LastUsedIMEI = row.GetCell(12).StringCellValue
            };

            await _repository.CreateAsync(item);
            added++;
        }

        LogItems.Add($"Added {added} disposals");
        LogItems.Add("Import complete");

        ImportHistory importHistory = await _import.CreateAsync(Path.GetFileName(DevonBaseReport!));
        
        LatestImport = $"Latest Import: {importHistory.File} ({importHistory.ImportDate})";

        DevonBaseReport = string.Empty;

        BaseReport.Clear();
        await LoadAsync();
    }

    [RelayCommand]
    private void CloseImport()
    { 
        ImportViewVisibility = Visibility.Collapsed;
        ReportViewVisibility = Visibility.Visible;
    }    

    public async Task LoadAsync()
    {
        if (_loaded) return;

        _loaded = true;

        ImportHistory? importHistory = _import.GetLatestImport();
        if (importHistory is null)
            LatestImport = $"Latest Import: None";
        else
            LatestImport = $"Latest Import: {importHistory.File} ({importHistory.ImportDate})"; 

        IEnumerable<EEBaseReport> report = await _repository.GetBaseReportAsync();

        foreach (EEBaseReport phone in report)
        {
            BaseReport.Add(phone);
        };
    }
}