using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Microsoft.EntityFrameworkCore;

using PhoneAssistant.WPF.Application;
using PhoneAssistant.WPF.Application.Entities;

namespace PhoneAssistant.WPF.Features.Phones;

public sealed partial class PhonesMainViewModel : ObservableValidator, IPhonesMainViewModel
{
    private readonly IPhonesRepository _phonesRepository;
    private readonly IPrintEnvelope _printEnvelope;

    public ObservableCollection<v1Phone> Phones { get; } = new();

    ICollectionView _filterView;

    public PhonesMainViewModel(IPhonesRepository phonesRepository, IPrintEnvelope printEnvelope)
    {
        _phonesRepository = phonesRepository ?? throw new ArgumentNullException(nameof(phonesRepository));
        _printEnvelope = printEnvelope ?? throw new ArgumentNullException(nameof(printEnvelope));

        _filterView = CollectionViewSource.GetDefaultView(Phones);
        _filterView.Filter = new Predicate<object>(FilterView);
    }

    #region Buttons
    [RelayCommand]
    private async Task RefreshPhones()
    {
        await LoadAsync();
        _filterView.Refresh();
    }

    [RelayCommand]
    private void PrintEnvelope()
    {
        if (SelectedPhone is null) return;

        CanPrintEnvelope = false;
        _printEnvelope.Execute(SelectedPhone);
    }
    #endregion

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
        if (item is not v1Phone phone) return false;

        if (FilterNorR is not null && FilterNorR.Length == 1)
            if (phone.NorR is not null && !phone.NorR.StartsWith(FilterNorR, StringComparison.InvariantCultureIgnoreCase))
                return false;

        if (FilterStatus is not null && FilterStatus.Length > 0)
            if (phone.Status is not null && !phone.Status.Contains(FilterStatus,StringComparison.InvariantCultureIgnoreCase))
                return false;

        if (FilterSR is not null && FilterSR.Length > 0)
            if (phone.SR is null)
                return false;
            else if (!phone.SR.ToString()!.Contains(FilterSR))
                return false;        

        if (FilterImei is not null && FilterImei.Length > 0)
            if (phone.Imei is not null && !phone.Imei.Contains(FilterImei))
                return false;

        if (FilterPhoneNumber is not null && FilterPhoneNumber.Length > 0)
            if (phone.PhoneNumber is null)
                return false;
            else if (!phone.PhoneNumber.Contains(FilterPhoneNumber))
                return false;

        if (FilterSimNumber is not null && FilterSimNumber.Length > 0)
            if (phone.SimNumber is null)
                return false;
            else if (!phone.SimNumber.Contains(FilterSimNumber))
                return false;

        if (FilterNewUser is not null && FilterNewUser.Length > 0)
            if (phone.NewUser is null)
                return false;
            else if (!phone.NewUser.Contains(FilterNewUser, StringComparison.InvariantCultureIgnoreCase))
                return false;

        if (FilterAssetTag is not null && FilterAssetTag.Length > 0)
            if (phone.AssetTag is null)
                return false;
            else if (!phone.AssetTag.Contains(FilterAssetTag, StringComparison.InvariantCultureIgnoreCase))
                return false;

        if (FilterOEM is not null && FilterOEM.Length > 0)
            if (phone.OEM is null)
                return false;
            else if (!phone.OEM.Contains(FilterOEM, StringComparison.InvariantCultureIgnoreCase))
                return false;

        if (FilterNotes is not null && FilterNotes.Length > 0)
            if (phone.Notes is null)
                return false;
            else if (!phone.Notes.Contains(FilterNotes, StringComparison.InvariantCultureIgnoreCase))
                return false;

        if (FilterLastUpdate is not null && FilterLastUpdate.Length > 0)
            if (phone.LastUpdate is null)
                return false;
            else if (!phone.LastUpdate.Contains(FilterLastUpdate, StringComparison.InvariantCultureIgnoreCase))
                return false;

        return true;
    }
    #endregion

    [ObservableProperty]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "CommunityToolkit.Mvvm")]
    private bool canPrintEnvelope;

    [ObservableProperty]
    private v1Phone? _selectedPhone;

    partial void OnSelectedPhoneChanged(v1Phone? value)
    {
        if (SelectedPhone is null)
        {
            CanPrintEnvelope = false;
        }
        else
        {
            CanPrintEnvelope = true;
        }
    }

    public async Task LoadAsync()
    {
        Phones.Clear();
        IEnumerable<v1Phone> phones = await _phonesRepository.GetPhonesAsync();

        foreach (v1Phone phone in phones) 
        {
            if (phone.NorR == "N")
                phone.NorR = "New";
            else
                phone.NorR = "Repurposed";
            Phones.Add(phone);
        }
    }

    public Task WindowClosingAsync()
    {
        throw new NotImplementedException();
    }
}
