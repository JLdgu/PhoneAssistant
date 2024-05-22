using CommunityToolkit.Mvvm.Messaging;

using PhoneAssistant.WPF.Application.Entities;
using PhoneAssistant.WPF.Application.Repositories;

namespace PhoneAssistant.WPF.Features.Disposals;

public sealed class ImportPhoneAssistant(IDisposalsRepository disposalsRepository, 
                                         IPhonesRepository phonesRepository,     
                                         IMessenger messenger)
{    
    public async Task Execute()
    {
        messenger.Send(new LogMessage(MessageType.Default, $"Importing from PhoneAssistant database"));

        IEnumerable<Phone> phones = await phonesRepository.GetAllPhonesAsync();

        int added = 0;
        int updated = 0;
        int unchanged = 0;


        foreach (Phone phone in phones)
        {
            Result result = await disposalsRepository.AddOrUpdatePAAsync(phone.Imei, phone.Status, phone.SR);
            switch (result)
            {
                case Result.Added:
                    added++; 
                    break;
                case Result.Updated:
                    updated++; 
                    break;
                case Result.Unchanged:
                    unchanged++; 
                    break;
            }
        }

        messenger.Send(new LogMessage(MessageType.Default, $"Added {added} disposals"));
        messenger.Send(new LogMessage(MessageType.Default, $"Updated {updated} disposals"));
        messenger.Send(new LogMessage(MessageType.Default, $"Unchanged {unchanged} disposals"));
        messenger.Send(new LogMessage(MessageType.Default, "Import complete"));
    }
}
