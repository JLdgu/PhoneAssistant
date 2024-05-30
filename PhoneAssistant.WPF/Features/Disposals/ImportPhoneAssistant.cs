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

        IEnumerable<Disposal> disposals = await disposalsRepository.GetAllDisposalsAsync();
        messenger.Send(new LogMessage(MessageType.MaxProgress, "", disposals.Count()));

        int added = 0;
        int updated = 0;
        int unchanged = 0;
        int row = 1;
        TrackProgress progress = new(disposals.Count());

        await Task.Run(async delegate
        {
            foreach (Disposal disposal in disposals)
            {
                Phone? phone = await phonesRepository.GetPhoneAsync(disposal.Imei);
                if (phone is not null)
                {
                    Result result = await disposalsRepository.AddOrUpdatePAAsync(disposal.Imei, phone.Status, phone.SR);
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

                if (progress.Milestone(row))
                {
                    messenger.Send(new LogMessage(MessageType.Progress, "", row));
                }
                row++;
            }
        });

        messenger.Send(new LogMessage(MessageType.Default, $"Added {added} disposals"));
        messenger.Send(new LogMessage(MessageType.Default, $"Updated {updated} disposals"));
        messenger.Send(new LogMessage(MessageType.Default, $"Unchanged {unchanged} disposals"));
        messenger.Send(new LogMessage(MessageType.Default, "Import complete"));
    }
}
