using FluentValidation;


namespace Elixir.Application.Features.Country.Commands.UpdateCountry;

public class UpdateCountryCommandValidator:AbstractValidator<UpdateCountryCommand>
{
    public UpdateCountryCommandValidator()
    {
        RuleFor(x => x.UpdateCountryDto.CountryName)
            .NotEmpty().WithMessage("Country is required.")
            .MaximumLength(50).WithMessage("Country name must not exceed 50 characters.");

        RuleFor(x => x.UpdateCountryDto.CountryShortName)
            .NotEmpty().WithMessage("Country short name is required.")
            .MaximumLength(50).WithMessage("Country short name must not exceed 50 characters.");
    }
}
