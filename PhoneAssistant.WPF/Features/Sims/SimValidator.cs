using FluentValidation;
using PhoneAssistant.Model;
using PhoneAssistant.WPF.Shared;

namespace PhoneAssistant.WPF.Features.Sims;

public class SimValidator : AbstractValidator<SimsMainViewModel>
{
    private readonly IPhonesRepository _phonesRepository;

    public SimValidator(IPhonesRepository phonesRepository)
    {
        _phonesRepository = phonesRepository ?? throw new ArgumentNullException(nameof(phonesRepository));        

        RuleFor(x => x.NewUser)
            .NotEmpty().WithMessage("New User required");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone Number required")
            .PhoneNumberRules();

        RuleFor(x => x.SimNumber)
            .NotEmpty().WithMessage("SIM Number required")
            .SimNumberRules();

        RuleFor(x => x.Ticket)
            .NotEmpty().WithMessage("Ticket required")
            .TicketRules();
    }
}
