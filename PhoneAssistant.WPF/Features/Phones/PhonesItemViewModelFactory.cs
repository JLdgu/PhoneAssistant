using CommunityToolkit.Mvvm.Messaging;
using PhoneAssistant.Model;

namespace PhoneAssistant.WPF.Features.Phones;

public sealed class PhonesItemViewModelFactory(IApplicationSettingsRepository appSettings,
                                  ISimRepository simRepository,
                                  IPhonesRepository phonesRepository,
                                  IMessenger messenger) : IPhonesItemViewModelFactory
{
    private readonly IApplicationSettingsRepository _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
    private readonly IMessenger _messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
    private readonly IPhonesRepository _phonesRepository = phonesRepository ?? throw new ArgumentNullException(nameof(phonesRepository));
    private readonly ISimRepository _simRepository = simRepository ?? throw new ArgumentNullException(nameof(simRepository));

    public PhonesItemViewModel Create(Phone phone) => new(_appSettings, _phonesRepository, _simRepository, _messenger, phone);
}