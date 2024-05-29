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
        messenger.Send(new LogMessage(MessageType.MaxProgress, "", disposals.Count()));

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
                    messenger.Send(new LogMessage(MessageType.Progress, "", row));

                row++;
            }
        });

        messenger.Send(new LogMessage(MessageType.Default, "Reconciliation complete"));
    }

    public static void CheckStatus(Disposal disposal)
    {
        ArgumentNullException.ThrowIfNull(disposal);
        
        if (disposal.StatusMS is null && disposal.StatusPA == ApplicationSettings.StatusDisposed && disposal.StatusSCC == ApplicationSettings.StatusDisposed)
        {
            disposal.Action = null;
            return;
        }

        if (disposal.StatusMS is null && disposal.StatusPA is null && disposal.StatusSCC == ApplicationSettings.StatusDisposed)
        {
            disposal.Action = "Check CI linked to Disposal SR or add IMEI to Phones";
            return;
        }

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
            if (disposal.StatusSCC == ApplicationSettings.StatusDisposed)
            {
                disposal.Action = "Update phone status in myScomis and PhoneAssistant";
                return;
            }
        }

        if (disposal.StatusMS == ApplicationSettings.StatusDecommissioned && disposal.StatusPA == ApplicationSettings.StatusDisposed && disposal.StatusSCC == ApplicationSettings.StatusDisposed)
        {
            disposal.Action = "Update myScomis status to Disposed";
            return;
        }
        
        if (disposal.StatusMS == ApplicationSettings.StatusDisposed)
        {
            if (disposal.StatusPA is null && disposal.StatusSCC is null)
            {
                disposal.Action = "Check if phone is an SCC disposal";
                return;
            }
            if (disposal.StatusPA is null && disposal.StatusSCC == ApplicationSettings.StatusDisposed)
            {
                disposal.Action = "Add phone to PhoneAssistant";
                return;
            }
            if (disposal.StatusPA == ApplicationSettings.StatusDisposed && disposal.StatusSCC == ApplicationSettings.StatusDisposed)
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
            if (disposal.StatusPA == ApplicationSettings.StatusInStock && disposal.StatusSCC is null)
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

        disposal.Action = "No matching reconcilation rule";
    }
}
