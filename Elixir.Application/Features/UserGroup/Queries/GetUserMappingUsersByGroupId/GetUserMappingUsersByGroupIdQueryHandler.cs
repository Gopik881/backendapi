using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.UserGroup.Queries.GetUserMappingUsersByGroupId;

public record GetUserMappingUsersByGroupIdQuery(int GroupId) : IRequest<IEnumerable<CompanyUserDto>>;

public class GetUserMappingUsersByGroupIdQueryHandler : IRequestHandler<GetUserMappingUsersByGroupIdQuery, IEnumerable<CompanyUserDto>>
{
    private readonly IUserGroupMappingsRepository _userGroupMappingsRepository;

    public GetUserMappingUsersByGroupIdQueryHandler(IUserGroupMappingsRepository userGroupMappingsRepository)
    {
        _userGroupMappingsRepository = userGroupMappingsRepository;
    }

    public async Task<IEnumerable<CompanyUserDto>> Handle(GetUserMappingUsersByGroupIdQuery request, CancellationToken cancellationToken)
    {
        return await _userGroupMappingsRepository.GetUserMappingUsersByGroupIdAsync(request.GroupId);
    }
}
