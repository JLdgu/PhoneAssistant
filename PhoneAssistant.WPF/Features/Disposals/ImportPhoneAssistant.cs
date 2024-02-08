using CommunityToolkit.Mvvm.Messaging;

using MaterialDesignThemes.Wpf;

using PhoneAssistant.WPF.Application.Entities;
using PhoneAssistant.WPF.Application.Repositories;

namespace PhoneAssistant.WPF.Features.Disposals;
public sealed class ImportPhoneAssistant(DisposalsRepository disposalsRepository, 
                                         IPhonesRepository phonesRepository,     
                                         IMessenger messenger)
{    
    public async Task Execute()
    {
        IEnumerable<Phone> phones = await phonesRepository.GetActivePhonesAsync();

        int added = 0;
        int updated = 0;
        int unchanged = 0;

        foreach (Phone phone in phones)
        {
            Disposal? disposal = await disposalsRepository.GetDisposalAsync(phone.Imei);
            if (disposal is null)
            {
                disposal = new() { Imei = phone.Imei, StatusPA = phone.Status, Action = "Missing from myScomis"};
                await disposalsRepository.AddAsync(disposal);
                added++;
            }
        }

        messenger.Send(new LogMessage($"Added {added} disposals"));
        messenger.Send(new LogMessage($"Unchanged {unchanged} disposals"));
        messenger.Send(new LogMessage($"Updated {updated} disposals"));
        messenger.Send(new LogMessage("Import complete"));
    }
}
