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

        if (disposal.StatusMS == ApplicationSettings.StatusAwaitingReturn ||
            disposal.StatusMS == ApplicationSettings.StatusLost ||
            disposal.StatusMS == ApplicationSettings.StatusProduction ||
            disposal.StatusMS == ApplicationSettings.StatusUnlocated)
        {
            if (disposal.StatusPA is null && disposal.StatusSCC is null)
            {
                disposal.Action = null;
                return;
            }
        }

        if (disposal.StatusMS == ApplicationSettings.StatusDecommissioned && disposal.StatusPA == ApplicationSettings.StatusDecommissioned)
        {
            if (disposal.StatusSCC == null)
            {
                disposal.Action = null;
                return;
            }
            else if (disposal.StatusSCC == ApplicationSettings.StatusDisposed)
            {
                disposal.Action = "Update phone status in myScomis and PhoneAssistant";
                return;
            }
        }

        if (disposal.StatusMS == ApplicationSettings.StatusDisposed)
        {
            if (disposal.StatusPA is null && disposal.StatusSCC is null)
            {
                disposal.Action = "Check if phone is an SCC disposal";
                return;
            }
            else if (disposal.StatusPA == ApplicationSettings.StatusDisposed && disposal.StatusSCC == ApplicationSettings.StatusDisposed)
            {
                disposal.Action = null;
                return;
            }
        }

        if (disposal.StatusMS == ApplicationSettings.StatusInStock)
        {
            if (disposal.StatusPA is null && disposal.StatusSCC is null)
            {
                disposal.Action = "Phone needs to be logged in PhoneAssistant";
                return;
            }
            else if (disposal.StatusPA == ApplicationSettings.StatusInStock && disposal.StatusSCC is null)
            {
                disposal.Action = null;
                return;
            }

        }

        if (disposal.StatusMS == ApplicationSettings.StatusProduction && disposal.StatusPA == ApplicationSettings.StatusProduction && disposal.StatusSCC is null)
        {
            disposal.Action = null;
            return;
        }

        //if (disposal.StatusMS == ApplicationSettings.StatusProduction && disposal.StatusPA == ApplicationSettings.StatusInStock)
        //    disposal.Action = "Reconcile";
        //if (disposal.StatusMS == ApplicationSettings.StatusDisposed && disposal.StatusPA == ApplicationSettings.StatusDisposed)
        //    disposal.Action = null;

        disposal.Action = "No matching reconcilation rule";
    }
}
