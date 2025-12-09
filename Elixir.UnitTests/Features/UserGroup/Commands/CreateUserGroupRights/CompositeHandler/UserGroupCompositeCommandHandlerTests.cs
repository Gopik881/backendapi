//using System.Threading;
//using System.Threading.Tasks;
//using Xunit;
//using Moq;
//using Elixir.Application.Features.UserGroup.Commands.CreateUserGroupRights.CompositeHandler;
//using Elixir.Application.Interfaces.Persistance;
//using Elixir.Application.Features.UserGroup.DTOs;
//using MediatR;
//using System.Collections.Generic;
//using Elixir.Domain.Entities;

//public class UserGroupCompositeCommandHandlerTests
//{
//    [Fact]
//    public async Task Handle_ReturnsError_WhenUserGroupNameIsMissing()
//    {
//        var handler = new UserGroupCompositeCommandHandler(
//            () => Mock.Of<ITransactionRepository>(),
//            Mock.Of<IMediator>(),
//            Mock.Of<IUserGroupsRepository>()
//        );
//        var dto = new CreateUserGroupDto { UserGroupName = "" };
//        var result = await handler.Handle(new UserGroupCompositeCommand(dto), CancellationToken.None);
//        Assert.False(((Elixir.Application.Common.Models.ApiResponse<List<string>>)result).Success);
//    }

//    [Fact]
//    public async Task Handle_ReturnsError_WhenGroupNameNotAvailable()
//    {
//        var repoMock = new Mock<IUserGroupsRepository>();
//        repoMock.Setup(r => r.IsGroupNameAvailableAsync(It.IsAny<string>())).ReturnsAsync(false);
//        repoMock.Setup(r => r.CheckForDuplicateRightsAsync(It.IsAny<List<UserGroupMenuRights>>(), It.IsAny<int?>()))
//            .ReturnsAsync((UserGroup?)null);

//        var handler = new UserGroupCompositeCommandHandler(
//            () => Mock.Of<ITransactionRepository>(),
//            Mock.Of<IMediator>(),
//            repoMock.Object
//        );
//        var dto = new CreateUserGroupDto { UserGroupName = "Test" };
//        var result = await handler.Handle(new UserGroupCompositeCommand(dto), CancellationToken.None);
//        Assert.False(((Elixir.Application.Common.Models.ApiResponse<List<string>>)result).Success);
//    }

//    [Fact]
//    public async Task Handle_ReturnsError_WhenDuplicateRights()
//    {
//        var repoMock = new Mock<IUserGroupsRepository>();

//        repoMock.Setup(r => r.CheckForDuplicateRightsAsync(It.IsAny<List<UserGroupMenuRights>>(), It.IsAny<int?>()))
//            .ReturnsAsync(new UserGroup()); // Simulate duplicate found

//        var handler = new UserGroupCompositeCommandHandler(
//            () => Mock.Of<ITransactionRepository>(),
//            Mock.Of<IMediator>(),
//            repoMock.Object
//        );

//        var dto = new CreateUserGroupDto { UserGroupName = "Test" };
//        var result = await handler.Handle(new UserGroupCompositeCommand(dto), CancellationToken.None);

//        Assert.False(((Elixir.Application.Common.Models.ApiResponse<List<string>>)result).Success);
//    }

//    [Fact]
//    public async Task Handle_ReturnsTrue_WhenAllValid()
//    {
//        var repoMock = new Mock<IUserGroupsRepository>();
//        repoMock.Setup(r => r.IsGroupNameAvailableAsync(It.IsAny<string>())).ReturnsAsync(true);
//        repoMock.Setup(r => r.CheckForDuplicateRightsAsync(It.IsAny<List<UserGroupMenuRights>>(), It.IsAny<int?>()))
//            .ReturnsAsync((UserGroup?)null);

//        var mediatorMock = new Mock<IMediator>();
//        mediatorMock.Setup(m => m.Send(It.IsAny<object>(), It.IsAny<CancellationToken>())).ReturnsAsync(1);
//        mediatorMock.Setup(m => m.Send(It.IsAny<AddHorizontalsCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
//        mediatorMock.Setup(m => m.Send(It.IsAny<AddReportAccessCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
//        mediatorMock.Setup(m => m.Send(It.IsAny<AddReportingAdminsCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
//        mediatorMock.Setup(m => m.Send(It.IsAny<AddUserRightsCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

//        var transactionMock = new Mock<ITransactionRepository>();
//        transactionMock.Setup(t => t.BeginTransactionAsync()).Returns(Task.CompletedTask);
//        transactionMock.Setup(t => t.CommitAsync()).Returns(Task.CompletedTask);

//        var handler = new UserGroupCompositeCommandHandler(
//            () => transactionMock.Object,
//            mediatorMock.Object,
//            repoMock.Object
//        );
//        var dto = new CreateUserGroupDto
//        {
//            UserGroupName = "Test",
//            UserGroupMenuRights = new List<UserGroupMenuRights>(),
//            userGroupHorizontals = new List<UserGroupHorizontals>(),
//            reportingAccessDto = new ReportingAccessDto { Reports = new List<SelectionItemDto>() },
//            userGroupReportingAdmins = new List<UserGroupReportingAdmin>()
//        };
//        var result = await handler.Handle(new UserGroupCompositeCommand(dto), CancellationToken.None);
//        Assert.True((bool)result);
//    }
//}