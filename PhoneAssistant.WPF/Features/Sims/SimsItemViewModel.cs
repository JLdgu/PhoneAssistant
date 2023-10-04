using PhoneAssistant.WPF.Application.Entities;

namespace PhoneAssistant.WPF.Features.Sims;

public sealed class SimsItemViewModel
{
    public v1Sim Sim { get; set; }

    private readonly ISimsRepository _simsRepository;

    public SimsItemViewModel(ISimsRepository simsRepository)
    {
        _simsRepository = simsRepository ?? throw new ArgumentNullException(nameof(simsRepository));
    }
}
