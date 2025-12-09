using FluentValidation;

namespace Elixir.Application.Features.Telephone.Commands.BulkInsertTelephoneCodes
{
    public class BulkInsertTelephoneCodesCommandValidator : AbstractValidator<BulkInsertTelephoneCodesCommand>
    {

        public BulkInsertTelephoneCodesCommandValidator()
        {
            RuleFor(x => x.TelephoneCodes)
                .NotEmpty().WithMessage("Telephone codes list cannot be empty.");
            RuleForEach(x => x.TelephoneCodes).ChildRules(code =>
            {
                code.RuleFor(c => c.CountryName)
                    .NotEmpty().WithMessage("Country name is required.")
                    .MaximumLength(50).WithMessage("Country name cannot exceed 50 characters.");
                code.RuleFor(c => c.TelephoneCode)
                    .NotEmpty().WithMessage("Telephone code is required.")
                    .MaximumLength(5).WithMessage("Telephone code cannot exceed 5 characters.");
                code.RuleFor(c => c.Description)
                    .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");
            });
        }
    }
}
