namespace PhoneAssistant.WPF.Shared;

/// <summary>
/// Ensures view models with repositories load data
/// </summary>
public interface IViewModel
{
    Task LoadAsync();
}
