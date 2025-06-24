using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CsvHelper;
using PhoneAssistant.Model;
using PhoneAssistant.WPF.Application.Entities;
using PhoneAssistant.WPF.Application.Repositories;
using PhoneAssistant.WPF.Shared;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace PhoneAssistant.WPF.Features.Phones;

public sealed partial class PhonesMainViewModel :
    ObservableValidator,
    IRecipient<Order>,
    IRecipient<Phone>,
    IPhonesMainViewModel
{
    private readonly IPhonesItemViewModelFactory _phonesItemViewModelFactory;
    private readonly IPhonesRepository _phonesRepository;

    public ObservableCollection<PhonesItemViewModel> PhoneItems { get; } = new();

    public EmailViewModel EmailViewModel { get; }

    private readonly ListCollectionView _filterView;

    public List<string> Conditions { get; } = ApplicationConstants.Conditions;

    public IEnumerable<OEMs> OEMs => Enum.GetValues(typeof(OEMs)).Cast<OEMs>();

    public List<string> Statuses { get; } = ApplicationConstants.Statuses;

    public PhonesMainViewModel(IPhonesItemViewModelFactory phonesItemViewModelFactory,
                               IPhonesRepository phonesRepository,
                               EmailViewModel emailViewModel,
                               IMessenger messenger)
    {
        _phonesItemViewModelFactory = phonesItemViewModelFactory ?? throw new ArgumentNullException(nameof(phonesItemViewModelFactory));
        _phonesRepository = phonesRepository ?? throw new ArgumentNullException(nameof(phonesRepository));
        EmailViewModel = emailViewModel ?? throw new ArgumentNullException(nameof(emailViewModel));
        _filterView = (ListCollectionView)CollectionViewSource.GetDefaultView(PhoneItems);
        _filterView.Filter = new Predicate<object>(FilterView);

        messenger.RegisterAll(this);
    }

    [RelayCommand(CanExecute = nameof(CanExport))]
    private void ExportFiltered()
    {
        IEnumerable<PhonesItemViewModel> filtered = _filterView.Cast<PhonesItemViewModel>();
        List<Phone> phones = [];
        foreach (PhonesItemViewModel item in filtered)
        {
            Phone phone = new Phone
            {
                AssetTag = item.AssetTag,
                FormerUser = item.FormerUser,
                Imei = item.Imei,
                LastUpdate = item.LastUpdate,
                Model = item.Model,
                NewUser = item.NewUser,
                Condition = item.NorR,
                Notes = item.Notes,
                OEM = item.OEM,
                PhoneNumber = item.PhoneNumber,
                SimNumber = item.SimNumber,
                Status = item.Status
            };
            if (item.SR == string.Empty || item.SR == "0")
                phone.SR = null;
            else
                phone.SR = int.Parse(item.SR);
            phones.Add(phone);
        }

        string exportCsv = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            "Downloads",
            $"Phones {DateTime.Now:yyyy-MM-dd HHmmss}.csv");
        StreamWriter writer = new(exportCsv);
        CsvWriter csv = new(writer, CultureInfo.InvariantCulture);
        csv.WriteRecords(phones);
    }

    private bool CanExport => !_filterView.IsEmpty;

    [RelayCommand]
    private async Task RefreshPhones()
    {
        CanRefeshPhones = false;
        await LoadAsync();
    }

    private void RefreshFilterView()
    {
        if (_filterView.IsEditingItem)
            _filterView.CommitEdit();
        _filterView.Refresh();
        ExportFilteredCommand.NotifyCanExecuteChanged();
    }

    [ObservableProperty]
    private bool _canRefeshPhones;

    [ObservableProperty]
    private bool _includeDisposals;
    async partial void OnIncludeDisposalsChanged(bool value)
    {
        CanRefeshPhones = false;
        await LoadAsync();
    }

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

        if (FilterOEM is not null)
            //if (vm.OEM.Contains(FilterOEM, StringComparison.InvariantCultureIgnoreCase))
            if (vm.OEM != FilterOEM)
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
    private string? _filterNorR;
    partial void OnFilterNorRChanged(string? value)
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
    private string? _filterSR;
    partial void OnFilterSRChanged(string? value)
    {
        RefreshFilterView();
    }

    [ObservableProperty]
    private string? _filterImei;
    partial void OnFilterImeiChanged(string? value)
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
    private string? _filterNewUser;
    partial void OnFilterNewUserChanged(string? value)
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
    private OEMs? _filterOEM;
    partial void OnFilterOEMChanged(OEMs? value)
    {
        RefreshFilterView();
    }

    [ObservableProperty]
    private string? _filterModel;
    partial void OnFilterModelChanged(string? value)
    {
        RefreshFilterView();
    }

    [ObservableProperty]
    private string? _filterFormerUser;
    partial void OnFilterFormerUserChanged(string? value)
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

    public async Task LoadAsync()
    {
        await EmailViewModel.LoadAsync();

        if (!CanRefeshPhones)
        {
            PhoneItems.Clear();
            IEnumerable<Phone> phones = IncludeDisposals ? await _phonesRepository.GetAllPhonesAsync() : await _phonesRepository.GetActivePhonesAsync();
            foreach (Phone phone in phones)
            {
                PhoneItems.Add(_phonesItemViewModelFactory.Create(phone));
            }
        }

        _filterView.SortDescriptions.Clear();
        _filterView.SortDescriptions.Add(new SortDescription("LastUpdate", ListSortDirection.Descending));
        RefreshFilterView();

        CanRefeshPhones = true;
    }

    public void Receive(Order message)
    {
        EmailViewModel.OrderDetails = message.OrderDetails;
    }

    public void Receive(Phone message)
    {
        PhoneItems.Add(_phonesItemViewModelFactory.Create(message));
    }
}
