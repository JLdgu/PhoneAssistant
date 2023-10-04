using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

using CommunityToolkit.Mvvm.ComponentModel;

using PhoneAssistant.WPF.Application.Entities;

namespace PhoneAssistant.WPF.Features.Sims;

public sealed partial class SimsMainViewModel : ObservableObject, ISimsMainViewModel
{
    private Func<v1Sim, SimsItemViewModel> _simsItemViewModelFactory { get; }

    private readonly ISimsRepository _simRepository;

    public ObservableCollection<v1Sim> Sims { get; } = new();

    public ObservableCollection<SimsItemViewModel> SimItems { get; } = new();

    ICollectionView _filterView;

    public SimsMainViewModel(Func<v1Sim, SimsItemViewModel> simsItemViewModelFactory, ISimsRepository simRepository)
    {
        _simsItemViewModelFactory = simsItemViewModelFactory ?? throw new ArgumentNullException(nameof(simsItemViewModelFactory));
        _simRepository = simRepository ?? throw new ArgumentNullException(nameof(simRepository));

        _filterView = CollectionViewSource.GetDefaultView(Sims);
        _filterView.Filter = new Predicate<object>(FilterView);

    }
    #region Filter View
    [ObservableProperty]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "CommunityToolkit.Mvvm")]
    private string filterPhoneNumber;

    partial void OnFilterPhoneNumberChanged(string value)
    {
        _filterView.Refresh();
    }

    [ObservableProperty]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "CommunityToolkit.Mvvm")]
    private string filterSimNumber;

    partial void OnFilterSimNumberChanged(string value)
    {
        _filterView.Refresh();
    }

    [ObservableProperty]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "CommunityToolkit.Mvvm")]
    private string filterStatus;

    partial void OnFilterStatusChanged(string value)
    {
        _filterView.Refresh();
    }

    [ObservableProperty]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "CommunityToolkit.Mvvm")]
    private string filterAssetTag;

    partial void OnFilterAssetTagChanged(string value)
    {
        _filterView.Refresh();
    }

    [ObservableProperty]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "CommunityToolkit.Mvvm")]
    private string filterNotes;

    partial void OnFilterNotesChanged(string value)
    {
        _filterView.Refresh();
    }

    [ObservableProperty]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "CommunityToolkit.Mvvm")]
    private string filterLastUpdate;

    partial void OnFilterLastUpdateChanged(string value)
    {
        _filterView.Refresh();
    }

    private bool FilterView(object item)
    {
        if (item is not v1Sim Sim) return false;

        if (FilterPhoneNumber is not null && FilterPhoneNumber.Length > 0)
            if (Sim.PhoneNumber is null)
                return false;
            else if (!Sim.PhoneNumber.Contains(FilterPhoneNumber))
                return false;

        if (FilterSimNumber is not null && FilterSimNumber.Length > 0)
            if (Sim.SimNumber is null)
                return false;
            else if (!Sim.SimNumber.Contains(FilterSimNumber))
                return false;

        if (FilterStatus is not null && FilterStatus.Length > 0)
            if (Sim.Status is not null && !Sim.Status.Contains(FilterStatus, StringComparison.InvariantCultureIgnoreCase))
                return false;

        if (FilterAssetTag is not null && FilterAssetTag.Length > 0)
            if (Sim.AssetTag is null)
                return false;
            else if (!Sim.AssetTag.Contains(FilterAssetTag, StringComparison.InvariantCultureIgnoreCase))
                return false;

        if (FilterNotes is not null && FilterNotes.Length > 0)
            if (Sim.Notes is null)
                return false;
            else if (!Sim.Notes.Contains(FilterNotes, StringComparison.InvariantCultureIgnoreCase))
                return false;

        if (FilterLastUpdate is not null && FilterLastUpdate.Length > 0)
            if (Sim.LastUpdate is null)
                return false;
            else if (!Sim.LastUpdate.Contains(FilterLastUpdate, StringComparison.InvariantCultureIgnoreCase))
                return false;

        return true;
    }
    #endregion

    [ObservableProperty]
    private SimsItemViewModel? _selectedSim;

    public async Task LoadAsync()
    {
        if (Sims.Any())
            return;

        IEnumerable<v1Sim> simCards = await _simRepository.GetSimsAsync();
        if (simCards == null)
        {
            throw new ArgumentNullException(nameof(simCards));
        }

        foreach (v1Sim simcard in simCards)
        {
            Sims.Add(simcard);
            SimItems.Add(_simsItemViewModelFactory(simcard));            
        }
    }

    public Task WindowClosingAsync()
    {
        // TODO : Check outstanding edits have been saved
        return Task.CompletedTask;

    }
}
