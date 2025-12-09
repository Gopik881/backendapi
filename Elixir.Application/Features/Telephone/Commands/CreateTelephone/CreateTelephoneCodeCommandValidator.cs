using FluentValidation;

namespace Elixir.Application.Features.Telephone.Commands.CreateTelephone;

public class CreateTelephoneCodeCommandValidator : AbstractValidator<CreateTelephoneCommand>
{
    public CreateTelephoneCodeCommandValidator()
    {
        RuleForEach(x => x.CreateTelephoneCodeDto)
            .ChildRules(dto =>
            {
                dto.RuleFor(y => y.TelephoneCode)
                    .NotEmpty().WithMessage("Telephone code is required.")
                    .MaximumLength(50).WithMessage("Telephone code must not exceed 50 characters.");
                dto.RuleFor(y => y.CountryId)
                    .NotEmpty().WithMessage("Country Id is required.");
            });
    }

}
