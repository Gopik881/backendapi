using Elixir.Application.Common.Constants;
using Elixir.Application.Features.Clients.Commands.CreateClient.ClientAccountManagersData;
using Elixir.Application.Features.Clients.Commands.CreateClient.ClientContactData;
using Elixir.Application.Features.Clients.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Interfaces.Services;
using MediatR;

namespace Elixir.Application.Features.Clients.Commands.CreateClient.CompositeClient;


/// <summary>
/// Command to create a composite client with all related data.
/// </summary>
public record CompositeClientCommand(int UserId, int ClientId, CreateClientDto CreateClientDto, bool IsSuperUser) : IRequest<bool>;

/// <summary>
/// Handles the creation of a composite client, including all related entities and mappings.
/// </summary>
public class CreateCompositeClientCommandHandler : IRequestHandler<CompositeClientCommand, bool>
{
    private readonly Func<ITransactionRepository> _transactionRepositoryFactory;
    private readonly IMediator _mediator;
    public IClientsRepository _clientsRepository;
    public IClientCompaniesMappingRepository _clientCompaniesMappingRepository;
    public ICompaniesRepository _companiesRepository;

    public INotificationsRepository _notificationRepository { get; set; }
    public CreateCompositeClientCommandHandler(
        Func<ITransactionRepository> transactionRepositoryFactory,
        IMediator mediator,
        IClientsRepository clientsRepository,
        IClientCompaniesMappingRepository clientCompaniesMappingRepository,
        ICompaniesRepository companiesRepository,
        INotificationsRepository notificationRepository)
    {
        _transactionRepositoryFactory = transactionRepositoryFactory;
        _mediator = mediator;
        _clientsRepository = clientsRepository;
        _clientCompaniesMappingRepository = clientCompaniesMappingRepository;
        _companiesRepository = companiesRepository;
        _notificationRepository = notificationRepository;
    }

    /// <summary>
    /// Handles the composite client creation command.
    /// </summary>
    /// <param name="request">The composite client command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if successful, otherwise false.</returns>
    public async Task<bool> Handle(CompositeClientCommand request, CancellationToken cancellationToken)
    {
        //// 1. Check if client name already exists
        //if (await _clientsRepository.ExistsWithClientNameAsync(request.CreateClientDto.ClientInfo.ClientName))
        //    return false;

        //// 2. Check if client code already exists (if provided)
        //if (!string.IsNullOrEmpty(request.CreateClientDto.ClientInfo.ClientCode) &&
        //    await _clientsRepository.ExistsWithClientCodeAsync(request.CreateClientDto.ClientInfo.ClientCode))
        //    return false;

        // 3. Check if any company IDs are already mapped to a client
        var companyIds = request.CreateClientDto.clientCompanyMappingDtos?
            .Where(x => x.CompanyId != 0)
            .Select(x => x.CompanyId)
            .ToList();
        List<int> existingClientIds = new List<int>();
        if (companyIds != null && companyIds.Count > 0)
        {
            existingClientIds = await _clientsRepository.GetDistinctClientIdsByCompanyIdsAsync(companyIds);
            //if (existingClientIds != null && existingClientIds.Count > 0)
            //    return false;
        }

        // 4. Begin transaction
        using var transactionRepository = _transactionRepositoryFactory();
        await transactionRepository.BeginTransactionAsync();
        try
        {
            // To avoid DbContext concurrency issues, process each clientId sequentially and await each operation before starting the next.
            foreach (var clientId in existingClientIds)
            {
                //Update Client Details
                var updateClientDetails = new UpdateClientCommand(request.UserId,clientId,request.CreateClientDto.ClientInfo);
                var updateClientResult = await _mediator.Send(updateClientDetails, cancellationToken);
                if (!updateClientResult)
                    throw new Exception("Failed to Update client Detials data.");

                // 5. Create client access entry
                var clientAccessCommand = new CreateClientAccessCommand(request.UserId,clientId,request.CreateClientDto.ClientAccess);

                var clientAccessResult = await _mediator.Send(clientAccessCommand, cancellationToken);
                if (!clientAccessResult)
                    throw new Exception("Failed to save client access data.");

                // 6. Register client account managers
                var clientAccountManagerCommand = new CreateClientAccountManagerCommand(request.UserId,clientId,request.CreateClientDto.ClientAccountManagers);

                var clientAccountManagerResult = await _mediator.Send(clientAccountManagerCommand, cancellationToken);
                if (!clientAccountManagerResult)
                    throw new Exception("Failed to save client account manager data.");

                // 7. Store client administrator details
                var clientAdminCommand = new CreateClientAdminCommand(request.UserId,clientId,request.CreateClientDto.ClientAdminInfo);

                var clientAdminResult = await _mediator.Send(clientAdminCommand, cancellationToken);
                if (!clientAdminResult)
                    throw new Exception("Failed to save client administrator data.");

                // 8. Map client to company modules
                var clientCompanyMapCommand = new CreateClientCompanyMapCommand(request.UserId,clientId,request.CreateClientDto.clientCompanyMappingDtos ?? new List<ClientCompanyMappingDto>(), request.CreateClientDto.ClientInfo.ClientName);

                var clientCompanyMapResult = await _mediator.Send(clientCompanyMapCommand, cancellationToken);
                if (!clientCompanyMapResult)
                    throw new Exception("Failed to save client-company module mapping data.");

                if (request.CreateClientDto.ClientContactInfo != null)
                {
                    // 9. Save client contact information
                    var clientContactCommand = new CreateClientContactCommand(request.UserId,clientId,request.CreateClientDto.ClientContactInfo);

                    var clientContactResult = await _mediator.Send(clientContactCommand, cancellationToken);
                    if (!clientContactResult)
                        throw new Exception("Failed to save client contact information.");
                }

                if (request.CreateClientDto.ReportingToolLimits != null)
                {
                    // 10. Set reporting tool limits for the client
                    var clientReportingToolLimitCommand = new CreateClientReportingToolLimitCommand(request.UserId,clientId,request.CreateClientDto.ReportingToolLimits);

                    var clientReportingToolLimitResult = await _mediator.Send(clientReportingToolLimitCommand, cancellationToken);
                    if (!clientReportingToolLimitResult)
                        throw new Exception("Failed to apply reporting tool limits for the client.");
                }
            }
            await transactionRepository.CommitAsync();

            if (!await _companiesRepository.AddClientAccountManagersAsync(request.CreateClientDto, request.UserId, existingClientIds[0]))
                return false;

            return true;
        }
        catch(Exception ex)
        {
            // 12. Rollback transaction on error
            await transactionRepository.RollbackAsync();
            //return false;
            throw new Exception(AppConstants.ErrorCodes.CLIENT_CREATION_FAILURE);
        }
    }
}
