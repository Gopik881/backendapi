using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Company.Queries.GetCompanyPopupDetails;
using Elixir.Application.Interfaces.Persistance;
using Moq;
using Xunit;

// Fix for CS1929: Adjusting the type of 'expected' to match the return type of 'GetCompanyPopupDetailsByCompanyIdAsync'.
public class GetCompanyPopupDetailsByCompanyIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsDetails_WhenRepositoryReturnsData()
    {
        var mockRepo = new Mock<ICompaniesRepository>();
        var expected = new List<object>(); // Changed from 'object' to 'List<object>' to match the method's return type.
        mockRepo.Setup(r => r.GetCompanyPopupDetailsByCompanyIdAsync(1)).ReturnsAsync(expected);

        var handler = new GetCompanyPopupDetailsByCompanyIdQueryHandler(mockRepo.Object);
        var result = await handler.Handle(new GetCompanyPopupDetailsByCompanyIdQuery(1), CancellationToken.None);

        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task Handle_ReturnsNull_WhenRepositoryReturnsNull()
    {
        var mockRepo = new Mock<ICompaniesRepository>();
        mockRepo.Setup(r => r.GetCompanyPopupDetailsByCompanyIdAsync(2)).ReturnsAsync((IEnumerable<object>)null); // Explicitly casting to 'IEnumerable<object>'.

        var handler = new GetCompanyPopupDetailsByCompanyIdQueryHandler(mockRepo.Object);
        var result = await handler.Handle(new GetCompanyPopupDetailsByCompanyIdQuery(2), CancellationToken.None);

        Assert.Null(result);
    }
}