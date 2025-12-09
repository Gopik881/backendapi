using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Company.Commands.ApproveCompany.ElixirUser;
using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Moq;
using Xunit;

public class ApproveCompany5TabElixirUsersCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnTrue_WhenRepositoryReturnsTrue()
    {
        var repoMock = new Mock<IElixirUsersRepository>();
        repoMock.Setup(x => x.Company5TabApproveElixirUserDataAsync(
                It.IsAny<int>(),
                It.IsAny<List<Company5TabElixirUserDto>>(),
                It.IsAny<int>(),
                CancellationToken.None)) // Explicitly pass CancellationToken.None to avoid optional argument issue
            .ReturnsAsync(true);

        var handler = new ApproveCompany5TabElixirUsersCommandHandler(repoMock.Object);
        var command = new ApproveCompany5TabElixirUsersCommand(1, 2, new List<Company5TabElixirUserDto>());

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result);
    }   

    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenRepositoryReturnsFalse()
    {
        var repoMock = new Mock<IElixirUsersRepository>();
        repoMock.Setup(x => x.Company5TabApproveElixirUserDataAsync(
                It.IsAny<int>(),
                It.IsAny<List<Company5TabElixirUserDto>>(),
                It.IsAny<int>(),
                CancellationToken.None)) // Explicitly pass CancellationToken.None to avoid optional argument issue
            .ReturnsAsync(false);

        var handler = new ApproveCompany5TabElixirUsersCommandHandler(repoMock.Object);
        var command = new ApproveCompany5TabElixirUsersCommand(1, 2, new List<Company5TabElixirUserDto>());

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result);
    }
}