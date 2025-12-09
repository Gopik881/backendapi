using Elixir.Application.Common.Constants;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Interfaces.Services;
using MediatR;

namespace Elixir.Application.Features.Telephone.Commands.DeleteTelephone;

public record DeleteTelephoneCommand(int TelephoneCodeId) : IRequest<bool>;

public class DeleteTelephoneCommandHandler : IRequestHandler<DeleteTelephoneCommand, bool>
{
    private readonly ITelephoneCodeMasterRepository _telephoneCodeMasterRepository;
    private readonly IEntityReferenceService _entityReferenceService;

    public DeleteTelephoneCommandHandler(
        ITelephoneCodeMasterRepository telephoneCodeMasterRepository,
        IEntityReferenceService entityReferenceService)
    {
        _telephoneCodeMasterRepository = telephoneCodeMasterRepository;
        _entityReferenceService = entityReferenceService;
    }

    public async Task<bool> Handle(DeleteTelephoneCommand request, CancellationToken cancellationToken)
    {
        if (await _entityReferenceService.HasActiveReferencesAsync(nameof(request.TelephoneCodeId), request.TelephoneCodeId))
            throw new Exception(AppConstants.ErrorCodes.TELEPHONE_CODE_MASTER_LIST_LINKED_ITEM_DELETE_ATTEMPT);

        return await _telephoneCodeMasterRepository.DeleteTelephoneCodeAsync(request.TelephoneCodeId);
    }
}
