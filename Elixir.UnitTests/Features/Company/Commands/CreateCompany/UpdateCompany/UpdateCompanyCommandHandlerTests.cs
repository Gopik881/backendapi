using Xunit;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Elixir.Application.Features.Company.Commands.CreateCompany.UpdateCompany;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Features.Notification.DTOs;
using Elixir.Application.Features.User.DTOs;

public class UpdateCompanyCommandHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsFalse_WhenCompanyExists()
    {
        var usersRepoMock = new Mock<IUsersRepository>(); // Declare and initialize usersRepoMock before using it
        usersRepoMock.Setup(u => u.GetUserProfileByUserIdAsync(It.IsAny<int>(), It.IsAny<bool?>()))
            .ReturnsAsync(new UserProfileDto { FirstName = "John" });

        var repoMock = new Mock<ICompaniesRepository>();
        repoMock.Setup(r => r.ExistsWithCompanyNameForUpdateAsync(It.IsAny<string>(), It.IsAny<int>()))
            .ReturnsAsync(true);

        var notificationsRepoMock = new Mock<INotificationsRepository>();

        var handler = new UpdateCompanyCommandHandler(repoMock.Object, notificationsRepoMock.Object, usersRepoMock.Object);
        var command = new UpdateCompanyCommand(1, 2, new CreateCompanyDto { CompanyName = "Test" });

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result);
    }

    //[Fact]
    //public async Task Handle_ReturnsTrue_WhenUpdateSucceeds()
    //{
    //    var repoMock = new Mock<ICompaniesRepository>();
    //    repoMock.Setup(r => r.ExistsWithCompanyNameForUpdateAsync(It.IsAny<string>(), It.IsAny<int>()))
    //        .ReturnsAsync(false);
    //    repoMock.Setup(r => r.UpdateCompanyAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CreateCompanyDto>(), CancellationToken.None))
    //        .ReturnsAsync(true);

    //    var notificationsRepoMock = new Mock<INotificationsRepository>();
    //    notificationsRepoMock.Setup(n => n.InsertNotificationAsync(It.IsAny<NotificationDto>()))
    //        .Returns((Task<bool>)Task.CompletedTask);

    //    var usersRepoMock = new Mock<IUsersRepository>();
    //    usersRepoMock.Setup(u => u.GetUserProfileByUserIdAsync(It.IsAny<int>(), null)) // Fix: Explicitly pass null for optional argument
    //        .ReturnsAsync(new UserProfileDto { FirstName = "John" });
    //    usersRepoMock.Setup(u => u.GetAccountManagersAndCheckersUserIdsAsync(It.IsAny<int>()))
    //        .ReturnsAsync(new List<int> { 1 });

    //    var handler = new UpdateCompanyCommandHandler(repoMock.Object, notificationsRepoMock.Object, usersRepoMock.Object);
    //    var command = new UpdateCompanyCommand(1, 2, new CreateCompanyDto { CompanyName = "Test" });

    //    var result = await handler.Handle(command, CancellationToken.None);

    //    Assert.True(result);
    //}
}