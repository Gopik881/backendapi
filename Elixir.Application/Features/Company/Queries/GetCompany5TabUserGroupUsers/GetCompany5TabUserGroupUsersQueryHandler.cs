using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Company.Queries.GetCompany5TabUserGroupUsers;

public record GetCompany5TabUserGroupUsersQuery(int GroupId, int CompanyId) : IRequest<List<CompanyUserDto>>;

public class GetCompany5TabUserGroupUsersQueryHandler : IRequestHandler<GetCompany5TabUserGroupUsersQuery, List<CompanyUserDto>>
{
    private readonly IUserGroupMappingsRepository _userGroupRepository;

    public GetCompany5TabUserGroupUsersQueryHandler(IUserGroupMappingsRepository userGroupRepository)
    {
        _userGroupRepository = userGroupRepository;
    }

    public async Task<List<CompanyUserDto>> Handle(GetCompany5TabUserGroupUsersQuery request, CancellationToken cancellationToken)
    {
        return await _userGroupRepository.GetCompany5TabUserGroupUsersAsync(request.GroupId, request.CompanyId);
    }
}
