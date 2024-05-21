using PhoneAssistant.WPF.Application;
using PhoneAssistant.WPF.Application.Entities;

namespace PhoneAssistant.WPF.Features.Disposals;

public static class Reconciliation
{   
    public static void Execute(Disposal disposal)
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
