using Elixir.Application.Features.Clients.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

public record UpdateClientReportingToolLimitCommand(int userId, int clientId, ReportingToolLimitsDto ClientReportingToolLimitDto) : IRequest<bool>;

public class UpdateClientReportingToolLimitCommandHandler : IRequestHandler<UpdateClientReportingToolLimitCommand, bool>
{
    private readonly IClientReportingToolLimitsRepository _clientReportingToolLimitRepository;

    public UpdateClientReportingToolLimitCommandHandler(IClientReportingToolLimitsRepository clientReportingToolLimitRepository)
    {
        _clientReportingToolLimitRepository = clientReportingToolLimitRepository;
    }

    public async Task<bool> Handle(UpdateClientReportingToolLimitCommand request, CancellationToken cancellationToken)
    {
        // Update data
        return await _clientReportingToolLimitRepository.UpdateClientReportingToolLimitDataAsync(
            request.userId,
            request.clientId,
            request.ClientReportingToolLimitDto
        );
    }
}
