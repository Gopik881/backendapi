using FluentValidation;

namespace Elixir.Application.Features.Company.Queries.CheckCompanyCode;

public class CheckCompanyCodeExistsQueryValidator  : AbstractValidator<CheckCompanyCodeExistsQuery>
{
    public CheckCompanyCodeExistsQueryValidator()
    {
        RuleFor(x => x.CompanyCode)
            .NotEmpty().WithMessage("Company Code is required.");
    }
}
