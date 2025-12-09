using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Notification.DTOs;
using Elixir.Application.Features.Notification.Queries.GetAllNotifications;
using Elixir.Application.Interfaces.Persistance;
using Moq;
using Xunit;

public class GetAllUserNotificationsQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsAllNotifications()
    {
        var repoMock = new Mock<INotificationsRepository>();
        var expected = new List<NotificationDto> { new NotificationDto() };
        repoMock.Setup(r => r.GetAllUserNotificationsAsync(It.IsAny<int>())).ReturnsAsync(expected);

        var handler = new GetAllUserNotificationsQueryHandler(repoMock.Object);
        var result = await handler.Handle(new GetAllUserNotificationsQuery(1), CancellationToken.None);

        Assert.Equal(expected, result);
        repoMock.Verify(r => r.GetAllUserNotificationsAsync(1), Times.Once);
    }
}