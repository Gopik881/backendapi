using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Common.DTOs;
using Elixir.Application.Common.Models;
using Elixir.Application.Features.BulkUploadStatus.GetBulkUploadStatus.Queries.GetPagedBulkUploadStatusQuery;
using Elixir.Application.Interfaces.Persistance;
using Moq;
using Xunit;

public class GetPagedBulkUploadStatusQueryHandlerTests
{
    [Fact]
    public async Task Handle_Should_Return_PaginatedResponse()
    {
        var items = new List<BulkUploadErrorListDto> { new BulkUploadErrorListDto() };
        var totalCount = 10;
        var repoMock = new Mock<IBulkUploadErrorListRepository>();
        repoMock.Setup(r => r.GetPagedBulkUploadErrorListAsync(It.IsAny<Guid>(), 1, 5))
            .ReturnsAsync(new Tuple<List<BulkUploadErrorListDto>, int>(items, totalCount));
        var handler = new GetPagedBulkUploadStatusQueryHandler(repoMock.Object);

        var result = await handler.Handle(new GetPagedBulkUploadStatusQuery(Guid.NewGuid(), 1, 5), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(items, result.Data);
        Assert.Equal(totalCount, result.Metadata.TotalItems);
    }
}