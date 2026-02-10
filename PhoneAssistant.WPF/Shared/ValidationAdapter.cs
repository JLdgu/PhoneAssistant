using FluentValidation;
using System.Collections;
using System.ComponentModel;

namespace PhoneAssistant.WPF.Shared;

public sealed class ValidationAdapter<T> where T : class
{
    private readonly IValidator<T>? _validator;
    private readonly Dictionary<string, List<string>> _errors = new();

    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    public ValidationAdapter(IServiceProvider serviceProvider)
    {
        if (serviceProvider is null) throw new ArgumentNullException(nameof(serviceProvider));
        _validator = serviceProvider.GetService(typeof(IValidator<T>)) as IValidator<T>;
    }

    public bool HasErrors => _errors.Any(kv => kv.Value?.Count > 0);

    public IEnumerable GetErrors(string? propertyName)
    {
        if (string.IsNullOrEmpty(propertyName))
            return _errors.SelectMany(k => k.Value ?? new List<string>()).ToList();

        return _errors.TryGetValue(propertyName, out var list) ? list : Enumerable.Empty<string>();
    }

    public async Task ValidateAllAsync(T instance)
    {
        if (_validator is null)
        {
            _errors.Clear();
            RaiseErrorsChanged(null);
            return;
        }

        var result = await _validator.ValidateAsync(instance);
        _errors.Clear();
        foreach (var failure in result.Errors)
        {
            if (!_errors.ContainsKey(failure.PropertyName))
                _errors[failure.PropertyName] = new List<string>();
            _errors[failure.PropertyName].Add(failure.ErrorMessage);
        }

        foreach (var prop in _errors.Keys.ToList())
            RaiseErrorsChanged(prop);
    }

    public async Task ValidatePropertyAsync(T instance, string propertyName)
    {
        if (_validator is null)
        {
            _errors.Remove(propertyName);
            RaiseErrorsChanged(propertyName);
            return;
        }

        var result = await _validator.ValidateAsync(instance);

        var failures = result.Errors.Where(e => string.Equals(e.PropertyName, propertyName, StringComparison.OrdinalIgnoreCase)).ToList();

        if (failures.Any())
            _errors[propertyName] = failures.Select(f => f.ErrorMessage).ToList();
        else
            _errors.Remove(propertyName);

        RaiseErrorsChanged(propertyName);
    }

    private void RaiseErrorsChanged(string? propertyName)
    {
        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName ?? string.Empty));
    }
}
