using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Data;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Microsoft.Win32;

using NPOI.SS.UserModel;

using PhoneAssistant.WPF.Application.Entities;
using PhoneAssistant.WPF.Application.Repositories;

namespace PhoneAssistant.WPF.Features.BaseReport;

public partial class BaseReportMainViewModel : ObservableObject, IBaseReportMainViewModel
{
    private readonly IBaseReportRepository _repository;
    private readonly IImportHistoryRepository _import;
    private bool _loaded;

    private readonly ListCollectionView _filterView;

    public BaseReportMainViewModel(IBaseReportRepository repository, IImportHistoryRepository importHistory)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _import = importHistory ?? throw new ArgumentNullException(nameof(importHistory));
        _filterView = (ListCollectionView)CollectionViewSource.GetDefaultView(BaseReport);
        _filterView.Filter = new Predicate<object>(FilterView);
        ImportViewVisibility = Visibility.Collapsed;
        ReportViewVisibility = Visibility.Visible;
    }

    public ObservableCollection<string> LogItems { get; } = new();

    public ObservableCollection<Application.Entities.BaseReport> BaseReport { get; } = new();

    [RelayCommand]
    private async Task Refresh()
    {
        await LoadAsync();
        RefreshFilterView();
    }

    private void RefreshFilterView()
    {
        if (_filterView.IsEditingItem)
            _filterView.CommitEdit();
        _filterView.Refresh();
    }

    [ObservableProperty]
    private Visibility _importViewVisibility;

    [ObservableProperty]
    private Visibility _reportViewVisibility;

    [ObservableProperty]
    private string _LatestImport = string.Empty;

    #region Filter
    public bool FilterView(object item)
    {
        if (item is not Application.Entities.BaseReport vm) return false;

        if (FilterOutItem(FilterConnectedIMEI, vm.ConnectedIMEI)) return false;

        if (FilterOutItem(FilterContractEndDate, vm.ContractEndDate)) return false;

        if (FilterOutItem(FilterHandset, vm.Handset)) return false;

        if (FilterOutItem(FilterLastUsedIMEI, vm.LastUsedIMEI)) return false;

        if (FilterOutItem(FilterPhoneNumber, vm.PhoneNumber)) return false;

        if (FilterOutItem(FilterSimNumber, vm.SimNumber)) return false;

        if (FilterOutItem(FilterTalkPlan, vm.TalkPlan)) return false;

        if (FilterOutItem(FilterUserName, vm.UserName)) return false;

        return true;
    }

    public static bool FilterOutItem(string? filter, string item)
    {
        if (filter is not null && filter.Length > 0)
            if (item is null)
                return true;
            else if (!item.Contains(filter, StringComparison.InvariantCultureIgnoreCase))
                return true;

        return false;
    }

    [ObservableProperty]
    private string? _filterConnectedIMEI;
    partial void OnFilterConnectedIMEIChanged(string? value)
    {
        RefreshFilterView();
    }

    [ObservableProperty]
    private string? _filterContractEndDate;
    partial void OnFilterContractEndDateChanged(string? value)
    {
        RefreshFilterView();
    }

    [ObservableProperty]
    private string? _filterHandset;
    partial void OnFilterHandsetChanged(string? value)
    {
        RefreshFilterView();
    }

    [ObservableProperty]
    private string? _filterLastUsedIMEI;
    partial void OnFilterLastUsedIMEIChanged(string? value)
    {
        RefreshFilterView();
    }

    [ObservableProperty]
    private string? _filterPhoneNumber;
    partial void OnFilterPhoneNumberChanged(string? value)
    {
        RefreshFilterView();
    }

    [ObservableProperty]
    private string? _filterSimNumber;
    partial void OnFilterSimNumberChanged(string? value)
    {
        RefreshFilterView();
    }

    [ObservableProperty]
    private string? _filterTalkPlan;
    partial void OnFilterTalkPlanChanged(string? value)
    {
        RefreshFilterView();
    }

    [ObservableProperty]
    private string? _filterUserName;
    partial void OnFilterUserNameChanged(string? value)
    {
        RefreshFilterView();
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
            Filter = "Devon Base Report (*.xlsx)|*.xlsx",
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
        using IWorkbook workbook = WorkbookFactory.Create(stream, readOnly: true);
        //using HSSFWorkbook workbook = new HSSFWorkbook(stream);

        ISheet sheet = workbook.GetSheetAt(0);

        IRow header = sheet.GetRow(0);
        ICell cell = header.GetCell(0);
        if (cell is null || cell.StringCellValue != "Group")
        {
            LogItems.Add($"Unable to find Group in cell A1, check you are importing the correct file.");
            return;
        }

        await _repository.TruncateAsync();

        int added = 0;
        for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
        {
            IRow row = sheet.GetRow(i);
            if (row == null) continue;
            if (row.Cells.Count == 4) break;

            _ = row.GetCell(11).DateCellValue.ToString() ?? string.Empty;

            var PhoneNumber = row.GetCell(6).StringCellValue;
            var UserName = row.GetCell(5).StringCellValue;
            var ContractEndDate = row.GetCell(15).DateCellValue.ToString() ?? string.Empty;
            var TalkPlan = row.GetCell(8).StringCellValue.ToString();
            var Handset = row.GetCell(21).StringCellValue;
            var SimNumber = row.GetCell(17).StringCellValue;
            var ConnectedIMEI = string.Empty;
            var LastUsedIMEI = row.GetCell(18).StringCellValue; 

            Application.Entities.BaseReport item = new()
            {
                PhoneNumber = row.GetCell(6).StringCellValue,
                UserName = row.GetCell(5).StringCellValue,
                ContractEndDate = row.GetCell(15).DateCellValue.ToString() ?? string.Empty,
                TalkPlan = TalkPlan = row.GetCell(8).StringCellValue.ToString(),
                Handset = row.GetCell(21).StringCellValue,
                SimNumber = row.GetCell(17).StringCellValue,
                ConnectedIMEI = string.Empty,
                LastUsedIMEI = row.GetCell(18).StringCellValue
            };

            await _repository.CreateAsync(item);
            added++;
        }

        LogItems.Add($"Added {added} disposals");
        LogItems.Add("Import complete");

        ImportHistory importHistory = await _import.CreateAsync(ImportType.BaseReport, Path.GetFileName(DevonBaseReport!));

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

        ImportHistory? importHistory = await _import.GetLatestImportAsync(ImportType.BaseReport);
        if (importHistory is null)
            LatestImport = $"Latest Import: None";
        else
            LatestImport = $"Latest Import: {importHistory.File} ({importHistory.ImportDate})";

        IEnumerable<Application.Entities.BaseReport> report = await _repository.GetBaseReportAsync();

        foreach (Application.Entities.BaseReport phone in report)
        {
            BaseReport.Add(phone);
        }
        ;
    }
}