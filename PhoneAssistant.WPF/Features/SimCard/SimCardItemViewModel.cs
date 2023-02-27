using CommunityToolkit.Mvvm.ComponentModel;

namespace PhoneAssistant.WPF.Features.SimCard
{
    public sealed partial class SimCardItemViewModel : ObservableObject
    {
        private readonly Models.Sim _simCard;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public SimCardItemViewModel(Models.Sim simCard)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            _simCard = simCard;
            PhoneNumber = _simCard.PhoneNumber;
        }

        [ObservableProperty]
        private string _phoneNumber;
    }
}
