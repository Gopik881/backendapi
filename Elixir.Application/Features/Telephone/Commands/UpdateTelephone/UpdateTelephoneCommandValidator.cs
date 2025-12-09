using FluentValidation;

namespace Elixir.Application.Features.Telephone.Commands.UpdateTelephone;

public class UpdateTelephoneCommandValidator : AbstractValidator<UpdateTelephoneCommand>
{
    public UpdateTelephoneCommandValidator()
    {
        RuleFor(x => x.UpdateTelephoneCodeDto.TelephoneCode)
            .NotEmpty().WithMessage("Telephone code is required.")
            .MaximumLength(50).WithMessage("Telephone code must not exceed 50 characters.");
        RuleFor(x => x.UpdateTelephoneCodeDto.CountryId)
            .NotEmpty().WithMessage("Country Id is required.");
    }
}
