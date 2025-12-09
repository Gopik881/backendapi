using Elixir.Application.Features.Telephone.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Telephone.Queries.GetValidateDuplicateTelephoneCodes;

public record GetValidateDuplicateTelephoneCodesQuery(CreateUpdateTelephoneCodeDto TelephoneCode) : IRequest<bool>;

public class GetValidateDuplicateTelephoneCodesQueryHandler : IRequestHandler<GetValidateDuplicateTelephoneCodesQuery, bool>
{
    private readonly ITelephoneCodeMasterRepository _telephoneCodeRepository;

    public GetValidateDuplicateTelephoneCodesQueryHandler(ITelephoneCodeMasterRepository telephoneCodeRepository)
    {
        _telephoneCodeRepository = telephoneCodeRepository;
    }

    public async Task<bool> Handle(GetValidateDuplicateTelephoneCodesQuery request, CancellationToken cancellationToken)
    {
        return await _telephoneCodeRepository.IsDuplicateTelephoneCodeExistsAsync(request.TelephoneCode);
    }
}
