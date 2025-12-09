using Elixir.Application.Common.Constants;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Interfaces.Services;
using MediatR;

namespace Elixir.Application.Features.Country.Commands.DeleteCountry;
public record DeleteCountryCommand(int CountryId) : IRequest<bool>;
public class DeleteCountryCommandHandler : IRequestHandler<DeleteCountryCommand, bool>
{
    private readonly ICountryMasterRepository _countryMasterRepository;
    private readonly IEntityReferenceService _entityReferenceService;

    public DeleteCountryCommandHandler(ICountryMasterRepository countryMasterRepository, IEntityReferenceService entityReferenceService)
    {
        _countryMasterRepository = countryMasterRepository;
        _entityReferenceService = entityReferenceService;
    }

    public async Task<bool> Handle(DeleteCountryCommand request, CancellationToken cancellationToken)
    {
        if (await _entityReferenceService.HasActiveReferencesAsync(nameof(request.CountryId), request.CountryId))
            throw new Exception(AppConstants.ErrorCodes.COMMON_MASTER_LIST_LINKED_ITEM_DELETE_ATTEMPT);
        return await _countryMasterRepository.DeleteCountryAsync(request.CountryId);
    }
}