using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Windows.Data;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Microsoft.EntityFrameworkCore.Metadata.Conventions;

using PhoneAssistant.WPF.Application;
using PhoneAssistant.WPF.Models;
using PhoneAssistant.WPF.Shared;

namespace PhoneAssistant.WPF.Features.Phones;

public sealed partial class PhonesMainViewModel : ObservableValidator, IPhonesMainViewModel
{
    private readonly IPhonesRepository _phoneRepository;
    private readonly IStateRepository _stateRepository;

    public PhonesMainViewModel(IPhonesRepository phoneRepository, IStateRepository stateRepository)
    {
        _phoneRepository = phoneRepository;
        _stateRepository = stateRepository;
    }
    public List<string> States { get; } = new();

    public ObservableCollection<Phone> Phones { get; } = new();

    [ObservableProperty]
    private ListCollectionView _phonesView;

    [ObservableProperty]
    private Phone? _selectedPhone;

    private int _previousPhoneIndex = -1;
    private Phone? _previousPhone;

    partial void OnSelectedPhoneChanged(Phone? value)
    {
        if (SelectedPhone is null) return;
        if (value is null) return;

        if (_previousPhoneIndex > -1 && _previousPhone is not null)
        {
            Phones.RemoveAt(_previousPhoneIndex);
            Phones.Insert(_previousPhoneIndex, _previousPhone);
        }

        Imei = value.Imei;

        _previousPhoneIndex = Phones.IndexOf(value);
        _previousPhone = value;
    }

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required]
    [CustomValidation(typeof(PhonesMainViewModel), nameof(ValidateImei))]
    private string _imei;

    partial void OnImeiChanged(string value)
    {
        SelectedPhone.Imei = value;
    }
    public static ValidationResult? ValidateImei(string imei, ValidationContext context)
    {

        return LuhnValidator.IsValid(imei,15) ? ValidationResult.Success : new("IMEI format is invalid");
    }

    [RelayCommand]
    private void SaveSelectedPhoneUpdates()
    {
        //await _phoneRepository.UpdateAsync(SelectedPhone);
    }

    public bool SelectedPhoneHasUpdates { get; private set; }

    #region Filter

    [ObservableProperty]
    private string _filterPhones;

    partial void OnFilterPhonesChanged(string value)
    {
        //        wipedFilter = !wipedFilter;
        PhonesView.Filter = ApplyPhonesFilter;
        PhonesView.Refresh();
    }

    private bool ApplyPhonesFilter(object obj)
    {
        if (obj is not Phone) return true;
        return true; // phone.Wiped == wipedFilter;
    }
    #endregion

    public async Task LoadAsync()
    {
        if (Phones.Any() && States.Any())
            return;        

        var phones = await _phoneRepository.GetPhonesAsync();
        if (phones == null)
        {
            throw new ArgumentNullException(nameof(phones));
        }

        foreach (var phone in phones)
        {
            Phones.Add(phone);
        }
        PhonesView = new ListCollectionView(Phones);
        //PhonesView.Filter = p => ((Phone)p).Wiped == wipedFilter;

        if (States.Any())
            return;

        var states = await _stateRepository.GetStatesAsync();
        if (states == null)
        {
            throw new ArgumentNullException(nameof(states));
        }

        foreach (var state in states)
        {
            States.Add(state.Status);
        }
    }

    public async Task WindowClosingAsync()
    {
        if (SelectedPhone is null) return;
        await _phoneRepository.UpdateAsync(SelectedPhone);
    }
}
