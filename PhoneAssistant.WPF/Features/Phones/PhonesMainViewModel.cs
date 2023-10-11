using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using PhoneAssistant.WPF.Application.Entities;

namespace PhoneAssistant.WPF.Features.Phones;

public sealed partial class PhonesMainViewModel : ObservableValidator, IPhonesMainViewModel
{
    private readonly IPhonesItemViewModelFactory _phonesItemViewModelFactory;
    private readonly IPhonesRepository _phonesRepository;

    public ObservableCollection<PhonesItemViewModel> PhoneItems { get; } = new();    

    private readonly ICollectionView _filterView;

    public PhonesMainViewModel(IPhonesItemViewModelFactory phonesItemViewModelFactory,
                               IPhonesRepository phonesRepository)
    {
        _phonesItemViewModelFactory = phonesItemViewModelFactory ?? throw new ArgumentNullException(nameof(phonesItemViewModelFactory));
        _phonesRepository = phonesRepository ?? throw new ArgumentNullException(nameof(phonesRepository));

        _filterView = CollectionViewSource.GetDefaultView(PhoneItems);
        _filterView.Filter = new Predicate<object>(FilterView);

        CanRefeshPhones = true;
    }

    [RelayCommand]
    private async Task RefreshPhones()
    {
        CanRefeshPhones = false;
        await LoadAsync();
        _filterView.Refresh();
        CanRefeshPhones = true;
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "CommunityToolkit.Mvvm")]
    [ObservableProperty]
    private bool canRefeshPhones;

    #region Filtering View
    [ObservableProperty]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "CommunityToolkit.Mvvm")]
    private string? filterNorR;

    partial void OnFilterNorRChanged(string? value)
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
    private string? filterSR;

    partial void OnFilterSRChanged(string? value)
    {
        _filterView.Refresh();
    }

    [ObservableProperty]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "CommunityToolkit.Mvvm")]
    private string? filterImei;

    partial void OnFilterImeiChanged(string? value)
    {
        _filterView.Refresh();
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
    private string? filterNewUser;

    partial void OnFilterNewUserChanged(string? value)
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
    private string? filterOEM;

    partial void OnFilterOEMChanged(string? value)
    {
        _filterView.Refresh();
    }

    [ObservableProperty]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "CommunityToolkit.Mvvm")]
    private string? filterModel;

    partial void OnFilterModelChanged(string? value)
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

    public bool FilterView(object item)
    {
        if (item is not PhonesItemViewModel vm) return false;

        //v1Phone phone = vm.Phone;

        if (FilterNorR is not null && FilterNorR.Length == 1)
            if (vm.NorR is not null && !vm.NorR.StartsWith(FilterNorR, StringComparison.InvariantCultureIgnoreCase))
                return false;

        if (FilterStatus is not null && FilterStatus.Length > 0)
            if (vm.Status is not null && !vm.Status.Contains(FilterStatus,StringComparison.InvariantCultureIgnoreCase))
                return false;

        if (FilterSR is not null && FilterSR.Length > 0)
            if (vm.SR is null)
                return false;
            else if (!vm.SR.ToString()!.Contains(FilterSR))
                return false;        

        if (FilterImei is not null && FilterImei.Length > 0)
            if (vm.Imei is not null && !vm.Imei.Contains(FilterImei))
                return false;

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

        if (FilterNewUser is not null && FilterNewUser.Length > 0)
            if (vm.NewUser is null)
                return false;
            else if (!vm.NewUser.Contains(FilterNewUser, StringComparison.InvariantCultureIgnoreCase))
                return false;

        if (FilterAssetTag is not null && FilterAssetTag.Length > 0)
            if (vm.AssetTag is null)
                return false;
            else if (!vm.AssetTag.Contains(FilterAssetTag, StringComparison.InvariantCultureIgnoreCase))
                return false;

        if (FilterOEM is not null && FilterOEM.Length > 0)
            if (vm.OEM is null)
                return false;
            else if (!vm.OEM.Contains(FilterOEM, StringComparison.InvariantCultureIgnoreCase))
                return false;

        if (FilterModel is not null && FilterModel.Length > 0)
            if (vm.Model is null)
                return false;
            else if (!vm.Model.Contains(FilterModel, StringComparison.InvariantCultureIgnoreCase))
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
    #endregion

    [ObservableProperty]
    private PhonesItemViewModel? _selectedPhone;

    public async Task LoadAsync()
    {
        PhoneItems.Clear();
        IEnumerable<v1Phone> phones = await _phonesRepository.GetPhonesAsync();

        foreach (v1Phone phone in phones) 
        {
            if (phone.NorR == "N")
                phone.NorR = "New";
            else
                phone.NorR = "Repurposed";
            PhoneItems.Add(_phonesItemViewModelFactory.Create(phone));
        }
    }

    public Task WindowClosingAsync()
    {
        throw new NotImplementedException();
    }
}
