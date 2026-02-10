using FluentValidation;
using PhoneAssistant.Model;

namespace PhoneAssistant.WPF.Features.Sims;

public class SimValidator : AbstractValidator<SimsMainViewModel>
{
    private readonly IPhonesRepository _phonesRepository;

    public SimValidator(IPhonesRepository phonesRepository)
    {
        _phonesRepository = phonesRepository ?? throw new ArgumentNullException(nameof(phonesRepository));

        //RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.NewUser)
            .NotEmpty().WithMessage("New User required");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone Number required")
            .Length(10, 11).WithMessage("Phone Number must be 10 or 11 digits")
            .Matches(@"0\d{9,10}").WithMessage("Phone Number must be 10 or 11 digits");

        RuleFor(x => x.SimNumber)
            .NotEmpty().WithMessage("SIM Number required")
            .Length(12, 19).WithMessage("SIM Number must be 12 or 19 digits")
            .Matches(@"29\d{10}$|47\d{10}$|8944\d{15}$").WithMessage("SIM Number must be 12 or 19 digits");
        //293342802663
        //475334494637
        //8944125605569171710

        RuleFor(x => x.Ticket)
            .NotEmpty().WithMessage("Ticket required")
            .Length(6,7).WithMessage("Ticket must 6 or 7 digits")
            .Matches(@"\d{6,7}").WithMessage("Ticket must 6 or 7 digits");
    }
}
