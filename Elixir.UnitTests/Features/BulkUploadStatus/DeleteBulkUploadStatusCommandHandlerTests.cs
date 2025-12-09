using System;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.BulkUploadStatus.GetBulkUploadStatus.Command.DeleteBulkUploadStatus;
using Elixir.Application.Interfaces.Persistance;
using Moq;
using Xunit;

public class DeleteBulkUploadStatusCommandHandlerTests
{
    [Fact]
    public async Task Handle_Should_Call_Repository_And_Return_True_When_Delete_Succeeds()
    {
        var repoMock = new Mock<IBulkUploadErrorListRepository>();
        repoMock.Setup(r => r.DeleteBulkUploadErrorListAsync(It.IsAny<Guid>())).ReturnsAsync(true);
        var handler = new DeleteBulkUploadStatusCommandHandler(repoMock.Object);

        var result = await handler.Handle(new DeleteBulkUploadStatusCommand(Guid.NewGuid()), CancellationToken.None);

        Assert.True(result);
        repoMock.Verify(r => r.DeleteBulkUploadErrorListAsync(It.IsAny<Guid>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Return_False_When_Repository_Returns_False()
    {
        var repoMock = new Mock<IBulkUploadErrorListRepository>();
        repoMock.Setup(r => r.DeleteBulkUploadErrorListAsync(It.IsAny<Guid>())).ReturnsAsync(false);
        var handler = new DeleteBulkUploadStatusCommandHandler(repoMock.Object);

        var result = await handler.Handle(new DeleteBulkUploadStatusCommand(Guid.NewGuid()), CancellationToken.None);

        Assert.False(result);
    }
}