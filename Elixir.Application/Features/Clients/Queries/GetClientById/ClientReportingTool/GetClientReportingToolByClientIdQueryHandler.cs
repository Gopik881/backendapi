using Elixir.Application.Features.Clients.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

public record GetClientReportingToolByClientIdQuery(int clientId) : IRequest<ReportingToolLimitsDto?>;

public class GetClientReportingToolByClientIdQueryHandler : IRequestHandler<GetClientReportingToolByClientIdQuery, ReportingToolLimitsDto?>
{
    private readonly IClientReportingToolLimitsRepository _clientReportingToolLimitRepository;

    public GetClientReportingToolByClientIdQueryHandler(IClientReportingToolLimitsRepository clientReportingToolLimitRepository)
    {
        _clientReportingToolLimitRepository = clientReportingToolLimitRepository;
    }

    public async Task<ReportingToolLimitsDto?> Handle(GetClientReportingToolByClientIdQuery request, CancellationToken cancellationToken)
    {
        return await _clientReportingToolLimitRepository.GetClientReportingToolLimitDataAsync(request.clientId);
    }
}
