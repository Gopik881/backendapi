using Elixir.Application.Features.Clients.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

public record CreateClientReportingToolLimitCommand(int userId, int clientId, ReportingToolLimitsDto ClientReportingToolLimitDto) : IRequest<bool>;

public class CreateClientReportingToolLimitCommandHandler : IRequestHandler<CreateClientReportingToolLimitCommand, bool>
{
    private readonly IClientReportingToolLimitsRepository _clientReportingToolLimitRepository;

    public CreateClientReportingToolLimitCommandHandler(IClientReportingToolLimitsRepository clientReportingToolLimitRepository)
    {
        _clientReportingToolLimitRepository = clientReportingToolLimitRepository;
    }

    public async Task<bool> Handle(CreateClientReportingToolLimitCommand request, CancellationToken cancellationToken)
    {
        // Save data
        return await _clientReportingToolLimitRepository.CreateClientReportingToolLimitDataAsync(
            request.userId,
            request.clientId,
            request.ClientReportingToolLimitDto
        );
    }
}
