using CommunityToolkit.Mvvm.Messaging;

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

        Reconciliation reconciliation = new(disposalsRepository);

        foreach (Phone phone in phones)
        {
            Result result = await reconciliation.DisposalAsync(Import.PA, phone.Imei, phone.Status);
            switch (result)
            {
                case Result.Added:
                    added++; break;
                case Result.Updated:
                    updated++; break;
                case Result.Unchanged:
                    unchanged++; break;
            }
        }

        messenger.Send(new LogMessage($"Added {added} disposals"));
        messenger.Send(new LogMessage($"Unchanged {unchanged} disposals"));
        messenger.Send(new LogMessage($"Updated {updated} disposals"));
        messenger.Send(new LogMessage("Import complete"));
    }
}
