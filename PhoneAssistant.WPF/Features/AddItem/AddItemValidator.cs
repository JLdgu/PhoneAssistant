using FluentValidation;

using PhoneAssistant.Model;
using PhoneAssistant.WPF.Shared;

namespace PhoneAssistant.WPF.Features.AddItem;

public class AddItemValidator : AbstractValidator<AddItemViewModel>
{
    private readonly IPhonesRepository _phonesRepository;

    public AddItemValidator(IPhonesRepository phonesRepository)
    {
        _phonesRepository = phonesRepository ?? throw new ArgumentNullException(nameof(phonesRepository));

        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(model => model.AssetTag)
            .NotEmpty().WithMessage("Asset Tag required")
            .When(model => model.Status == ApplicationConstants.StatusInStock);

        RuleFor(model => model.AssetTag)
                .Length(7).WithMessage("Invalid format")
                .Matches(@"(MP|PC)\d{5}").WithMessage("Invalid format")
                .MustAsync(async (assetTag, cancellation) =>
                {
                    bool unique = await _phonesRepository.AssetTagUniqueAsync(assetTag);
                    return unique;
                }).WithMessage("Asset Tag must be unique")
                .When(model => !string.IsNullOrEmpty( model.AssetTag));

        RuleFor(model => model.Imei)
            .NotEmpty().WithMessage("IMEI required")
            .Length(15).WithMessage("IMEI must be 15 digits")
            .Matches(@"\d{15}").WithMessage("IMEI must be 15 digits")
            .Must(imei => LuhnValidator.IsValid(imei, 15)).WithMessage("IMEI check digit incorrect")
            .MustAsync(async (imei, cancellation) =>
            {
                bool exists = await _phonesRepository.ExistsAsync(imei);
                return !exists;
            }).WithMessage("IMEI must be unique");

        RuleFor(model => model.Model)
            .NotEmpty().WithMessage("Model required");

        RuleFor(x => x.PhoneNumber)
            .PhoneNumberRules()
            .When(model => !string.IsNullOrEmpty(model.PhoneNumber));

        RuleFor(x => x.SimNumber)
            .SimNumberRules()
            .When(model => !string.IsNullOrEmpty(model.SimNumber));

        RuleFor(model => model.Ticket)
            .NotEmpty().WithMessage("Ticket required")
            .When(model => model.Status == "Decommissioned" || model.Status == "Disposed");

        RuleFor(model => model.Ticket)
            .TicketRules()
            .When(model => !string.IsNullOrEmpty(model.Ticket));
    }
}
