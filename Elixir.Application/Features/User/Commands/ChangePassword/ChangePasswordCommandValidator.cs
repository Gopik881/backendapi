using Elixir.Application.Features.User.Commands.ResetPassword;
using FluentValidation;

namespace Elixir.Application.Features.User.Commands.ChangePassword;

public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(x => x.ChangePasswordRequestDto.UserName)
            .NotEmpty().WithMessage("User name is required.")
            .MaximumLength(50).WithMessage("User name must not exceed 50 characters.");
        RuleFor(x => String.Equals(x.ChangePasswordRequestDto.NewPassword, x.ChangePasswordRequestDto.OldPassword))
            .Equal(false).WithMessage("New password must be different from the current password.").OverridePropertyName("NewPassword");
        //RuleFor(x => x.ChangePasswordRequestDto.OldPassword)
        //    .NotEmpty().WithMessage("Current password is required.")
        //    .MinimumLength(6).WithMessage("Current password must be at least 6 characters long.");
        //RuleFor(x => x.ChangePasswordRequestDto.NewPassword)
        //    .NotEmpty().WithMessage("New password is required.")
        //    .MinimumLength(6).WithMessage("New password must be at least 6 characters long.");
    }

}
