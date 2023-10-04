using PhoneAssistant.WPF.Application.Entities;

namespace PhoneAssistant.WPF.Features.Sims;

public sealed class SimsItemViewModelFactory : ISimsItemViewModelFactory
{
    private readonly ISimsRepository _simsRepository;

    public SimsItemViewModelFactory(ISimsRepository simsRepository)
    {
        _simsRepository = simsRepository ?? throw new ArgumentNullException(nameof(simsRepository));
    }

    public SimsItemViewModel Create(v1Sim sim)
    {
        SimsItemViewModel vm = new(_simsRepository);
        vm.Sim = sim;
        return vm;
    }
}
