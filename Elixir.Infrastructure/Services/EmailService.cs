using Azure;
using Azure.Communication.Email;
using Elixir.Application.Common.DTOs;
using Elixir.Application.Common.Models;
using Elixir.Application.Features.Authentication.Models;
using Elixir.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Elixir.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailConfigSettings _emailConfigSettings;
        public EmailService(IOptions<EmailConfigSettings> emailOptions)
        {
            _emailConfigSettings = emailOptions.Value ?? null;
        }

        public async Task<EmailResponseDto> SendEmailAsync(EmailRequestDto emailRequest)
        {
            var emailClient = new EmailClient(_emailConfigSettings.ConnectionString);
            var emailMessage = new EmailMessage(
                    senderAddress: _emailConfigSettings.SenderAddress,
                    content: new EmailContent(emailRequest.Subject)
                    {
                        PlainText = emailRequest.Subject,
                        Html = emailRequest.HtmlBody
                    },
                    recipients: new EmailRecipients(new List<EmailAddress> { new EmailAddress(emailRequest.To) })
                );
            try
            {
                await emailClient.SendAsync(WaitUntil.Completed, emailMessage);

                return new EmailResponseDto { IsSuccess = true, Message = "Email sent successfully." };
            }
            catch (Exception ex)
            {
                return new EmailResponseDto { IsSuccess = false, Message = $"Failed to send email: {ex.Message}" };
            }
        }
    }
}
