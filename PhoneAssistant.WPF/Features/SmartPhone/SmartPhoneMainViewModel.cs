using CommunityToolkit.Mvvm.ComponentModel;
using PhoneAssistant.WPF.Models;
using PhoneAssistant.WPF.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace PhoneAssistant.WPF.Features.SmartPhone;

internal sealed partial class SmartPhoneMainViewModel : ObservableObject, IViewModel
{
    private readonly PhoneRepository _phoneRepository;
    private readonly StateRepository _stateRepository;

    public SmartPhoneMainViewModel(PhoneRepository phoneRepository, StateRepository stateRepository)
    {
        _phoneRepository = phoneRepository;
        _stateRepository = stateRepository;
    }

    public ObservableCollection<Phone> Phones { get; } = new();

    public List<string> States { get; } = new();

    [ObservableProperty]
    private Phone _selectedPhone;

    partial void OnSelectedPhoneChanged(Phone value)
    {
        Phone phone = Phones.First(x => x.Id == value.Id);
        if (phone != null)
        {
            _phoneRepository.SaveChanges(phone);
        }
    }

    //public string ViewName => "Phones";
    //public string ViewPackIcon => "CellphoneScreenshot";

    public async Task LoadAsync()
    {
        if (Phones.Any() && States.Any())
            return;

        if (!Phones.Any())
        {
            var phones = await _phoneRepository.AllAsync();
            if (phones == null)
            {
                throw new ArgumentNullException(nameof(phones));
            }

            foreach (var phone in phones)
            {
                Phones.Add(phone);
            }
        }

        if (States.Any())
            return;

        var states = await _stateRepository.AllAsync();
        if (states == null)
        {
            throw new ArgumentNullException(nameof(states));
        }

        foreach (var state in states)
        {
            States.Add(state.Status);
        }
    }
}
