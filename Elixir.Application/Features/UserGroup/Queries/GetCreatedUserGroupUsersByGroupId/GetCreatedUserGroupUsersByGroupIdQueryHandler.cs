using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.UserGroup.Queries.GetCreatedUserGroupUsersByGroupId;

public record GetCreatedUserGroupUsersByGroupIdQuery(int GroupId) : IRequest<List<CompanyUserDto>>;

public class GetCreatedUserGroupUsersByGroupIdQueryHandler : IRequestHandler<GetCreatedUserGroupUsersByGroupIdQuery, List<CompanyUserDto>>
{
    private readonly IUserGroupsRepository _userGroupRepository;

    public GetCreatedUserGroupUsersByGroupIdQueryHandler(IUserGroupsRepository userGroupRepository)
    {
        _userGroupRepository = userGroupRepository;
    }

    public async Task<List<CompanyUserDto>> Handle(GetCreatedUserGroupUsersByGroupIdQuery request, CancellationToken cancellationToken)
    {
        return await _userGroupRepository.GetUserGroupUsersByGroupIdAsync(request.GroupId);
    }
}
