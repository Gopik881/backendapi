using Elixir.Application.Common.Constants;
using Elixir.Application.Features.Telephone.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Domain.Entities;
using MediatR;

namespace Elixir.Application.Features.Telephone.Commands.CreateTelephone;
public record CreateTelephoneCommand(List<CreateUpdateTelephoneCodeDto> CreateTelephoneCodeDto) : IRequest<bool>;
public class CreateTelephoneCommandHandler : IRequestHandler<CreateTelephoneCommand, bool>
{
    private readonly ITelephoneCodeMasterRepository _telephonecodeMasterRepository;
    private readonly ICountryMasterRepository _countryMasterRepository;
    public CreateTelephoneCommandHandler(ICountryMasterRepository countryMasterRepository, ITelephoneCodeMasterRepository telephoneCodeMasterRepository)
    {
        _countryMasterRepository = countryMasterRepository;
        _telephonecodeMasterRepository = telephoneCodeMasterRepository;
    }
    public async Task<bool> Handle(CreateTelephoneCommand request, CancellationToken cancellationToken)
    {
        try { 
            //bool anyDuplicateTelephoneCodesExists = await _telephonecodeMasterRepository.AnyDuplicateTelephoneCodesExistsAsync(request.CreateTelephoneCodeDto);

            //if (anyDuplicateTelephoneCodesExists) return false;
            

            foreach (var telephone in request.CreateTelephoneCodeDto)
            {
                //var countryExists = await _countryMasterRepository.ExistsAsync(telephone.CountryId);
                //if (!countryExists) return 0;

                //If Telephone code existing with same country donot create a duplicate.
                //var telephoneCodeExists = await _telephonecodeMasterRepository.ExistsWithTelephoneCodeAsync(telephone.TelephoneCode, telephone.CountryId);
                //if (telephoneCodeExists) return 0;


                var newTelephoneCode = new TelephoneCodeMaster
                {
                    CountryId = telephone.CountryId,
                    TelephoneCode = telephone.TelephoneCode,
                    Description = telephone.Description,
                };
                await _telephonecodeMasterRepository.CreateTelephoneCodeAsync(newTelephoneCode);
            }
            return true;
        }
        catch
        {
            throw new Exception(AppConstants.ErrorCodes.INVALID_TELEPHONE_CODE_DUPLICATE);
        }
    }
}
