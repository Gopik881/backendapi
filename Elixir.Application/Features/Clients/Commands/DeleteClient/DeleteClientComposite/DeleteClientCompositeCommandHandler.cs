using Elixir.Application.Features.Clients.Commands.DeleteClient.ClientAccess;
using Elixir.Application.Features.Clients.Commands.DeleteClient.ClientAccountManager;
using Elixir.Application.Features.Clients.Commands.DeleteClient.ClientAdmin;
using Elixir.Application.Features.Clients.Commands.DeleteClient.ClientCompanyMapping;
using Elixir.Application.Features.Clients.Commands.DeleteClient.ClientContact;
using Elixir.Application.Features.Clients.Commands.DeleteClient.ClientInfo;
using Elixir.Application.Features.Clients.Commands.DeleteClient.ClientReportingToolLimit;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Clients.Commands.DeleteClient.DeleteClientComposite;

public record DeleteClientCompositeCommand(int ClientId) : IRequest<bool>;

public class DeleteClientCompositeCommandHandler : IRequestHandler<DeleteClientCompositeCommand, bool>
{
    private readonly Func<ITransactionRepository> _transactionRepositoryFactory;
    private readonly IClientsRepository clientsRepository;
    private readonly IMediator _mediator;
    public DeleteClientCompositeCommandHandler(
        Func<ITransactionRepository> transactionRepositoryFactory, IClientsRepository clientsRepository, IMediator mediator)
    {
        _transactionRepositoryFactory = transactionRepositoryFactory;
        this.clientsRepository = clientsRepository;
        _mediator = mediator;
    }

    public async Task<bool> Handle(DeleteClientCompositeCommand request, CancellationToken cancellationToken)
    {
        using var transactionRepository = _transactionRepositoryFactory();
        await transactionRepository.BeginTransactionAsync();
        try
        {
            // Get clientName from clientId
            var clientInfo = await clientsRepository.GetClientDetailsByIdAsync(request.ClientId);
            if (clientInfo == null)
                throw new Exception("Client not found.");

            var clientName = clientInfo.ClientName;
            if (string.IsNullOrWhiteSpace(clientName))
                throw new Exception("Client name not found.");

            // Get all clientIds with the same clientName
            var clientIds = await clientsRepository.GetListOfClientIdsByCompanyNameAsync(clientName);
            if (clientIds == null || clientIds.Count == 0)
                throw new Exception("No client IDs found for the client name.");

            foreach (var clientId in clientIds)
            {
                // Delete client account manager
                var accountManagerCommand = new DeleteClientAccountManagerCommand(clientId);
                if (!await _mediator.Send(accountManagerCommand))
                    throw new Exception($"Failed to delete client account manager for clientId {clientId}.");

                // Delete client admin
                var adminCommand = new DeleteClientAdminCommand(clientId);
                if (!await _mediator.Send(adminCommand))
                    throw new Exception($"Failed to delete client admin for clientId {clientId}.");

                // Delete client company mapping
                var companyMappingCommand = new DeleteClientCompanyMappingCommand(clientId);
                if (!await _mediator.Send(companyMappingCommand))
                    throw new Exception($"Failed to delete client company mapping for clientId {clientId}.");

                // Delete client contact
                var contactCommand = new DeleteClientContactCommand(clientId);
                if (!await _mediator.Send(contactCommand))
                    throw new Exception($"Failed to delete client contact for clientId {clientId}.");

                // Delete client reporting tool limit
                var reportingToolLimitCommand = new DeleteClientReportingToolLimitCommand(clientId);
                if (!await _mediator.Send(reportingToolLimitCommand))
                    throw new Exception($"Failed to delete client reporting tool limit for clientId {clientId}.");

                // Delete client access (should be last)
                var accessCommand = new DeleteClientAccessCommand(clientId);
                if (!await _mediator.Send(accessCommand))
                    throw new Exception($"Failed to delete client access for clientId {clientId}.");


                // Delete client info
                var deleteclientInfoCommand = new DeleteClientInfoCommand(clientId);
                if (!await _mediator.Send(deleteclientInfoCommand))
                    throw new Exception($"Failed to delete client info for clientId {clientId}.");
            }

            await transactionRepository.CommitAsync();
            return true;
        }
        catch (Exception ex)
        {
            await transactionRepository.RollbackAsync();
            throw new Exception($"Failed to delete client Error: ", ex);
            //return false;
        }
    }
}
