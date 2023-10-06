using PhoneAssistant.WPF.Application.Entities;

namespace PhoneAssistant.WPF.Features.Phones;
public sealed class PhonesItemViewModelFactory : IPhonesItemViewModelFactory
{
    private readonly IPhonesRepository _repository;
    private readonly IPrintEnvelope _printEnvelope;

    public PhonesItemViewModelFactory(IPhonesRepository repository, IPrintEnvelope printEnvelope)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _printEnvelope = printEnvelope ?? throw new ArgumentNullException(nameof(printEnvelope));
    }

    public PhonesItemViewModel Create(v1Phone phone)
    {
        PhonesItemViewModel vm = new(_repository, _printEnvelope);
        vm.Phone = phone;
        return vm;
    }
}