using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Controls;

using CommunityToolkit.Mvvm.ComponentModel;

using PhoneAssistant.WPF.Application;
using PhoneAssistant.WPF.Models;
using PhoneAssistant.WPF.Shared;

namespace PhoneAssistant.WPF.Features.Phones;

public sealed partial class PhonesMainViewModel : ObservableObject, IViewModel
{
    private readonly IPhonesRepository _phoneRepository;
    private readonly StateRepository _stateRepository;

    public PhonesMainViewModel(IPhonesRepository phoneRepository,
                                   StateRepository stateRepository)
    {
        _phoneRepository = phoneRepository;
        _stateRepository = stateRepository;
    }

    public ObservableCollection<Phone> Phones { get; } = new();

    public List<string> States { get; } = new();

    [ObservableProperty]
    private Phone _selectedPhone;

    async partial void OnSelectedPhoneChanging(Phone value)
    {
        if (SelectedPhone is null) return;
        await _phoneRepository.UpdateAsync(SelectedPhone);
    }

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
