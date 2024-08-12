using CommunityToolkit.Mvvm.Messaging;

using PhoneAssistant.WPF.Application;
using PhoneAssistant.WPF.Application.Entities;
using PhoneAssistant.WPF.Application.Repositories;
using PhoneAssistant.WPF.Shared;

namespace PhoneAssistant.WPF.Features.Disposals;

public sealed class Reconciliation
{
    private readonly IDisposalsRepository _disposalsRepository;
    private readonly IPhonesRepository _phonesRepository;
    private readonly IMessenger _messenger;

    public Reconciliation(IDisposalsRepository disposalsRepository,
                          IPhonesRepository phonesRepository,
                          IMessenger messenger)
    {
        _phonesRepository = phonesRepository ?? throw new ArgumentNullException(nameof(phonesRepository));
        _messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
        _disposalsRepository = disposalsRepository ?? throw new ArgumentNullException(nameof(disposalsRepository));
    }

    public async Task Execute()
    {
        _messenger.Send(new LogMessage(MessageType.Default, $"Reconciling imports"));

        IEnumerable<Disposal> disposals = await _disposalsRepository.GetAllDisposalsAsync();
        _messenger.Send(new LogMessage(MessageType.MaxProgress, "", disposals.Count()));

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
                    await _disposalsRepository.UpdateAsync(disposal);

                if (progress.Milestone(row))
                    _messenger.Send(new LogMessage(MessageType.Progress, "", row));

                row++;
            }
        });

        _messenger.Send(new LogMessage(MessageType.Default, "Reconciliation complete"));
    }

    public void CheckStatus(Disposal disposal)
    {
        ArgumentNullException.ThrowIfNull(disposal);

        if (disposal.Action is not null) return;

        if (!LuhnValidator.IsValid(disposal.Imei, 15))
        {
            disposal.Action = ReconciliationConstants.ImeiInvalid;
            return;
        }

        if (disposal.StatusMS == ApplicationSettings.StatusDisposed && disposal.StatusPA == ApplicationSettings.StatusDisposed)
        {
            disposal.Action = ReconciliationConstants.Complete;
            return;
        }

        if (disposal.StatusMS == ApplicationSettings.StatusAwaitingReturn)
        {
            disposal.Action = ReconciliationConstants.MyScomisExport;
            return;
        }

        if (disposal.StatusMS == ReconciliationConstants.StatusMissing && disposal.StatusPA == ApplicationSettings.StatusDisposed && disposal.TrackedSKU == false)
        {
            disposal.Action = ReconciliationConstants.Complete;
            return;
        }

        //if (disposal.StatusMS == ApplicationSettings.StatusDisposed)
        //{
        //    if (disposal.StatusPA is null)
        //    {
        //        disposal.Action = "Add phone to PhoneAssistant";
        //        return;
        //    }
        //    if (disposal.StatusPA == ApplicationSettings.StatusDecommissioned)
        //    {
        //        disposal.StatusPA = ApplicationSettings.StatusDisposed;
        //        phonesRepository.UpdateStatusAsync(disposal.Imei, ApplicationSettings.StatusDisposed);
        //        disposal.Action = "Complete";
        //        return;
        //    }
        //    if (disposal.StatusPA == ApplicationSettings.StatusDisposed)
        //    {
        //        disposal.Action = ReconciliationConstants.Complete;
        //        return;
        //    }
        //}

        //if (disposal.StatusMS is null && disposal.StatusPA is null)
        //{
        //    disposal.Action = "Check CI linked to Disposal SR or add IMEI to Phones";
        //    return;
        //}

        //if (disposal.StatusMS == ApplicationSettings.StatusAwaitingReturn ||
        //    disposal.StatusMS == ApplicationSettings.StatusLost ||
        //    disposal.StatusMS == ApplicationSettings.StatusProduction ||
        //    disposal.StatusMS == ApplicationSettings.StatusUnlocated)
        //{
        //    disposal.Action = "Update phone status in myScomis";
        //}

        //if (disposal.StatusMS == ApplicationSettings.StatusDecommissioned && disposal.StatusPA == ApplicationSettings.StatusDecommissioned)
        //{
        //if (disposal.StatusSCC == null)
        //{
        //    disposal.Action = null;
        //    return;
        //}
        //    disposal.StatusPA = ApplicationSettings.StatusDisposed;
        //    phonesRepository.UpdateStatusAsync(disposal.Imei, ApplicationSettings.StatusDisposed);
        //    disposal.Action = "Update phone status in myScomis";
        //    return;
        //}

        //if (disposal.StatusMS == ApplicationSettings.StatusDecommissioned && disposal.StatusPA == ApplicationSettings.StatusDisposed)
        //{
        //    disposal.Action = "Update myScomis status to Disposed";
        //    return;
        //}

        //if (disposal.StatusMS == ApplicationSettings.StatusInStock)
        //{
        //    if (disposal.StatusPA is null)
        //    {
        //        disposal.Action = "Phone needs to be logged in PhoneAssistant";
        //        return;
        //    }
        //    if (disposal.StatusPA == ApplicationSettings.StatusInStock)
        //    {
        //        disposal.Action = null;
        //        return;
        //    }
        //}

        //if (disposal.StatusMS == ApplicationSettings.StatusProduction && disposal.StatusPA == ApplicationSettings.StatusProduction)
        //{
        //    disposal.Action = null;
        //    return;
        //}

        disposal.Action = "No matching reconcilation rule";
    }
}
