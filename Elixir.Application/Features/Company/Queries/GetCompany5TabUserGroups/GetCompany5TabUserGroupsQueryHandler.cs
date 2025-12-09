using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Company.Queries.GetCompany5TabUserGroups;

public record GetCompany5TabUserGroupsQuery(int CompanyId) : IRequest<IEnumerable<Company5TabUserGroupDto>>;

public class GetCompany5TabUserGroupsQueryHandler : IRequestHandler<GetCompany5TabUserGroupsQuery, IEnumerable<Company5TabUserGroupDto>>
{
    private readonly IUserGroupsRepository _company5TabUserGroupRepository;

    public GetCompany5TabUserGroupsQueryHandler(IUserGroupsRepository company5TabUserGroupRepository)
    {
        _company5TabUserGroupRepository = company5TabUserGroupRepository;
    }

    public async Task<IEnumerable<Company5TabUserGroupDto>> Handle(GetCompany5TabUserGroupsQuery request, CancellationToken cancellationToken)
    {
        return await _company5TabUserGroupRepository.GetCompany5TabUserGroupsByCompanyIdAsync(request.CompanyId);
    }
}
