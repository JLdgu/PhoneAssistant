using CommunityToolkit.Mvvm.Messaging;
using PhoneAssistant.Model;
using PhoneAssistant.WPF.Shared;

namespace PhoneAssistant.WPF.Features.Phones;

public sealed class PhonesItemViewModelFactory : IPhonesItemViewModelFactory
{
    private readonly IApplicationSettingsRepository _appSettings;
    private readonly IBaseReportRepository _baseReportRepository;
    private readonly IPhonesRepository _phonesRepository;
    private readonly IMessenger _messenger;

    public PhonesItemViewModelFactory(IApplicationSettingsRepository appSettings,        
                                      IBaseReportRepository baseReportRepository,
                                      IPhonesRepository phonesRepository,
                                      IMessenger messenger)
    {
        _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
        _baseReportRepository = baseReportRepository ?? throw new ArgumentNullException(nameof(baseReportRepository));
        _phonesRepository = phonesRepository ?? throw new ArgumentNullException(nameof(phonesRepository));
        _messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
    }

    public PhonesItemViewModel Create(Phone phone) => new(_appSettings, _baseReportRepository, _phonesRepository, _messenger, phone);
}