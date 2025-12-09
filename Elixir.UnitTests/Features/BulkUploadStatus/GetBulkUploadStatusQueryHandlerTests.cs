using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Common.DTOs;
using Elixir.Application.Features.BulkUploadStatus.GetBulkUploadStatus.Queries.GetBulkUploadStatusQuery;
using Elixir.Application.Interfaces.Persistance;
using Moq;
using Xunit;

public class GetBulkUploadStatusQueryHandlerTests
{
    [Fact]
    public async Task Handle_Should_Return_List_From_Repository()
    {
        var expected = new List<BulkUploadErrorListDto> { new BulkUploadErrorListDto() };
        var repoMock = new Mock<IBulkUploadErrorListRepository>();
        repoMock.Setup(r => r.GetBulkUploadErrorListAsync(It.IsAny<Guid>())).ReturnsAsync(expected);
        var handler = new GetBulkUploadStatusQueryHandler(repoMock.Object);

        var result = await handler.Handle(new GetBulkUploadStatusQuery(Guid.NewGuid()), CancellationToken.None);

        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task Handle_Should_Return_Empty_List_When_Repository_Returns_Empty()
    {
        var repoMock = new Mock<IBulkUploadErrorListRepository>();
        repoMock.Setup(r => r.GetBulkUploadErrorListAsync(It.IsAny<Guid>())).ReturnsAsync(new List<BulkUploadErrorListDto>());
        var handler = new GetBulkUploadStatusQueryHandler(repoMock.Object);

        var result = await handler.Handle(new GetBulkUploadStatusQuery(Guid.NewGuid()), CancellationToken.None);

        Assert.Empty(result);
    }
}