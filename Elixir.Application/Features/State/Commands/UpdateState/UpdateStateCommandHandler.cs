using Elixir.Application.Common.Constants;
using Elixir.Application.Features.State.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Interfaces.Services;
using MediatR;

namespace Elixir.Application.Features.State.Commands.UpdateState;
public record UpdateStateCommand(int StateId, CreateUpdateStateDto UpdateStateDto) : IRequest<bool>;

public class UpdateStateCommandHandler : IRequestHandler<UpdateStateCommand, bool>
{
    private readonly IStateMasterRepository _stateMasterRepository;
    private readonly ICountryMasterRepository _countryMasterRepository;
    private readonly IEntityReferenceService _entityReferenceService;
    public UpdateStateCommandHandler(IStateMasterRepository stateMasterRepository, ICountryMasterRepository countryMasterRepository, IEntityReferenceService entityReferenceService)
    {
        _stateMasterRepository = stateMasterRepository;
        _countryMasterRepository = countryMasterRepository;
        _entityReferenceService = entityReferenceService;
    }

    public async Task<bool> Handle(UpdateStateCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var state = await _stateMasterRepository.GetStateByIdAsync(request.StateId);
            if (state == null) return false;

            // Check if the country is actively referenced before restricting change
            int? activeReferenceId = await _entityReferenceService.GetActiveReferenceIdAsync(nameof(state.CountryId), state.CountryId);
            if (activeReferenceId.HasValue && state.CountryId != request.UpdateStateDto.CountryId)
            {
                throw new Exception(AppConstants.ErrorCodes.COMMON_MASTER_LIST_LINKED_ITEM_DELETE_CANNOT_REMAP);
            }

            state.StateName = request.UpdateStateDto.StateName;
            state.StateShortName = request.UpdateStateDto.StateShortName;
            state.Description = request.UpdateStateDto.Description;

            return await _stateMasterRepository.UpdateStateAsync(state);
        }
        catch
        {
            throw new Exception(AppConstants.ErrorCodes.STATE_MASTER_UPDATE_FAILED);
        }
    }
}