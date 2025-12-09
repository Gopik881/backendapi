using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Common.Models;
using Elixir.Application.Features.Notification.DTOs;
using Elixir.Application.Features.Notification.Queries.GetPagedNotifications;
using Elixir.Application.Interfaces.Persistance;
using Moq;
using Xunit;

public class GetPagedNotificationsQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsPaginatedResponse()
    {
        var repoMock = new Mock<INotificationsRepository>();
        repoMock.Setup(r => r.GetFilteredNotificationsAsync(It.IsAny<int?>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string?>()))
            .ReturnsAsync(new Tuple<List<NotificationDto>, int>(new List<NotificationDto>(), 0));

        var handler = new GetPagedNotificationsQueryHandler(repoMock.Object);
        var result = await handler.Handle(new GetPagedNotificationsQuery(1, false, 1, 10, null), CancellationToken.None);

        Assert.NotNull(result);
        Assert.IsType<PaginatedResponse<NotificationDto>>(result);
    }
}