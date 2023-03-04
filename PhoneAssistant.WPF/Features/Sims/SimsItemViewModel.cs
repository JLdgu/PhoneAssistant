using CommunityToolkit.Mvvm.ComponentModel;

namespace PhoneAssistant.WPF.Features.Sims;

public sealed partial class SimsItemViewModel : ObservableObject
{
    private readonly Models.Sim _simCard;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public SimsItemViewModel(Models.Sim simCard)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
        _simCard = simCard;
        PhoneNumber = _simCard.PhoneNumber;
    }

    [ObservableProperty]
    private string _phoneNumber;
}
