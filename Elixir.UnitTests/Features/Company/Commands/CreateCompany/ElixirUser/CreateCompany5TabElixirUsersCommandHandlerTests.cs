using Xunit;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Features.Company.DTOs;

public class CreateCompany5TabElixirUsersCommandHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsTrue_WhenRepositoryReturnsTrue()
    {
        var repoMock = new Mock<IElixirUsersHistoryRepository>();
        repoMock.Setup(r => r.Company5TabCreateElixirUserDataAsync(
            It.IsAny<int>(),
            It.IsAny<List<Company5TabElixirUserDto>>(),
            It.IsAny<int>(),
            It.IsAny<CancellationToken>() // Explicitly pass CancellationToken instead of relying on optional arguments
        )).ReturnsAsync(true);

        var handler = new CreateCompany5TabElixirUsersCommandHandler(repoMock.Object);
        var command = new CreateCompany5TabElixirUsersCommand(1, 2, new List<Company5TabElixirUserDto>());

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task Handle_ReturnsFalse_WhenRepositoryReturnsFalse()
    {
        var repoMock = new Mock<IElixirUsersHistoryRepository>();
        repoMock.Setup(r => r.Company5TabCreateElixirUserDataAsync(
            It.IsAny<int>(),
            It.IsAny<List<Company5TabElixirUserDto>>(),
            It.IsAny<int>(),
            It.IsAny<CancellationToken>() // Explicitly pass CancellationToken instead of relying on optional arguments
        )).ReturnsAsync(false);

        var handler = new CreateCompany5TabElixirUsersCommandHandler(repoMock.Object);
        var command = new CreateCompany5TabElixirUsersCommand(1, 2, new List<Company5TabElixirUserDto>());

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result);
    }
}