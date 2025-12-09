using FluentValidation;

namespace Elixir.Application.Features.Country.Commands.DeleteCountry;
public class DeleteCountryCommandValidator : AbstractValidator<DeleteCountryCommand>
{
    public DeleteCountryCommandValidator()
    {
        RuleFor(x => x.CountryId)
            .GreaterThan(0).WithMessage("Country ID must be greater than zero."); 
    }

}
