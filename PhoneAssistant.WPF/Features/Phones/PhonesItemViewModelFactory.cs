using CommunityToolkit.Mvvm.Messaging;

using PhoneAssistant.WPF.Application.Entities;
using PhoneAssistant.WPF.Application.Repositories;

namespace PhoneAssistant.WPF.Features.Phones;
public sealed class PhonesItemViewModelFactory(IPhonesRepository repository, 
                                               ISimsRepository simsRepository,
                                               IPrintEnvelope printEnvelope, 
                                               IMessenger messenger) : IPhonesItemViewModelFactory
{
    private readonly IPhonesRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    private readonly ISimsRepository _simsRepository = simsRepository ?? throw new ArgumentNullException(nameof(simsRepository));
    private readonly IPrintEnvelope _printEnvelope = printEnvelope ?? throw new ArgumentNullException(nameof(printEnvelope));
    private readonly IMessenger _messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));

    public PhonesItemViewModel Create(Phone phone) => new PhonesItemViewModel(_repository, _simsRepository, _printEnvelope, _messenger, phone);
}