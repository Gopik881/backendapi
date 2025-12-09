using Elixir.Application.Features.UserGroup.DTOs;
using FluentValidation;

namespace Elixir.Application.Features.UserGroup.Commands.CreateUserGroupRights.CompositeHandler
{
    public class UserGroupCompositeCommandValidator : AbstractValidator<UserGroupCompositeCommand>
    {
        public UserGroupCompositeCommandValidator()
        {
            RuleFor(x => x)
                .NotNull()
                .WithMessage("Invalid user group data.");

            RuleFor(x => x.CreateUserGroupDto.UserGroupName)
                .NotEmpty()
                .WithMessage("GroupName should not be empty or contain only whitespace.")
                .Must(name => !string.IsNullOrWhiteSpace(name))
                .WithMessage("GroupName should not be empty or contain only whitespace.")
                .MaximumLength(50)
                .WithMessage("GroupName must not exceed 50 characters.");

            RuleFor(x => x.CreateUserGroupDto.Description)
                .MaximumLength(500) // varchar(MAX) in SQL, so no practical limit, but you can omit this or set a reasonable app-side limit if needed
                .WithMessage("Description must not exceed 500 characters.");
        }
    }
}
