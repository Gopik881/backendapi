using Elixir.Application.Common.Constants;
using Elixir.Application.Features.State.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Domain.Entities;
using MediatR;

namespace Elixir.Application.Features.State.Commands.CreateState;

public record CreateStateCommand(List<CreateUpdateStateDto> CreateStateDto) : IRequest<bool>;
public class CreateStateCommandHandler : IRequestHandler<CreateStateCommand, bool>
{
    private readonly IStateMasterRepository _stateMasterRepository;
    private readonly ICountryMasterRepository _countryMasterRepository;

    public CreateStateCommandHandler(IStateMasterRepository stateMasterRepository, ICountryMasterRepository countryMasterRepository)
    {
        _stateMasterRepository = stateMasterRepository;
        _countryMasterRepository = countryMasterRepository;
    }

    public async Task<bool> Handle(CreateStateCommand request, CancellationToken cancellationToken)
    {
        try
        {

            bool anyDuplicateStatesExists = await _stateMasterRepository.AnyDuplicateStatesExistsAsync(request.CreateStateDto);
            if (anyDuplicateStatesExists) return false;

            foreach (var state in request.CreateStateDto)
            {
                //var countryExists = await _countryMasterRepository.ExistsAsync(state.CountryId);
                //if (!countryExists) return 0;

                ////If State existing with same country donot create a duplicate.
                //var stateExists = await _stateMasterRepository.ExistsWithStateNameAsync(state.StateName, state.CountryId);
                //if (stateExists) return 0;


                var newState = new StateMaster
                {
                    CountryId = state.CountryId,
                    StateName = state.StateName,
                    StateShortName = state.StateShortName,
                    Description = state.Description
                };

                await _stateMasterRepository.CreateStateAsync(newState);

            }
            return true;
        }
        catch
        {
            throw new Exception(AppConstants.ErrorCodes.STATE_CREATION_FAILED);
        }
        
    }
}
