using FluentValidation;

namespace Elixir.Application.Features.Country.Commands.CreateCountry;

public class CreateCountryCommandValidator : AbstractValidator<CreateCountryCommand>
{
    public CreateCountryCommandValidator()
    {
        RuleFor(x => x.CreateCountryDto)
            .NotEmpty().WithMessage("At least one country must be provided.");
        RuleForEach(x => x.CreateCountryDto).ChildRules(country =>
        {
            country.RuleFor(c => c.CountryName)
                .NotEmpty().WithMessage("Country Name is required.")
                .MaximumLength(50).WithMessage("Country Name must not exceed 50 characters.");
            country.RuleFor(c => c.CountryShortName)
                .NotEmpty().WithMessage("Country short Name is required.")
                .MaximumLength(50).WithMessage("Country short Name must not exceed 50 characters.");
        });
    }
}
