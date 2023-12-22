using CommunityToolkit.Mvvm.Messaging;

using PhoneAssistant.WPF.Application.Entities;
using PhoneAssistant.WPF.Application.Repositories;

namespace PhoneAssistant.WPF.Features.Phones;
public sealed class PhonesItemViewModelFactory : IPhonesItemViewModelFactory
{
    private readonly IPhonesRepository _repository;
    private readonly IPrintEnvelope _printEnvelope;
    private readonly IMessenger _messenger;

    public PhonesItemViewModelFactory(IPhonesRepository repository, IPrintEnvelope printEnvelope, IMessenger messenger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _printEnvelope = printEnvelope ?? throw new ArgumentNullException(nameof(printEnvelope));
        _messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
    }

    public PhonesItemViewModel Create(v1Phone phone) => new PhonesItemViewModel(_repository, _printEnvelope, _messenger, phone);
}

public sealed record class Email(v1Phone Phone);