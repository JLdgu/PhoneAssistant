using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using PhoneAssistant.WPF.Application.Entities;
using PhoneAssistant.WPF.Application.Repositories;

namespace PhoneAssistant.WPF.Features.Sims;

public sealed partial class SimsMainViewModel : ObservableObject, IRecipient<Sim>, ISimsMainViewModel
{
    private readonly ISimsItemViewModelFactory _simsItemViewModelFactory;
    private readonly ISimsRepository _simRepository;
    private readonly ListCollectionView _filterView;

    public ObservableCollection<SimsItemViewModel> SimItems { get; } = new();

    public SimsMainViewModel(ISimsItemViewModelFactory simsItemViewModelFactory, ISimsRepository simRepository, IMessenger messenger)
    {
        _simsItemViewModelFactory = simsItemViewModelFactory ?? throw new ArgumentNullException(nameof(simsItemViewModelFactory));
        _simRepository = simRepository ?? throw new ArgumentNullException(nameof(simRepository));

        _filterView = (ListCollectionView)CollectionViewSource.GetDefaultView(SimItems);
        _filterView.Filter = new Predicate<object>(FilterView);

        messenger.Register<Sim>(this);
    }

    [RelayCommand]
    private async Task RefreshSims()
    {
        CanRefeshSims = false;
        await LoadAsync();
    }

    private void RefreshFilterView()
    {
        if (_filterView.IsEditingItem)
            _filterView.CommitEdit();
        _filterView.Refresh();
    }

    [ObservableProperty]
    private bool _canRefeshSims;

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
    private string? _filterStatus;
    partial void OnFilterStatusChanged(string? value)
    {
        RefreshFilterView();
    }

    [ObservableProperty]
    private string? _filterAssetTag;
    partial void OnFilterAssetTagChanged(string? value)
    {
        RefreshFilterView();
    }

    [ObservableProperty]
    private string? _filterNotes;
    partial void OnFilterNotesChanged(string? value)
    {
        RefreshFilterView();
    }

    [ObservableProperty]
    private string? _filterLastUpdate;
    partial void OnFilterLastUpdateChanged(string? value)
    {
        RefreshFilterView();
    }
    #endregion

    [ObservableProperty]
    private SimsItemViewModel? _selectedSim;

    public async Task LoadAsync()
    {
        if (!CanRefeshSims)
        {
            SimItems.Clear();
            IEnumerable<Sim> simCards = await _simRepository.GetSimsAsync();
            if (simCards == null)
            {
                throw new ArgumentNullException(nameof(simCards));
            }

            foreach (Sim simcard in simCards)
            {
                SimItems.Add(_simsItemViewModelFactory.Create(simcard));
            }
        }

        _filterView.SortDescriptions.Clear();
        _filterView.SortDescriptions.Add(new SortDescription("LastUpdate", ListSortDirection.Descending));
        RefreshFilterView();

        CanRefeshSims = true;
    }

    public void Receive(Sim message)
    {
        SimItems.Add(_simsItemViewModelFactory.Create(message));
    }
}
