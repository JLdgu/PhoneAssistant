using System.Threading.Tasks;

namespace PhoneAssistant.WPF.ViewModels;

/// <summary>
/// Use Interface rather than class as CommunityToolkit.MVVM objects
/// must be inherited as base classes
/// </summary>
public interface IMainViewModel
{
    public abstract Task LoadAsync();
}
