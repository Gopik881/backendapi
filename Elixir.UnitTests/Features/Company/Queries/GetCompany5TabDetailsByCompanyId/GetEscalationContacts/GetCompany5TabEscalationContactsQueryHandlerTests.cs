using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Features.Company.Queries.GetCompany5TabDetailsByCompanyId.GetEscalationContacts;
using Elixir.Application.Interfaces.Persistance;
using Moq;
using Xunit;

public class GetCompany5TabEscalationContactsQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsEscalationContacts()
    {
        var repo = new Mock<IEscalationContactsHistoryRepository>();
        var expected = new List<Company5TabEscalationContactDto> { new Company5TabEscalationContactDto() };

        // Fix: Explicitly pass all arguments, including optional ones, to avoid CS0854
        repo.Setup(r => r.GetCompany5TabLatestEscalationContactsHistoryAsync(1, false, CancellationToken.None))
            .ReturnsAsync(expected);

        var handler = new GetCompany5TabEscalationContactsQueryHandler(repo.Object);
        var result = await handler.Handle(new GetCompany5TabEscalationContactsQuery(1, false), CancellationToken.None);

        Assert.Equal(expected, result);
    }
}