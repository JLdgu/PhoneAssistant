namespace PhoneAssistant.WPF.Features.Disposals;

public class TrackProgress(int maximum)
{
    readonly int _modProgess = maximum < 1000 ? (int)Math.Ceiling((decimal)maximum / 100) : maximum / 100;

    public bool Milestone(int value)
    {
        return value % _modProgess == 0;
    }
}
