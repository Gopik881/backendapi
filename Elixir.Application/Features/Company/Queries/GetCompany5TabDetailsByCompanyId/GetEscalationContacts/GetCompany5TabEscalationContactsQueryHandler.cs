using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Company.Queries.GetCompany5TabDetailsByCompanyId.GetEscalationContacts;

public record GetCompany5TabEscalationContactsQuery(int CompanyId, bool IsPrevious) : IRequest<List<Company5TabEscalationContactDto>>;

public class GetCompany5TabEscalationContactsQueryHandler : IRequestHandler<GetCompany5TabEscalationContactsQuery, List<Company5TabEscalationContactDto>>
{
    private readonly IEscalationContactsHistoryRepository _companyEscalationContactsHistoryRepository;

    public GetCompany5TabEscalationContactsQueryHandler(IEscalationContactsHistoryRepository companyEscalationContactsHistoryRepository)
    {
        _companyEscalationContactsHistoryRepository = companyEscalationContactsHistoryRepository;
    }

    public async Task<List<Company5TabEscalationContactDto>> Handle(GetCompany5TabEscalationContactsQuery request, CancellationToken cancellationToken)
    {
        return await _companyEscalationContactsHistoryRepository.GetCompany5TabLatestEscalationContactsHistoryAsync(request.CompanyId, request.IsPrevious);
    }
}
