namespace PhoneAssistant.WPF.Shared;

/// <summary>
/// Use Interface rather than class as CommunityToolkit.MVVM objects
/// must be inherited as base classes
/// </summary>
public interface IViewModel
{
    Task LoadAsync();
    Task WindowClosingAsync();
}
