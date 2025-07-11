using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using NPOI.SS.UserModel;
using PhoneAssistant.Model;
using System.IO;
using System.Windows;
using System.Windows.Data;

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

    public ObservableCollection<Model.BaseReport> BaseReport { get; } = new();

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
        if (item is not Model.BaseReport vm) return false;

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

    public async Task LoadAsync()
    {
        if (_loaded) return;

        _loaded = true;

        ImportHistory? importHistory = await _import.GetLatestImportAsync(ImportType.BaseReport);
        LatestImport = importHistory is null ? $"Latest Import: None" : $"Latest Import: {importHistory.File} ({importHistory.ImportDate})";

        IEnumerable<Model.BaseReport> report = await _repository.GetBaseReportAsync();

        foreach (Model.BaseReport phone in report)
        {
            BaseReport.Add(phone);
        }
        ;
    }
}