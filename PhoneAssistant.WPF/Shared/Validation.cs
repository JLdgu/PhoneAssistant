using PhoneAssistant.WPF.Features.AddItem;
using PhoneAssistant.WPF.Features.Sims;

using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace PhoneAssistant.WPF.Shared;

public static partial class Validation
{
    [return: MaybeNull]
    public static ValidationResult ValidateSimNumber(string? simNumber, ValidationContext context)
    {
        Regex regex = SimNumberFormat();

        if (string.IsNullOrEmpty(simNumber)) return ValidationResult.Success!;

        if (!regex.IsMatch(simNumber))
            return new ValidationResult("SIM Number must be 19 digits");

        return LuhnValidator.IsValid(simNumber, 19) ? ValidationResult.Success : new ValidationResult("SIM Number check digit incorrect");
    }

    [GeneratedRegex(@"8944\d{15}", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-GB")]
    private static partial Regex SimNumberFormat();

    [return: MaybeNull]
    public static ValidationResult ValidateTicket(string? ticket, ValidationContext context)
    {
        if (context.ObjectInstance is SimsMainViewModel && string.IsNullOrEmpty(ticket))
            return new ValidationResult("Ticket required");

        if (!string.IsNullOrEmpty(ticket))
        {
            if (int.TryParse(ticket, out int result))
            {
                if (result < 100000 || result > 9999999)
                    return new ValidationResult("Ticket must 6 or 7 digits");
            }
            else
            {
                return new ValidationResult("Ticket must 6 or 7 digits");
            }
        }
        if (context.ObjectInstance is AddItemViewModel vm)
        {
            if (vm.Status == "Decommissioned" || vm.Status == "Disposed")
                if (ticket is null)
                    return new ValidationResult("Ticket required when disposal");
        }

        return ValidationResult.Success;
    }
}
