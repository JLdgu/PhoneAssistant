using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using PhoneAssistant.WPF.Application.Entities;
using PhoneAssistant.WPF.Application.Repositories;

namespace PhoneAssistant.WPF.Features.Sims;

public sealed partial class SimsMainViewModel : ObservableObject, ISimsMainViewModel
{
    private readonly ISimsItemViewModelFactory _simsItemViewModelFactory;
    private readonly ISimsRepository _simRepository;
    private readonly ICollectionView _filterView;

    public ObservableCollection<SimsItemViewModel> SimItems { get; } = new();

    public SimsMainViewModel(ISimsItemViewModelFactory simsItemViewModelFactory, ISimsRepository simRepository)
    {
        _simsItemViewModelFactory = simsItemViewModelFactory ?? throw new ArgumentNullException(nameof(simsItemViewModelFactory));
        _simRepository = simRepository ?? throw new ArgumentNullException(nameof(simRepository));

        _filterView = CollectionViewSource.GetDefaultView(SimItems);
        _filterView.Filter = new Predicate<object>(FilterView);

        CanRefeshSims = true;
    }

    [RelayCommand]
    private async Task RefreshSims()
    {
        CanRefeshSims = false;
        await LoadAsync();
        _filterView.Refresh();
        CanRefeshSims = true;
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "CommunityToolkit.Mvvm")]
    [ObservableProperty]
    private bool canRefeshSims;

    #region Filter View
    private bool FilterView(object item)
    {
        if (item is not SimsItemViewModel vm) return false;

        if (FilterPhoneNumber is not null && FilterPhoneNumber.Length > 0)
            if (vm.PhoneNumber is null)
                return false;
            else if (!vm.PhoneNumber.Contains(FilterPhoneNumber))
                return false;

        if (FilterSimNumber is not null && FilterSimNumber.Length > 0)
            if (vm.SimNumber is null)
                return false;
            else if (!vm.SimNumber.Contains(FilterSimNumber))
                return false;

        if (FilterStatus is not null && FilterStatus.Length > 0)
            if (vm.Status is null)
                return false;
            else if (!vm.Status.Contains(FilterStatus, StringComparison.InvariantCultureIgnoreCase))
                return false;

        if (FilterAssetTag is not null && FilterAssetTag.Length > 0)
            if (vm.AssetTag is null)
                return false;
            else if (!vm.AssetTag.Contains(FilterAssetTag, StringComparison.InvariantCultureIgnoreCase))
                return false;

        if (FilterNotes is not null && FilterNotes.Length > 0)
            if (vm.Notes is null)
                return false;
            else if (!vm.Notes.Contains(FilterNotes, StringComparison.InvariantCultureIgnoreCase))
                return false;

        if (FilterLastUpdate is not null && FilterLastUpdate.Length > 0)
            if (vm.LastUpdate is null)
                return false;
            else if (!vm.LastUpdate.Contains(FilterLastUpdate, StringComparison.InvariantCultureIgnoreCase))
                return false;

        return true;
    }

    [ObservableProperty]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "CommunityToolkit.Mvvm")]
    private string? filterPhoneNumber;

    partial void OnFilterPhoneNumberChanged(string? value)
    {
        _filterView.Refresh();
    }

    [ObservableProperty]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "CommunityToolkit.Mvvm")]
    private string? filterSimNumber;

    partial void OnFilterSimNumberChanged(string? value)
    {
        _filterView.Refresh();
    }

    [ObservableProperty]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "CommunityToolkit.Mvvm")]
    private string? filterStatus;

    partial void OnFilterStatusChanged(string? value)
    {
        _filterView.Refresh();
    }

    [ObservableProperty]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "CommunityToolkit.Mvvm")]
    private string? filterAssetTag;

    partial void OnFilterAssetTagChanged(string? value)
    {
        _filterView.Refresh();
    }

    [ObservableProperty]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "CommunityToolkit.Mvvm")]
    private string? filterNotes;

    partial void OnFilterNotesChanged(string? value)
    {
        _filterView.Refresh();
    }

    [ObservableProperty]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "CommunityToolkit.Mvvm")]
    private string? filterLastUpdate;

    partial void OnFilterLastUpdateChanged(string? value)
    {
        _filterView.Refresh();
    }
    #endregion

    [ObservableProperty]
    private SimsItemViewModel? _selectedSim;

    public async Task LoadAsync()
    {
        SimItems.Clear();

        IEnumerable<v1Sim> simCards = await _simRepository.GetSimsAsync();
        if (simCards == null)
        {
            throw new ArgumentNullException(nameof(simCards));
        }

        foreach (v1Sim simcard in simCards)
        {            
            SimItems.Add(_simsItemViewModelFactory.Create(simcard));
        }
    }
}
