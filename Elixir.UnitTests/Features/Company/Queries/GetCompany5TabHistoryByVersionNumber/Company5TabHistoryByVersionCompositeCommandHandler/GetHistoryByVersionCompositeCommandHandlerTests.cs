using Xunit;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Company.Queries.GetCompany5TabHistoryByVersionNumber.Company5TabHistoryByVersionCompositeCommandHandler;
using Elixir.Application.Features.Company.DTOs;
using MediatR;
using Elixir.Application.Features.Company.Queries.GetCompany5TabHistory.CompanyHistory;

public class GetHistoryByVersionCompositeCommandHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsCompanyHistory_WhenMediatorReturnsCompanyHistory()
    {
        var mediatorMock = new Mock<IMediator>();
        var expected = new Company5TabHistoryDto();
        mediatorMock.Setup(m => m.Send(It.IsAny<IRequest<Company5TabHistoryDto>>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(expected);

        var handler = new GetHistoryByVersionCompositeCommandHandler(mediatorMock.Object);
        var result = await handler.Handle(new GetCompositeCompany5TabHistoryCommand(1, 2, 3), CancellationToken.None);

        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task Handle_CallsMediatorWithCorrectQuery()
    {
        var mediatorMock = new Mock<IMediator>();
        var handler = new GetHistoryByVersionCompositeCommandHandler(mediatorMock.Object);
        var command = new GetCompositeCompany5TabHistoryCommand(4, 5, 6);

        await handler.Handle(command, CancellationToken.None);

        mediatorMock.Verify(m => m.Send(
            It.Is<GetCompany5TabCompanyHistoryQuery>(q => q.UserId == 4 && q.CompanyId == 5 && q.VersionNumber == 6),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}