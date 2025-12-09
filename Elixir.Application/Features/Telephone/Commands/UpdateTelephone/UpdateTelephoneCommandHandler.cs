using Elixir.Application.Common.Constants;
using Elixir.Application.Features.Telephone.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Interfaces.Services;
using MediatR;

namespace Elixir.Application.Features.Telephone.Commands.UpdateTelephone;

public record UpdateTelephoneCommand(int TelephoneCodeId, CreateUpdateTelephoneCodeDto UpdateTelephoneCodeDto) : IRequest<bool>;
public class UpdateTelephoneCommandHandler : IRequestHandler<UpdateTelephoneCommand, bool>
{
    private readonly ITelephoneCodeMasterRepository _telephoneCodeMasterRepository;
    private readonly ICountryMasterRepository _countryMasterRepository;
    private readonly IEntityReferenceService _entityReferenceService;
    public UpdateTelephoneCommandHandler(ITelephoneCodeMasterRepository telephoneCodeMasterRepository, ICountryMasterRepository countryMasterRepository, IEntityReferenceService entityReferenceService)
    {
        _telephoneCodeMasterRepository = telephoneCodeMasterRepository;
        _countryMasterRepository = countryMasterRepository;
        _entityReferenceService = entityReferenceService;
    }
    public async Task<bool> Handle(UpdateTelephoneCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var telephone = await _telephoneCodeMasterRepository.GetByIdAsync(request.TelephoneCodeId);
            if (telephone == null) return false;

            var countryExists = await _countryMasterRepository.ExistsAsync(request.UpdateTelephoneCodeDto.CountryId);
            if (!countryExists) return false;

            // Check if the telephone code is actively referenced before restricting change
            int? activeReferenceId = await _entityReferenceService.GetActiveReferenceIdAsync(nameof(telephone.CountryId), telephone.CountryId);
            if (activeReferenceId.HasValue && telephone.CountryId != request.UpdateTelephoneCodeDto.CountryId)
            {
                throw new Exception(AppConstants.ErrorCodes.COMMON_MASTER_LIST_LINKED_ITEM_DELETE_CANNOT_REMAP);
            }

            telephone.CountryId = request.UpdateTelephoneCodeDto.CountryId;
            telephone.TelephoneCode = request.UpdateTelephoneCodeDto.TelephoneCode;
            telephone.Description = request.UpdateTelephoneCodeDto.Description;

            var updateResult = await _telephoneCodeMasterRepository.UpdateTelephoneAsync(telephone);

            // Ensure a return value is provided for all code paths
            return updateResult > 0;
        }
        catch
        {
            throw new Exception(AppConstants.ErrorCodes.CLIENT_UPDATE_FAILED);
        }
    }
}
