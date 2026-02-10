using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections;
using System.ComponentModel;

namespace PhoneAssistant.WPF.Shared;

public abstract partial class ValidatableViewModel<T> : ObservableObject, IViewModel, INotifyDataErrorInfo where T : class
{
    private readonly ValidationAdapter<T> _adapter;

    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged
    {
        add => _adapter.ErrorsChanged += value;
        remove => _adapter.ErrorsChanged -= value;
    }

    protected ValidatableViewModel(IServiceProvider serviceProvider)
    {
        _adapter = new ValidationAdapter<T>(serviceProvider);
        _ = _adapter.ValidateAllAsync((T)(object)this);
    }

    public bool HasErrors => _adapter.HasErrors;

    public IEnumerable GetErrors(string? propertyName) => _adapter.GetErrors(propertyName);

    protected Task ValidateAllPropertiesAsync() => _adapter.ValidateAllAsync((T)(object)this);

    protected Task ValidatePropertyAsync(string propertyName) => _adapter.ValidatePropertyAsync((T)(object)this, propertyName);

    public virtual Task LoadAsync() => Task.CompletedTask;
}
