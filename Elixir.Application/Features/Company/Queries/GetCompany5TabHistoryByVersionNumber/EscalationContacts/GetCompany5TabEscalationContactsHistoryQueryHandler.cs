using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Company.Queries.GetCompany5TabHistory.EscalationContacts;

public record GetCompany5TabEscalationContactsHistoryQuery(int UserId, int CompanyId, int VersionNumber) : IRequest<Company5TabHistoryDto>;

public class GetCompany5TabEscalationContactsHistoryQueryHandler : IRequestHandler<GetCompany5TabEscalationContactsHistoryQuery, Company5TabHistoryDto>
{
    private readonly IEscalationContactsHistoryRepository _companyEscalationContactsHistoryRepository;

    public GetCompany5TabEscalationContactsHistoryQueryHandler(IEscalationContactsHistoryRepository companyEscalationContactsHistoryRepository)
    {
        _companyEscalationContactsHistoryRepository = companyEscalationContactsHistoryRepository;
    }

    public async Task<Company5TabHistoryDto> Handle(GetCompany5TabEscalationContactsHistoryQuery request, CancellationToken cancellationToken)
    {
        return await _companyEscalationContactsHistoryRepository.GetCompany5TabEscalationContactsHistoryByVersionAsync(request.UserId, request.CompanyId, request.VersionNumber);
    }
}
