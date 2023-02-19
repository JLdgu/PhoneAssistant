using System.Threading.Tasks;

namespace PhoneAssistant.WPF.Shared;

/// <summary>
/// Use Interface rather than class as CommunityToolkit.MVVM objects
/// must be inherited as base classes
/// </summary>
public interface IViewModel
{
    //public abstract string ViewPackIcon { get; }

    //public abstract string ViewName { get; }

    public abstract Task LoadAsync();
}
