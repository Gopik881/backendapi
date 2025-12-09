using Xunit;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Clients.Queries.GetClientById.GetClientCompositeCommand;
using Elixir.Application.Features.Clients.DTOs;
using MediatR;
using Elixir.Application.Features.Clients.Queries.GetClientById.Client;
using Elixir.Application.Features.Clients.Queries.GetClientById.ClientAccess;
using Elixir.Application.Features.Clients.Queries.GetClientById.ClientAccountManager;
using Elixir.Application.Features.Clients.Queries.GetClientById.ClientContact;

public class GetClientCompositeCommandHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsCombinedDto_WhenMediatorReturnsData()
    {
        var mediatorMock = new Mock<IMediator>();

        // Correctly setup the mock to return the expected types
        mediatorMock.Setup(m => m.Send(It.IsAny<GetClientDetailsByClientIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ClientInfoDto());

        mediatorMock.Setup(m => m.Send(It.IsAny<GetClientAccessByClientIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ClientAccessDto());

        mediatorMock.Setup(m => m.Send(It.IsAny<GetClientAccountManagerByClientIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ClientAccountManagersDto>());

        mediatorMock.Setup(m => m.Send(It.IsAny<GetClientAdminByClientIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ClientAdminInfoDto());

        mediatorMock.Setup(m => m.Send(It.IsAny<GetClientCompanyMappingByClientIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ClientCompanyMappingDto>());

        // Fix: Ensure the correct type is used for the mock setup
        mediatorMock.Setup(m => m.Send(It.IsAny<GetClientContactByClientIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ClientContactInfoDto>()); // Corrected type to match the expected return type

        // Fix: Correct the type mismatch for GetClientReportingToolByClientIdQuery
        mediatorMock.Setup(m => m.Send(It.IsAny<GetClientReportingToolByClientIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ReportingToolLimitsDto()); // Changed to match the expected return type

        var handler = new GetClientCompositeCommandHandler(mediatorMock.Object);
        var command = new GetClientCompositeCommand(1);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.IsType<CreateClientDto>(result);
    }
}