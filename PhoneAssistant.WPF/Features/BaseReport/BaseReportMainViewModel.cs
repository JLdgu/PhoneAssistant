using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

using CommunityToolkit.Mvvm.ComponentModel;

using Microsoft.EntityFrameworkCore;

using PhoneAssistant.WPF.Application;
using PhoneAssistant.WPF.Application.Entities;
using PhoneAssistant.WPF.Features.Phones;

namespace PhoneAssistant.WPF.Features.BaseReport;

public partial class BaseReportMainViewModel : ObservableObject, IBaseReportMainViewModel
{
    private readonly PhoneAssistantDbContext _dbContext;

    public ObservableCollection<EEBaseReport> BaseReport { get; } = new();
    
    private readonly ICollectionView _filterView;

    public BaseReportMainViewModel(PhoneAssistantDbContext dbContext)
    {
        _dbContext = dbContext;
        _filterView = CollectionViewSource.GetDefaultView(BaseReport);
        _filterView.Filter = new Predicate<object>(FilterView);
    }

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


    public async Task LoadAsync()
    {
        IEnumerable<EEBaseReport> phones = await _dbContext
                .BaseReport.
                AsNoTracking()
                .ToListAsync();

        foreach (EEBaseReport phone in phones)
        {
            BaseReport.Add(phone);
        };
    }
}