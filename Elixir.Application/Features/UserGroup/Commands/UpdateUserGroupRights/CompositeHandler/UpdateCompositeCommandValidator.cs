using FluentValidation;

namespace Elixir.Application.Features.UserGroup.Commands.UpdateUserGroupRights.CompositeHandler
{
    public class UpdateCompositeCommandValidator : AbstractValidator<UpdateCompositeCommand>
    {
        public UpdateCompositeCommandValidator()
        {
            RuleFor(x => x)
                .NotNull()
                .WithMessage("Invalid user group data.");

            RuleFor(x => x.UpdateCompositeDto.UserGroupName)
                .NotEmpty()
                .WithMessage("GroupName should not be empty or contain only whitespace.")
                .Must(name => !string.IsNullOrWhiteSpace(name))
                .WithMessage("GroupName should not be empty or contain only whitespace.")
                .MaximumLength(50)
                .WithMessage("GroupName must not exceed 50 characters.");

            RuleFor(x => x.UpdateCompositeDto.Description)
                .MaximumLength(500)
                .WithMessage("Description must not exceed 500 characters.");
        }
    }
}
