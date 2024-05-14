using PhoneAssistant.WPF.Application.Entities;

namespace PhoneAssistant.WPF.Features.Disposals;

public static class Reconciliation
{   
    public static void PAUpdate(Disposal update)
    {
        ArgumentNullException.ThrowIfNull(nameof(update));

        if (update.StatusMS == "Production" && update.StatusPA == "In Stock")
            update.Action = "Reconcile";
    }
}
