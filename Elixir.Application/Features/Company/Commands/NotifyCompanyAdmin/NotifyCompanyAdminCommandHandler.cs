using Elixir.Application.Common.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Interfaces.Services;
using MediatR;


namespace Elixir.Application.Features.Company.Commands.NotifyCompanyAdmin;

public record NotifyCompanyAdminCommand(EmailRequestDto EmailRequestDto,string passwordResetUrl, string passwordResetHTMLTemplate) : IRequest<EmailResponseDto>;


public class NotifyCompanyAdminCommandHandler : IRequestHandler<NotifyCompanyAdminCommand, EmailResponseDto>
{
    ICompanyAdminUsersRepository _companyAdminUsersRepository;
    IEmailService _emailService;
    ICryptoService _cryptoService;
    public NotifyCompanyAdminCommandHandler(ICompanyAdminUsersRepository companyAdminUsersRepository, IEmailService emailService, ICryptoService cryptoService)
    {
        _companyAdminUsersRepository = companyAdminUsersRepository;
        _emailService = emailService;
        _cryptoService = cryptoService;
    }
    public async Task<EmailResponseDto> Handle(NotifyCompanyAdminCommand request, CancellationToken cancellationToken)
    {
        if (request.EmailRequestDto == null)
        {
            throw new ArgumentNullException(nameof(request.EmailRequestDto), "Email request DTO cannot be null.");
        }
        
        // Retrieve the company admin user by email and hash
        var companyAdminUser = await _companyAdminUsersRepository.GetCompanyAdminUserByEmailAsync(request.EmailRequestDto.To, _cryptoService.GenerateIntegerHashForString(request.EmailRequestDto.To)); 
        if (companyAdminUser == null)
        {
            return new EmailResponseDto { IsSuccess = false, Message = "User not found, cannot send email." };  // User not found, cannot send email
        }
        var resetToken = _cryptoService.EncryptPasswordResetData(companyAdminUser.Email);
        var resetLink = $"{request.passwordResetUrl}?token={resetToken}";
        var mailHtmlBody = request.passwordResetHTMLTemplate
            .Replace("{{FirstName}}", companyAdminUser.FirstName)
            .Replace("{{LastName}}", companyAdminUser.LastName)
            .Replace("{{RESET_PASSWORD_LINK}}", resetLink)
            .Replace("{{RESET_TOKEN}}", resetToken);

        var mailSendResponse = await _emailService.SendEmailAsync(new EmailRequestDto()
        {
            HtmlBody = mailHtmlBody,
            Subject = "Welcome to the ElixirHr Application!",
            To = companyAdminUser.Email,
        });


        // Send the email
        return mailSendResponse;
    }
}
