using Elixir.Application.Common.DTOs;

namespace Elixir.Application.Interfaces.Services
{
    public interface IEmailService
    {
        Task<EmailResponseDto> SendEmailAsync(EmailRequestDto emailRequest);
    }
}
