using CommunityToolkit.Mvvm.ComponentModel;

namespace PhoneAssistant.WPF.Shared;

/// <summary>
/// Ensures view models with repositories load data
/// </summary>
public interface IViewModel
{
    Task LoadAsync();
}

/// <summary>
/// Base view model providing a default asynchronous load operation.
/// Inherit this when your view model needs the functionality of CommunityToolkit's ObservableObject.
/// </summary>
public abstract class ViewModelBase : ObservableObject, IViewModel
{
    public virtual Task LoadAsync() => Task.CompletedTask;
}

/// <summary>
/// Base view model for view models that require CommunityToolkit's ObservableValidator.
/// Provides a default LoadAsync implementation.
/// </summary>
public abstract class ViewModelValidatorBase : ObservableValidator, IViewModel
{
    public virtual Task LoadAsync() => Task.CompletedTask;
}
