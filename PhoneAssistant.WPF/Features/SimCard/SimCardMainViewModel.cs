using CommunityToolkit.Mvvm.ComponentModel;
using PhoneAssistant.WPF.Models;
using PhoneAssistant.WPF.Shared;
using System.Collections.ObjectModel;

namespace PhoneAssistant.WPF.Features.SimCard;

public sealed partial class SimCardMainViewModel : ObservableObject, IViewModel
{
    private readonly SimRepository _simCardRepository;
    private readonly StateRepository _stateRepository;

    public SimCardMainViewModel(SimRepository simCardRepository, StateRepository stateRepository)
    {
        _simCardRepository = simCardRepository;
        _stateRepository = stateRepository;
    }

    public ObservableCollection<Sim> Sims { get; } = new();

    public ObservableCollection<string> States { get; } = new();

    [ObservableProperty]
    private SimCardItemViewModel? _selectedSimCardViewModel;

    [ObservableProperty]
    private Sim? _selectedSimCard;

    partial void OnSelectedSimCardChanged(Sim? value)
    {
        if (value is not null)
            SelectedSimCardViewModel = new SimCardItemViewModel(value);
    }

    public async Task LoadAsync()
    {
        if (Sims.Any() && States.Any())
            return;

        if (!Sims.Any())
        {
            var simCards = await _simCardRepository.AllAsync();
            if (simCards == null)
            {
                throw new ArgumentNullException(nameof(simCards));
            }

            foreach (var simcard in simCards)
            {
                Sims.Add(simcard);
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
