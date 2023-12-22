using System.Numerics;

using CommunityToolkit.Mvvm.ComponentModel;

using PhoneAssistant.WPF.Application.Entities;
using PhoneAssistant.WPF.Application.Repositories;

namespace PhoneAssistant.WPF.Features.Sims;

public sealed partial class SimsItemViewModel : ObservableObject
{
    private readonly ISimsRepository _simsRepository;
    private readonly Sim _sim;

    public SimsItemViewModel(ISimsRepository simsRepository, Sim sim)
    {
        _simsRepository = simsRepository ?? throw new ArgumentNullException(nameof(simsRepository));
        _sim = sim ?? throw new ArgumentNullException(nameof(sim));

        AssetTag = sim.AssetTag ?? string.Empty;
        LastUpdate = sim.LastUpdate ?? string.Empty;
        Notes = sim.Notes ?? string.Empty;
        PhoneNumber = sim.PhoneNumber ?? string.Empty;
        SimNumber = sim.SimNumber ?? string.Empty;
        SR = sim.SR.ToString() ?? string.Empty;
        Status = sim.Status ?? string.Empty;
    }

    [ObservableProperty]
    private string _assetTag;

    [ObservableProperty]
    private string _lastUpdate;

    [ObservableProperty] 
    private string _notes;

    [ObservableProperty]
    private string _phoneNumber;

    [ObservableProperty]
    private string _simNumber;

    [ObservableProperty]
    private string _sR;

    [ObservableProperty]
    private string _status;
}
