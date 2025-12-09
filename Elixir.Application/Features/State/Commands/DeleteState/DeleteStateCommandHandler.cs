using Elixir.Application.Common.Constants;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Interfaces.Services;
using MediatR;

namespace Elixir.Application.Features.State.Commands.DeleteState;
public record DeleteStateCommand(int StateId) : IRequest<bool>;
public class DeleteStateCommandHandler : IRequestHandler<DeleteStateCommand, bool>
{
    private readonly IStateMasterRepository _stateMasterRepository;
    private readonly IEntityReferenceService _entityReferenceService;

    public DeleteStateCommandHandler(IStateMasterRepository stateMasterRepository, IEntityReferenceService entityReferenceService)
    {
        _stateMasterRepository = stateMasterRepository;
        _entityReferenceService = entityReferenceService;
    }

    public async Task<bool> Handle(DeleteStateCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (await _entityReferenceService.HasActiveReferencesAsync(nameof(request.StateId), request.StateId))
                throw new Exception(AppConstants.ErrorCodes.STATE_MASTER_DELETE_FAILED);
            return await _stateMasterRepository.DeleteStateAsync(request.StateId);
        }
        catch
        {
            throw new Exception(AppConstants.ErrorCodes.STATE_MASTER_DELETE_FAILED);
        }
    }
}