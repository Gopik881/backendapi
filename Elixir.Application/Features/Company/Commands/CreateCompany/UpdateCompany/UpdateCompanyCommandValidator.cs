using FluentValidation;

namespace Elixir.Application.Features.Company.Commands.CreateCompany.UpdateCompany;

public class UpdateCompanyCommandValidator : AbstractValidator<UpdateCompanyCommand>
{
    public UpdateCompanyCommandValidator()
    {
        RuleFor(x => x.companyId)
            .GreaterThan(0).WithMessage("CompanyId must be greater than 0.");

        RuleFor(x => x.userId)
            .GreaterThan(0).WithMessage("UserId must be greater than 0.");

        RuleFor(x => x.EditCompanyDto)
            .NotNull().WithMessage("EditCompanyDto is required.");

        When(x => x.EditCompanyDto != null, () =>
        {
            RuleFor(x => x.EditCompanyDto.CompanyName)
                .NotEmpty().WithMessage("Company name is required.")
                .MaximumLength(50).WithMessage("Company name must not exceed 50 characters.");

           

            // Add more rules for other properties of CreateCompanyDto as needed
        });
    }
}
