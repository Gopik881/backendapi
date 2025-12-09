using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Clients.Commands.DeleteClient.ClientReportingToolLimit
{
    public record DeleteClientReportingToolLimitCommand(int ClientId) : IRequest<bool>;

    public class DeleteClientReportingToolLimitCommandHandler : IRequestHandler<DeleteClientReportingToolLimitCommand, bool>
    {
        private readonly IClientReportingToolLimitsRepository _clientReportingToolLimitRepository;

        public DeleteClientReportingToolLimitCommandHandler(IClientReportingToolLimitsRepository clientReportingToolLimitRepository)
        {
            _clientReportingToolLimitRepository = clientReportingToolLimitRepository;
        }

        public async Task<bool> Handle(DeleteClientReportingToolLimitCommand request, CancellationToken cancellationToken)
        {
            return await _clientReportingToolLimitRepository.DeleteClientReportingToolLimitsAsync(request.ClientId);
        }
    }
}
