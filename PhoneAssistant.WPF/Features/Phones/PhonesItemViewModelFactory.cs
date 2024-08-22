using CommunityToolkit.Mvvm.Messaging;

using PhoneAssistant.WPF.Application.Entities;
using PhoneAssistant.WPF.Application.Repositories;

namespace PhoneAssistant.WPF.Features.Phones;
public sealed class PhonesItemViewModelFactory : IPhonesItemViewModelFactory
{
    private readonly IPhonesRepository _repository;
    private readonly IBaseReportRepository _baseReportRepository;
    private readonly IPrintEnvelope _printEnvelope;
    private readonly IMessenger _messenger;

    public PhonesItemViewModelFactory(IPhonesRepository repository,
                                      IBaseReportRepository baseReportRepository,
                                      IPrintEnvelope printEnvelope, 
                                      IMessenger messenger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _baseReportRepository = baseReportRepository ?? throw new ArgumentNullException(nameof(baseReportRepository));
        _printEnvelope = printEnvelope ?? throw new ArgumentNullException(nameof(printEnvelope));
        _messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
    }

    public PhonesItemViewModel Create(Phone phone) => new PhonesItemViewModel(_repository, _baseReportRepository, _printEnvelope, _messenger, phone);
}