using FluentValidation;
namespace Elixir.Application.Features.User.Commands.Login;

public class LoginCommandValidator :AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        
        RuleFor(x => x.LoginReqDto.UserName)
             .NotEmpty().WithMessage("User Name is required.");
         RuleFor(x => x.LoginReqDto.Password)
             .NotEmpty().WithMessage("Password is required.");
    }
}
