using System.Globalization;
using System.Windows.Controls;

namespace PhoneAssistant.WPF.Shared;

public class IMEIValidationRule : ValidationRule
{
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        if (value is null) return ValidationResult.ValidResult;

        if (value is not string) return new ValidationResult(false, "IMEI must be empty or 15 digits");

        string? imei = value.ToString();

        if (string.IsNullOrWhiteSpace(imei)) return ValidationResult.ValidResult;

        bool luhn = LuhnValidator.IsValid(imei, 15);

        return luhn ? ValidationResult.ValidResult : new ValidationResult(false, "IMEI must be empty or 15 digits");
    }
}

public class RequiredIMEIValidationRule : ValidationRule
{
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        if (value is not string) return new ValidationResult(false, "IMEI must be 15 digits");

        string? imei = value.ToString();

        if (string.IsNullOrWhiteSpace(imei)) return new ValidationResult(false, "IMEI must be 15 digits");

        bool luhn = LuhnValidator.IsValid(imei, 15);

        return luhn ? ValidationResult.ValidResult : new ValidationResult(false, "IMEI must be 15 digits");
    }
}

public static  class LuhnValidator
{
    /// <summary>
    /// This class uses the Luhn algorithm to validate a string
    /// </summary>
    /// <param name="luhn">IMEI/SIM card number</param>
    /// 
    /// <param name="length">The expected length of the luhn parameter</param>
    /// <returns>bool</returns>
    public static bool IsValid(string? luhn, int length = -1)
    {
        if (luhn is null)
            return false;

        if (luhn.Length == 1)
            return false;

        if (length == 0 || length == 1)
            return false;
        
        if (length > 1 && luhn.Length != length)
            return false;

        if (!long.TryParse(luhn, out _))
            return false;

        int inputCheckDigit = int.Parse(luhn.Substring(luhn.Length -1 , 1));        

        int sum = 0;
        bool parityDigit = luhn.Length % 2 == 0; 
        for (int index = 0; index < luhn.Length - 1; index++)
        //bool parityDigit = true;
        //for (int index = luhnString.Length - 2; index >= 0; index--)
        {
            int digit = int.Parse(luhn.Substring(index, 1));

            if (parityDigit)
            {
                digit *= 2;

                if (digit > 9)
                {
                    digit = (digit % 10) + 1;
                }
            }
            sum += digit;
            parityDigit = !parityDigit;
        }
        int calculatedCheckDigit = (10 - (sum % 10)) % 10;

        return (inputCheckDigit == calculatedCheckDigit);
    }
}
