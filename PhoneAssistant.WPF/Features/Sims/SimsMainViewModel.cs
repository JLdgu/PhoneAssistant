using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;

using PhoneAssistant.WPF.Application;
using PhoneAssistant.WPF.Models;
using PhoneAssistant.WPF.Shared;

namespace PhoneAssistant.WPF.Features.Sims;

public sealed partial class SimsMainViewModel : ObservableObject, ISimsMainViewModel
{
    private readonly SimsRepository _simCardRepository;
    private readonly StateRepository _stateRepository;

    public SimsMainViewModel(SimsRepository simCardRepository, StateRepository stateRepository)
    {
        _simCardRepository = simCardRepository;
        _stateRepository = stateRepository;
    }

    public ObservableCollection<Sim> Sims { get; } = new();

    public ObservableCollection<string> States { get; } = new();

    [ObservableProperty]
    private SimsItemViewModel? _selectedSimCardViewModel;

    [ObservableProperty]
    private Sim? _selectedSimCard;

    partial void OnSelectedSimCardChanged(Sim? value)
    {
        if (value is not null)
            SelectedSimCardViewModel = new SimsItemViewModel(value);
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

    public Task WindowClosingAsync()
    {
        // TODO : Check outstanding edits have been saved
        return Task.CompletedTask;

    }
}
