using System.Collections.ObjectModel;
using System.Diagnostics;

using CommunityToolkit.Mvvm.ComponentModel;

using PhoneAssistant.WPF.Application;
using PhoneAssistant.WPF.Models;
using PhoneAssistant.WPF.Shared;

namespace PhoneAssistant.WPF.Features.Sims;

public sealed partial class SimsMainViewModel : ObservableObject, ISimsMainViewModel
{
    private readonly ISimsRepository _simRepository;
    private readonly IStateRepository _stateRepository;


    public SimsMainViewModel(ISimsRepository simRepository, IStateRepository stateRepository)
    {
        _simRepository = simRepository;
        _stateRepository = stateRepository;
    }

    public ObservableCollection<Sim> Sims { get; } = new();

    public ObservableCollection<string> States { get; } = new();
   
    [ObservableProperty]
    private Sim? _selectedSim;

    private int _previousSimIndex = -1;
    private Sim? _previousSim;

    [ObservableProperty]
    private int _id;

    partial void OnIdChanged(int value)
    {
        SelectedSim.Id = Id;    
    }

    partial void OnSelectedSimChanged(Sim? value)
    {
        if (SelectedSim is null) return;
        if (value is null) return;

        if (_previousSimIndex > -1)
        {
            Sims.RemoveAt(_previousSimIndex);
            Sims.Insert(_previousSimIndex, _previousSim);            
        }

        Id = value.Id;

        _previousSimIndex = Sims.IndexOf(value);
        _previousSim = value;
    }

    public async Task LoadAsync()
    {
        if (Sims.Any() && States.Any())
            return;

        if (!Sims.Any())
        {
            var simCards = await _simRepository.GetSimsAsync();
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

    public Task WindowClosingAsync()
    {
        // TODO : Check outstanding edits have been saved
        return Task.CompletedTask;

    }
}
