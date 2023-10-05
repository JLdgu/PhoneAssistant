using PhoneAssistant.WPF.Application.Entities;
using PhoneAssistant.WPF.Features.Sims;

namespace PhoneAssistant.WPF.Features.Phones;
public sealed class PhonesItemViewModelFactory : IPhonesItemViewModelFactory
{
    private readonly IPhonesRepository _repository;

    public PhonesItemViewModelFactory(IPhonesRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public PhonesItemViewModel Create(v1Phone phone)
    {
        PhonesItemViewModel vm = new(_repository);
        vm.Phone = phone;
        return vm;
    }
}