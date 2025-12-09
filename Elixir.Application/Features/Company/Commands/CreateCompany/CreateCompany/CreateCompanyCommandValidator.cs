using FluentValidation;

namespace Elixir.Application.Features.Company.Commands.CreateCompany.CreateCompany;

public class CreateCompanyCommandValidator : AbstractValidator<CreateCompanyCommand>
{
    public CreateCompanyCommandValidator()
    {
        RuleFor(x => x.userId)
            .GreaterThan(0).WithMessage("UserId must be greater than 0.");

        RuleFor(x => x.CreateCompanyDto)
            .NotNull().WithMessage("CreateCompanyDto is required.");

        When(x => x.CreateCompanyDto != null, () =>
        {
            RuleFor(x => x.CreateCompanyDto.CompanyName)
                .NotEmpty().WithMessage("Company name is required.")
                .MaximumLength(50).WithMessage("Company name should not exceed 50 characters.");

        });
    }
}
