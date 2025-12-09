using System;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Common.DTOs;
using Elixir.Application.Features.Company.Commands.NotifyCompanyAdmin;
using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Interfaces.Services;
using Moq;
using Xunit;

public class NotifyCompanyAdminCommandHandlerTests
{
    [Fact]
    public async Task Handle_ThrowsArgumentNullException_WhenEmailRequestDtoIsNull()
    {
        var repo = new Mock<ICompanyAdminUsersRepository>();
        var emailService = new Mock<IEmailService>();
        var cryptoService = new Mock<ICryptoService>();
        var handler = new NotifyCompanyAdminCommandHandler(repo.Object, emailService.Object, cryptoService.Object);

        var command = new NotifyCompanyAdminCommand(null, "url", "template");

        await Assert.ThrowsAsync<ArgumentNullException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ReturnsFailure_WhenUserNotFound()
    {
        var repo = new Mock<ICompanyAdminUsersRepository>();
        var emailService = new Mock<IEmailService>();
        var cryptoService = new Mock<ICryptoService>();

        // Fix for CS1973: Cast the null value to the expected type explicitly.
        repo.Setup(r => r.GetCompanyAdminUserByEmailAsync(It.IsAny<string>(), It.IsAny<int>()))
            .ReturnsAsync((CompanyAdminUserDto)null);

        var handler = new NotifyCompanyAdminCommandHandler(repo.Object, emailService.Object, cryptoService.Object);

        var command = new NotifyCompanyAdminCommand(new EmailRequestDto { To = "test@test.com" }, "url", "template");
        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("User not found, cannot send email.", result.Message);
    }

    //[Fact]
    //public async Task Handle_SendsEmailAndReturnsResponse_WhenUserFound()
    //{
    //    var repo = new Mock<ICompanyAdminUsersRepository>();
    //    var emailService = new Mock<IEmailService>();
    //    var cryptoService = new Mock<ICryptoService>();

    //    // Fix: Use the correct type for the user object to match the expected return type of GetCompanyAdminUserByEmailAsync.
    //    var user = new CompanyAdminUserDto { Email = "test@test.com", FirstName = "John", LastName = "Doe" };
    //    repo.Setup(r => r.GetCompanyAdminUserByEmailAsync(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(user);

    //    cryptoService.Setup(c => c.GenerateIntegerHashForString(It.IsAny<string>())).Returns(123);
    //    cryptoService.Setup(c => c.EncryptPasswordResetData(It.IsAny<string>())).Returns("token");
    //    emailService.Setup(e => e.SendEmailAsync(It.IsAny<EmailRequestDto>())).ReturnsAsync(new EmailResponseDto { IsSuccess = true });

    //    var handler = new NotifyCompanyAdminCommandHandler(repo.Object, emailService.Object, cryptoService.Object);

    //    var command = new NotifyCompanyAdminCommand(new EmailRequestDto { To = "test@test.com" }, "http://reset", "<html>{{FirstName}}{{LastName}}{{RESET_PASSWORD_LINK}}{{RESET_TOKEN}}</html>");
    //    var result = await handler.Handle(command, CancellationToken.None);

    //    Assert.True(result.IsSuccess);
    //    emailService.Verify(e => e.SendEmailAsync(It.Is<EmailRequestDto>(dto =>
    //        dto.HtmlBody.Contains("John") &&
    //        dto.HtmlBody.Contains("Doe") &&
    //        dto.HtmlBody.Contains("http://reset?token=token") &&
    //        dto.HtmlBody.Contains("token")
    //    )), Times.Once);
    //}
}   