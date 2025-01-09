using CommunityToolkit.Mvvm.Messaging;

using PhoneAssistant.WPF.Application;
using PhoneAssistant.WPF.Application.Entities;
using PhoneAssistant.WPF.Application.Repositories;

namespace PhoneAssistant.WPF.Features.Phones;
public sealed class PhonesItemViewModelFactory : IPhonesItemViewModelFactory
{
    private readonly IPhonesRepository _repository;
    private readonly IBaseReportRepository _baseReportRepository;
    private readonly IUserSettings _userSettings;
    private readonly IPrintEnvelope _printEnvelope;
    private readonly IMessenger _messenger;

    public PhonesItemViewModelFactory(IPhonesRepository repository,
                                      IBaseReportRepository baseReportRepository,
                                      IUserSettings userSettings,
                                      IPrintEnvelope printEnvelope, 
                                      IMessenger messenger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _baseReportRepository = baseReportRepository ?? throw new ArgumentNullException(nameof(baseReportRepository));
        _userSettings = userSettings ?? throw new ArgumentNullException(nameof(userSettings));
        _printEnvelope = printEnvelope ?? throw new ArgumentNullException(nameof(printEnvelope));
        _messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
    }

    public PhonesItemViewModel Create(Phone phone) 
        => new PhonesItemViewModel(_repository, _baseReportRepository, _userSettings, _printEnvelope, _messenger, phone);
}