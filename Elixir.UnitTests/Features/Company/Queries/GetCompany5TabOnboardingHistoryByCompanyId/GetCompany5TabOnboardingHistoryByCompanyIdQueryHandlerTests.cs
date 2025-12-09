using Xunit;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Company.Queries.GetCompany5TabOnboardingHistoryByCompanyId;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Features.Company.DTOs;

public class GetCompany5TabOnboardingHistoryByCompanyIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsList_WhenRepositoryReturnsList()
    {
        var repoMock = new Mock<ICompany5TabOnboardingHistoryRepository>();
        var expected = new List<Company5TabOnboardingHistoryDto> { new Company5TabOnboardingHistoryDto() };
        repoMock.Setup(r => r.GetCompany5TabOnboardingHistoryByCompanyIdAsync(It.Is<int>(id => id == 1), false))
                .ReturnsAsync(expected);

        var handler = new GetCompany5TabOnboardingHistoryByCompanyIdQueryHandler(repoMock.Object);
        var result = await handler.Handle(new GetCompany5TabOnboardingHistoryByCompanyIdQuery(1), CancellationToken.None);

        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task Handle_CallsRepositoryWithCorrectParameter()
    {
        var repoMock = new Mock<ICompany5TabOnboardingHistoryRepository>();
        var handler = new GetCompany5TabOnboardingHistoryByCompanyIdQueryHandler(repoMock.Object);
        var query = new GetCompany5TabOnboardingHistoryByCompanyIdQuery(2);

        await handler.Handle(query, CancellationToken.None);

        repoMock.Verify(r => r.GetCompany5TabOnboardingHistoryByCompanyIdAsync(It.Is<int>(id => id == 2), false), Times.Once);
    }
}