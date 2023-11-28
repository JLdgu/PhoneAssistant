using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Documents;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using PhoneAssistant.WPF.Application.Entities;

namespace PhoneAssistant.WPF.Features.Phones;

public sealed partial class PhonesMainViewModel : ObservableValidator, IRecipient<Email>, IPhonesMainViewModel
{
    private readonly IPhonesItemViewModelFactory _phonesItemViewModelFactory;
    private readonly IPhonesRepository _phonesRepository;

    public ObservableCollection<PhonesItemViewModel> PhoneItems { get; } = new();    

    public EmailViewModel EmailViewModel { get; set; }

    private readonly ICollectionView _filterView;

    public List<string> NorRs { get; init; } = new();
    public List<string> OEMs { get; init; } = new();
    public List<string> Statuses { get; init; } = new();

    public PhonesMainViewModel(IPhonesItemViewModelFactory phonesItemViewModelFactory,
                               IPhonesRepository phonesRepository,
                               IMessenger messenger)
    {
        _phonesItemViewModelFactory = phonesItemViewModelFactory ?? throw new ArgumentNullException(nameof(phonesItemViewModelFactory));
        _phonesRepository = phonesRepository ?? throw new ArgumentNullException(nameof(phonesRepository));
        _filterView = CollectionViewSource.GetDefaultView(PhoneItems);
        _filterView.Filter = new Predicate<object>(FilterView);
               
        messenger.RegisterAll(this);

        EmailViewModel = new EmailViewModel();

        CanRefeshPhones = true;

        NorRs.Add("N(ew)");
        NorRs.Add("R(epurposed)");

        OEMs.Add("Apple");
        OEMs.Add("Nokia");
        OEMs.Add("Samsung");

        Statuses.Add("Production");
        Statuses.Add("In Stock");
        Statuses.Add("In Repair");
        Statuses.Add("Misplaced");
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

    [ObservableProperty]
    private v1Phone? _selectedItem;

    [ObservableProperty]
    private string _imei;

    [ObservableProperty]
    private string _phoneNumber;

    //[ObservableProperty]
    //private FlowDocument _emailFlowDocument;

    #region Filtering View
    public bool FilterView(object item)
    {
        if (item is not PhonesItemViewModel vm) return false;

        if (FilterNorR is not null && FilterNorR.Length == 1)
            if (vm.NorR is not null && !vm.NorR.StartsWith(FilterNorR, StringComparison.InvariantCultureIgnoreCase))
                return false;

        if (FilterStatus is not null && FilterStatus.Length > 0)
            if (vm.Status is not null && !vm.Status.Contains(FilterStatus, StringComparison.InvariantCultureIgnoreCase))
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

        if (FilterFormerUser is not null && FilterFormerUser.Length > 0)
            if (vm.FormerUser is null)
                return false;
            else if (!vm.FormerUser.Contains(FilterFormerUser, StringComparison.InvariantCultureIgnoreCase))
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
    private string? filterFormerUser;

    partial void OnFilterFormerUserChanged(string? value)
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

    public async Task LoadAsync()
    {
        PhoneItems.Clear();
        IEnumerable<v1Phone> phones = await _phonesRepository.GetPhonesAsync();

        foreach (v1Phone phone in phones) 
        {
            PhoneItems.Add(_phonesItemViewModelFactory.Create(phone));
        }
    }

    public Task WindowClosingAsync()
    {
        throw new NotImplementedException();
    }

    public void Receive(Email message)
    {
        SelectedItem = message.Phone;
        EmailViewModel.GenerateEmail(SelectedItem.Imei, SelectedItem.PhoneNumber);
    }
}
