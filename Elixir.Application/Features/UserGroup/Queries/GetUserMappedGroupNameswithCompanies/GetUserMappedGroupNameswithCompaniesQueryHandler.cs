using Elixir.Application.Features.UserGroup.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.UserGroup.Queries.GetUserMappedGroupNameswithCompanies;

public record GetUserMappedGroupNameswithCompaniesQuery(int UserId) : IRequest<UserMappedGroupNamesWithCompaniesDto>;

public class GetUserMappedGroupNameswithCompaniesQueryHandler : IRequestHandler<GetUserMappedGroupNameswithCompaniesQuery, UserMappedGroupNamesWithCompaniesDto>
{
    private readonly IUserGroupMappingsRepository _userGroupMappingsRepository;

    public GetUserMappedGroupNameswithCompaniesQueryHandler(IUserGroupMappingsRepository userGroupMappingsRepository)
    {
        _userGroupMappingsRepository = userGroupMappingsRepository;
    }

    public async Task<UserMappedGroupNamesWithCompaniesDto> Handle(GetUserMappedGroupNameswithCompaniesQuery request, CancellationToken cancellationToken)
    {
        return await _userGroupMappingsRepository.GetUserMappedGroupNamesWithCompaniesAsync(request.UserId);
    }
}
