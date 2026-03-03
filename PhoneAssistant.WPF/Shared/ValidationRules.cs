using FluentValidation;

namespace PhoneAssistant.WPF.Shared;

public static class ValidationRules
{
    public static IRuleBuilderOptions<T, string?> PhoneNumberRules<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("Phone Number required")
            .Length(10, 11).WithMessage("Phone Number must be 10 or 11 digits")
            .Matches(@"0\d{9,10}").WithMessage("Phone Number must be 10 or 11 digits");

    }
    public static IRuleBuilderOptions<T, string?> SimNumberRules<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("SIM Number required")
            .Length(12, 19).WithMessage("SIM Number must be 12 or 19 digits")
            .Matches(@"29\d{10}$|47\d{10}$|8944\d{15}$").WithMessage("SIM Number must be 12 or 19 digits");
    }

    public static IRuleBuilderOptions<T, string?> TicketRules<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder
            .Length(6, 7).WithMessage("Ticket must 6 or 7 digits")
            .Matches(@"\d{6,7}").WithMessage("Ticket must 6 or 7 digits");
    }
}
