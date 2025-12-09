using FluentValidation;

namespace Elixir.Application.Features.State.Commands.UpdateState;
public class UpdateStateCommandValidator : AbstractValidator<UpdateStateCommand>
{
    public UpdateStateCommandValidator()
    {
        RuleFor(x => x.UpdateStateDto.StateName)
            .NotEmpty().WithMessage("State is required.")
            .MaximumLength(50).WithMessage("State name must not exceed 50 characters.");
        RuleFor(x => x.UpdateStateDto.StateShortName)
            .NotEmpty().WithMessage("State short name is required.")
            .MaximumLength(50).WithMessage("State short name must not exceed 50 characters.");
        RuleFor(x => x.UpdateStateDto.CountryId)
            .NotEmpty().WithMessage("Country Id is required.");

    }

}
