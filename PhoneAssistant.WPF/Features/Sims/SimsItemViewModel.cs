using PhoneAssistant.WPF.Application.Entities;

namespace PhoneAssistant.WPF.Features.Sims;

public sealed class SimsItemViewModel
{
    private readonly ISimsRepository _simsRepository;

    public string PhoneNumber { get; set; }
    public string SimNumber { get; set; }
    public string? Status { get; set; }
    public int? SR { get; set; }
    public string? AssetTag { get; set; }
    public string? Notes { get; set; }
    public string? LastUpdate { get; }
    public SimsItemViewModel(v1Sim sim, ISimsRepository simsRepository)
    {
        PhoneNumber = sim.PhoneNumber;
        SimNumber = sim.SimNumber;
        Status = sim.Status;
        SR = sim.SR;
        AssetTag = sim.AssetTag;
        Notes = sim.Notes;
        LastUpdate = sim.LastUpdate;

        _simsRepository = simsRepository ?? throw new ArgumentNullException(nameof(simsRepository));
    }
}
