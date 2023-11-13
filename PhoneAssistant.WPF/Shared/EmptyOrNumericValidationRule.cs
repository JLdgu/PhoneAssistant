using System.Globalization;
using System.Windows.Controls;

namespace PhoneAssistant.WPF.Shared;

public class EmptyOrNumericValidationRule : ValidationRule
{
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        if (value is null) return ValidationResult.ValidResult;

        if (value is not string) new ValidationResult(false, "Field must be empty or numeric");

        string stringValue = value.ToString() ?? "";

        return string.IsNullOrEmpty(stringValue)
            ? ValidationResult.ValidResult
            : Int64.TryParse(stringValue, out _)
            ? ValidationResult.ValidResult
            : new ValidationResult(false, "Field must be empty or numeric");
    }
}
