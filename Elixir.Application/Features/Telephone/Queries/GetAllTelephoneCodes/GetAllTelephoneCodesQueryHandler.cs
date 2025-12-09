using Elixir.Application.Features.Telephone.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Telephone.Queries.GetAllTelephoneCodes;

public record GetAllTelephoneCodesQuery : IRequest<IEnumerable<TelephoneCodeMasterDto>>;

public class GetAllTelephoneCodesQueryHandler : IRequestHandler<GetAllTelephoneCodesQuery, IEnumerable<TelephoneCodeMasterDto>>
{
    private readonly ITelephoneCodeMasterRepository _telephoneCodeMasterRepository;

    public GetAllTelephoneCodesQueryHandler(ITelephoneCodeMasterRepository telephoneCodeMasterRepository)
    {
        _telephoneCodeMasterRepository = telephoneCodeMasterRepository;
    }

    public async Task<IEnumerable<TelephoneCodeMasterDto>> Handle(GetAllTelephoneCodesQuery request, CancellationToken cancellationToken)
    {
        return await _telephoneCodeMasterRepository.GetAllTelephoneCodesAsync();
    }
}
