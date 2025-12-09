using Xunit;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Company.Commands.CreateCompany.CreateCompany;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Features.Notification.DTOs;
using Elixir.Application.Common.Models;
using System.Collections.Generic;
using Elixir.Application.Features.User.DTOs;

public class CreateCompanyCommandHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsError_WhenCompanyExists()
    {
        var repoMock = new Mock<ICompaniesRepository>();
        repoMock.Setup(r => r.ExistsWithCompanyNameAsync(It.IsAny<string>())).ReturnsAsync(true);

        var notificationsRepoMock = new Mock<INotificationsRepository>();
        var usersRepoMock = new Mock<IUsersRepository>();

        var handler = new CreateCompanyCommandHandler(repoMock.Object, notificationsRepoMock.Object, usersRepoMock.Object);
        var command = new CreateCompanyCommand(1, new CreateCompanyDto { CompanyName = "Test" });

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Contains("Duplicate company name.", result.Errors);
    }

    [Fact]
    public async Task Handle_ReturnsError_WhenCreateCompanyFails()
    {
        var repoMock = new Mock<ICompaniesRepository>();
        repoMock.Setup(r => r.ExistsWithCompanyNameAsync(It.IsAny<string>())).ReturnsAsync(false);

        // Fix for CS0854: Replace the invocation with a lambda expression to avoid optional arguments in expression trees.
        repoMock.Setup(r => r.CreateCompanyAsync(It.IsAny<int>(), It.IsAny<CreateCompanyDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(-1);

        var notificationsRepoMock = new Mock<INotificationsRepository>();
        var usersRepoMock = new Mock<IUsersRepository>();

        var handler = new CreateCompanyCommandHandler(repoMock.Object, notificationsRepoMock.Object, usersRepoMock.Object);
        var command = new CreateCompanyCommand(1, new CreateCompanyDto { CompanyName = "Test" });

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal(500, result.StatusCode);
        Assert.Contains("Database operation failed.", result.Errors);
    }

    //[Fact]
    //public async Task Handle_ReturnsSuccess_WhenCompanyCreated()
    //{
    //    var repoMock = new Mock<ICompaniesRepository>();
    //    repoMock.Setup(r => r.ExistsWithCompanyNameAsync(It.IsAny<string>())).ReturnsAsync(false);

    //    // Fix for CS0854: Replace the invocation with a lambda expression to avoid optional arguments in expression trees.
    //    repoMock.Setup(r => r.CreateCompanyAsync(It.IsAny<int>(), It.IsAny<CreateCompanyDto>(), It.IsAny<CancellationToken>()))
    //            .ReturnsAsync(123);

    //    var notificationsRepoMock = new Mock<INotificationsRepository>();
    //    notificationsRepoMock.Setup(n => n.InsertNotificationAsync(It.IsAny<NotificationDto>())).Returns((Task<bool>)Task.CompletedTask);

    //    var usersRepoMock = new Mock<IUsersRepository>();
    //    // Fix for CS0854: Use a lambda expression to avoid optional arguments in expression trees.
    //    usersRepoMock.Setup(u => u.GetUserProfileByUserIdAsync(It.IsAny<int>(), It.IsAny<bool?>()))
    //                 .ReturnsAsync(new UserProfileDto { FirstName = "John" });

    //    // Fix for CS0234: Ensure the namespace is correct and the type exists.
    //    usersRepoMock.Setup(u => u.GetAccountManagersAndCheckersUserIdsAsync(It.IsAny<int>())).ReturnsAsync(new List<int> { 1 });

    //    var handler = new CreateCompanyCommandHandler(repoMock.Object, notificationsRepoMock.Object, usersRepoMock.Object);
    //    var command = new CreateCompanyCommand(1, new CreateCompanyDto { CompanyName = "Test" });

    //    var result = await handler.Handle(command, CancellationToken.None);

    //    Assert.True(result.Success);
    //    Assert.Equal(201, result.StatusCode);
    //    Assert.True(result.Data);
    //}
}