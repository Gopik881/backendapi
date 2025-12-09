using Elixir.Application.Common.Constants;
using Elixir.Application.Features.Clients.Commands.UpdateClient.UpdateClient.ClientAccess;
using Elixir.Application.Features.Clients.Commands.UpdateClient.UpdateClient.ClientAccountManagers;
using Elixir.Application.Features.Clients.Commands.UpdateClient.UpdateClient.ClientContact;
using Elixir.Application.Features.Clients.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Clients.Commands.UpdateClient.UpdateClient.UpdateClientCompositeCommand;

/// <summary>
/// Command to update a composite client with all related data.
/// </summary>
public record UpdateClientCompositeCommand(int UserId, int ClientId, CreateClientDto UpdateClientDto, bool IsSuperUser) : IRequest<bool>;

/// <summary>
/// Handles the update of a composite client, including all related entities and mappings.
/// </summary>
public class UpdateClientCompostieCommandHandler : IRequestHandler<UpdateClientCompositeCommand, bool>
{
    private readonly Func<ITransactionRepository> _transactionRepositoryFactory;
    private readonly IMediator _mediator;
    private readonly IClientsRepository _clientsRepository;
    private readonly IClientCompaniesMappingRepository _clientCompaniesMappingRepository;
    private readonly ICompaniesRepository _companiesRepository;
    private readonly IElixirUsersRepository _usersElixirUsersRepository;
    public readonly INotificationsRepository _notificationRepository;
    public UpdateClientCompostieCommandHandler(
        Func<ITransactionRepository> transactionRepositoryFactory,
        IMediator mediator,
        IClientsRepository clientsRepository,
        IClientCompaniesMappingRepository clientCompaniesMappingRepository,
        ICompaniesRepository companiesRepository,
        IElixirUsersRepository elixirUserRepository,
        INotificationsRepository notificationRepository)
    {
        _transactionRepositoryFactory = transactionRepositoryFactory;
        _mediator = mediator;
        _clientsRepository = clientsRepository;
        _clientCompaniesMappingRepository = clientCompaniesMappingRepository;
        _companiesRepository = companiesRepository;
        _usersElixirUsersRepository = elixirUserRepository;
        _notificationRepository = notificationRepository;
    }

