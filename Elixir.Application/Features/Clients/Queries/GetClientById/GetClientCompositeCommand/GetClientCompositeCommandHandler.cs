using Elixir.Application.Features.Clients.DTOs;
using Elixir.Application.Features.Clients.Queries.GetClientById.Client;
using Elixir.Application.Features.Clients.Queries.GetClientById.ClientAccess;
using Elixir.Application.Features.Clients.Queries.GetClientById.ClientAccountManager;
using Elixir.Application.Features.Clients.Queries.GetClientById.ClientContact;
using MediatR;

namespace Elixir.Application.Features.Clients.Queries.GetClientById.GetClientCompositeCommand;

public record GetClientCompositeCommand(int ClientId) : IRequest<CreateClientDto>;

public class GetClientCompositeCommandHandler : IRequestHandler<GetClientCompositeCommand, CreateClientDto>
{
    private readonly IMediator _mediator;

    public GetClientCompositeCommandHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<CreateClientDto> Handle(GetClientCompositeCommand request, CancellationToken cancellationToken)
    {
        // Get Client Data
        var clientData = await _mediator.Send(new GetClientDetailsByClientIdQuery(request.ClientId), cancellationToken);

        // Get Client Access Data
        var clientAccessData = await _mediator.Send(new GetClientAccessByClientIdQuery(request.ClientId), cancellationToken);

        // Get Client Account Manager Data
        var clientAccountManagerData = await _mediator.Send(new GetClientAccountManagerByClientIdQuery(request.ClientId), cancellationToken);

        // Get Client Admin Data
        var clientAdminData = await _mediator.Send(new GetClientAdminByClientIdQuery(request.ClientId), cancellationToken);

        // Get Client Company Map Data
        var clientCompanyMapData = await _mediator.Send(new GetClientCompanyMappingByClientIdQuery(request.ClientId), cancellationToken);

        // Get Client Contact Data
        var clientContactData = await _mediator.Send(new GetClientContactByClientIdQuery(request.ClientId), cancellationToken);

        // Get Client Reporting Tool Data
        var clientReportingToolData = await _mediator.Send(new GetClientReportingToolByClientIdQuery(request.ClientId), cancellationToken);

        // Combine all into CreateClientDto
        var result = new CreateClientDto
        {
            ClientInfo = clientData,
            ClientAccess = clientAccessData,
            ClientAccountManagers = clientAccountManagerData,
            ClientAdminInfo = clientAdminData,
            clientCompanyMappingDtos = clientCompanyMapData,
            ClientContactInfo = clientContactData,
            ReportingToolLimits = clientReportingToolData
        };

        return result;
    }
}
