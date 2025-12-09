using FluentValidation;

namespace Elixir.Application.Features.Country.Queries.GetCountryWithStates;

public class GetCountryWithStatesQueryValidator : AbstractValidator<GetCountryWithStatesQuery>
{
    public GetCountryWithStatesQueryValidator()
    {
        // Validation rules for the GetCountryWithStatesQuery
        RuleFor(x => x.CountryId)
            .NotEmpty().WithMessage("Country ID is required.")
            .GreaterThan(0).WithMessage("Country ID must be greater than 0.");
    }
}