    /// <inheritdoc />
    public async Task<bool> Handle(UpdateClientCompositeCommand request, CancellationToken cancellationToken)
    {
        var clientInfo = request.UpdateClientDto.ClientInfo;
        var clientId = request.ClientId;

        //// 1. Check if client name already exists (excluding current client)
        //if (await _clientsRepository.ExistsWithClientNameAsync(clientInfo.ClientName) && clientInfo.ClientId != clientId)
        //    return false;

        //// 2. Check if client code already exists (excluding current client)
        //if (!string.IsNullOrEmpty(clientInfo.ClientCode))
        //{
        //    var clientCodeExists = await _clientsRepository.ExistsWithClientCodeAsync(clientInfo.ClientCode);
        //    if (clientCodeExists && clientInfo.ClientId != clientId)
        //        return false;
        //}

        // 3. Check if any company IDs are already mapped to a different client
        var companyIds = request.UpdateClientDto.clientCompanyMappingDtos?
            .Where(x => x.CompanyId != 0)
            .Select(x => x.CompanyId)
            .ToList();

        List<int> existingClientIds = new();
        if (companyIds is { Count: > 0 })
        {
            existingClientIds = await _clientsRepository.GetDistinctClientIdsByCompanyIdsAsync(companyIds);
            //if (existingClientIds.Any(id => id != clientId))
            //    return false;
            // If no existingClientIds, take clientIds based on clientName from dto
            
        }
        if (existingClientIds.Count == 0 && !string.IsNullOrWhiteSpace(request.UpdateClientDto.ClientInfo?.ClientName))
        {
            existingClientIds = await _clientsRepository.GetListOfClientIdsByCompanyNameAsync(request.UpdateClientDto.ClientInfo.ClientName);
        }

        // 4. Begin transaction
        using var transactionRepository = _transactionRepositoryFactory();
        await transactionRepository.BeginTransactionAsync();
        try
        {
            // If no existingClientIds, ensure at least the current client is updated
            if (existingClientIds.Count == 0)
                existingClientIds.Add(clientId);

            foreach (var id in existingClientIds.Distinct())
            {
                if (clientInfo != null)
                {
                    // 5. Update client entry
                    var updateClientCommand = new UpdateClientCommand(request.UserId, id, clientInfo);
                    if (!await _mediator.Send(updateClientCommand, cancellationToken))
                        throw new Exception("Failed to update client data.");
                }

                if (request.UpdateClientDto.ClientAccess != null)
                {
                    // 6. Update client access entry
                    var updateClientAccessCommand = new UpdateClientAccessCommand(request.UserId, id, request.UpdateClientDto.ClientAccess);
                    if (!await _mediator.Send(updateClientAccessCommand, cancellationToken))
                        throw new Exception("Failed to update client access data.");
                }

                if (request.UpdateClientDto.ClientAdminInfo != null)
                {
                    // 8. Update client administrator details
                    var updateClientAdminCommand = new UpdateClientAdminCommand(request.UserId, id, request.UpdateClientDto.ClientAdminInfo);
                    if (!await _mediator.Send(updateClientAdminCommand, cancellationToken))
                        throw new Exception("Failed to update client administrator data.");
                }
                if(request.UpdateClientDto.ClientAccountManagers != null && request.UpdateClientDto.ClientAccountManagers.Count > 0)
                {
                    // 7. Update client account managers
                    var updateClientAccountManagerCommand = new UpdateClientAccountManagerCommand(request.UserId, id, request.UpdateClientDto.ClientAccountManagers);
                    if (!await _mediator.Send(updateClientAccountManagerCommand, cancellationToken))
                        throw new Exception("Failed to update client account manager data.");
                }

                if (request.UpdateClientDto.clientCompanyMappingDtos != null)
                {
                    // 9. Update client-company module mappings
                    var updateClientCompanyMappingCommand = new UpdateClientCompanyMappingCommand(request.UserId, id, request.UpdateClientDto.clientCompanyMappingDtos, request.UpdateClientDto.ClientInfo.ClientName, request.IsSuperUser);
                    if (!await _mediator.Send(updateClientCompanyMappingCommand, cancellationToken))
                        throw new Exception("Failed to update client-company module mapping data.");
                }

                if (request.UpdateClientDto.ClientContactInfo != null)
                {
                    // 10. Update client contact information
                    var updateClientContactCommand = new UpdateClientContactCommand(request.UserId, id, request.UpdateClientDto.ClientContactInfo);
                    if (!await _mediator.Send(updateClientContactCommand, cancellationToken))
                        throw new Exception("Failed to update client contact information.");
                }

                if (request.UpdateClientDto.ReportingToolLimits != null)
                {
                    // 11. Update reporting tool limits for the client
                    var updateClientReportingToolCommand = new UpdateClientReportingToolLimitCommand(request.UserId, id, request.UpdateClientDto.ReportingToolLimits);
                    if (!await _mediator.Send(updateClientReportingToolCommand, cancellationToken))
                        throw new Exception("Failed to update reporting tool limits for the client.");
                }
            }


            // Remove client-company mappings and update client/company names if needed (before transaction)
            var clientIdsList = await _clientsRepository.GetDistinctClientIdsByCompanyIdsAsync(
                request.UpdateClientDto.ClientInfo.ClientName != null
                    ? new List<int> { request.UpdateClientDto.ClientInfo.ClientId }
                    : new List<int>()
            );

            var ClientexistingIds = await _clientsRepository.GetListOfClientIdsByCompanyNameAsync(request.UpdateClientDto.ClientInfo.ClientName);
            foreach(var cId in ClientexistingIds)
            {
                if (clientInfo != null)
                {
                    // 5. Update client entry
                    var updateClientCommand = new UpdateClientCommand(request.UserId, cId, clientInfo);
                    if (!await _mediator.Send(updateClientCommand, cancellationToken))
                        throw new Exception("Failed to update client data.");
                }
            }

            // 7. Replace client account managers

            var replaced = await _usersElixirUsersRepository.ReplaceClientAccountManagersAsync(request.UserId,request.ClientId,request.UpdateClientDto.ClientAccountManagers, cancellationToken);
            if (!replaced)
                throw new Exception("Failed to replace client account manager data.");

            await transactionRepository.CommitAsync();

           
            if (!await _companiesRepository.AddClientAccountManagersAsync(request.UpdateClientDto, request.UserId, existingClientIds[0]))
                return false;


            return true;
        }
        catch(Exception ex)
        {
            await transactionRepository.RollbackAsync();            
            throw new Exception(AppConstants.ErrorCodes.CLIENT_UPDATE_FAILED);
        }
    }
}
