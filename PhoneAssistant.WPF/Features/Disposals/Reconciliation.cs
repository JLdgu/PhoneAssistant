using CommunityToolkit.Mvvm.Messaging;

using PhoneAssistant.WPF.Application;
using PhoneAssistant.WPF.Application.Entities;
using PhoneAssistant.WPF.Application.Repositories;

namespace PhoneAssistant.WPF.Features.Disposals;

public sealed class Reconciliation(IDisposalsRepository disposalsRepository,
                                   IMessenger messenger)
{
    public async Task Execute()
    {
        IEnumerable<Disposal> disposals = await disposalsRepository.GetAllDisposalsAsync();
        messenger.Send(new LogMessage(MessageType.ReconciliationMaxProgress, "", disposals.Count()));

        int row = 1;
        TrackProgress progress = new(disposals.Count());
        string? lastAction;

        await Task.Run(async delegate
        {
            foreach (Disposal disposal in disposals)
            {
                lastAction = disposal.Action;
                CheckStatus(disposal);
                if (disposal.Action != lastAction)
                    await disposalsRepository.UpdateAsync(disposal);

                if (progress.Milestone(row))
                    messenger.Send(new LogMessage(MessageType.ReconciliationProgress, "", row));

                row++;
            }
        });

        messenger.Send(new LogMessage(MessageType.Default, "Reconciliation complete"));
    }

    public static void CheckStatus(Disposal disposal)
    {
        ArgumentNullException.ThrowIfNull(disposal);

        if (disposal.StatusMS is null)
        {
            if (disposal.StatusPA == ApplicationSettings.StatusDisposed)
            {
                disposal.Action = null;
                return;
            }
            else
            {
                disposal.Action = "Phone CI missing from myScomis";
                return;
            }
        }

        if (disposal.StatusMS == ApplicationSettings.StatusProduction && disposal.StatusPA == ApplicationSettings.StatusInStock)
        {
            disposal.Action = "Reconcile";
            return;
        }
        if (disposal.StatusMS == ApplicationSettings.StatusDisposed && disposal.StatusPA == ApplicationSettings.StatusDisposed)
        {
            disposal.Action = null;
            return;
        }


        
        disposal.Action = "No matching reconcilation rule";
    }
}
