using Elixir.Application.Common.DTOs;
using Elixir.Application.Features.User.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Interfaces.Services;
using MediatR;

namespace Elixir.Application.Features.User.Commands.SendPasswordResetEmail;

public record EmailUserSetPasswordCommand(EmailRequestDto EmailRequestDto, string passwordResetUrl, string passwordResetHTMLTemplate, string updatepasswordResetHTMLTemplate) : IRequest<EmailResponseDto>;


public class EmailUserSetPasswordCommandHandler : IRequestHandler<EmailUserSetPasswordCommand, EmailResponseDto>
{
    IUsersRepository _usersRepository;
    IEmailService _emailService;
    ICryptoService _cryptoService;
    public EmailUserSetPasswordCommandHandler(IUsersRepository usersRepository, IEmailService emailService, ICryptoService cryptoService)
    {
        _usersRepository = usersRepository;
        _emailService = emailService;
        _cryptoService = cryptoService;
    }

    public async Task<EmailResponseDto> Handle(EmailUserSetPasswordCommand request, CancellationToken cancellationToken)
    {
        if (request.EmailRequestDto == null)
        {
            throw new ArgumentNullException(nameof(request.EmailRequestDto), "Email request DTO cannot be null.");
        }

        // Retrieve the user by email and hash
        var user = await _usersRepository.GetUserByEmailAsync(request.EmailRequestDto.To, _cryptoService.GenerateIntegerHashForString(request.EmailRequestDto.To));
        if (user == null)
        {
            return new EmailResponseDto { IsSuccess = false, Message = "User not found, cannot send email." };  // User not found, cannot send email
        }

        if (string.IsNullOrWhiteSpace(request.EmailRequestDto.To))
            return new EmailResponseDto { IsSuccess = false, Message = "User not found, cannot send email." };

        // Attempt to clear any existing reset token. Only proceed if a non-empty token existed and was cleared.
        var cleared = await _usersRepository.SetResetPasswordTokenEmptyAsync(user.Email, string.Empty);
        if (!cleared)
        {
            // No existing token to clear or update failed -> do not proceed
            return new EmailResponseDto { IsSuccess = false, Message = "No existing reset request to replace or failed to clear existing token." };
        }


        //if (!String.IsNullOrEmpty(user.ResetPasswordToken))
        //{
        //    var tokenData = _cryptoService.DecryptPasswordResetData(user.ResetPasswordToken);
        //    if (tokenData != null && tokenData.ExpiryDate >= DateTime.UtcNow)
        //    {
        //        return new EmailResponseDto { IsSuccess = false, Message = "You already have a valid reset password request. Check your email." };
        //    }
        //}
        var resetToken = _cryptoService.EncryptPasswordResetData(user.Email);
        var resetLink = $"{request.passwordResetUrl}?token={resetToken}";
        var templateToUse = string.IsNullOrEmpty(user.PasswordHash)
            ? request.passwordResetHTMLTemplate
            : request.updatepasswordResetHTMLTemplate;

        var mailHtmlBody = templateToUse
            .Replace("{{FirstName}}", user.FirstName)
            .Replace("{{LastName}}", user.LastName)
            .Replace("{{RESET_PASSWORD_LINK}}", resetLink)
            .Replace("{{RESET_TOKEN}}", resetToken);

        var mailSendResponse = await _emailService.SendEmailAsync(new EmailRequestDto()
        {
            HtmlBody = mailHtmlBody,
            Subject = "Welcome to the ElixirHr Application!",
            To = user.Email,
        });

        if (mailSendResponse.IsSuccess)
        {
            await _usersRepository.SaveResetPasswordTokenAsync(_cryptoService.GenerateIntegerHashForString(user.Email), user.Email, resetToken);
        }

        // Send the email
        return mailSendResponse;
    }
}
