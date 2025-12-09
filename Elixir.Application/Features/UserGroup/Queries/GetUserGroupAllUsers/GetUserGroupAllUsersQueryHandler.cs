using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Features.User.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.UserGroup.Queries.GetUserGroupAllUsers;

public record GetUserGroupAllUsersQuery() : IRequest<IEnumerable<CompanyUserDto>>;

public class GetUserGroupAllUsersQueryHandler : IRequestHandler<GetUserGroupAllUsersQuery, IEnumerable<CompanyUserDto>>
{
    private readonly IUserGroupMappingsRepository _userGroupMappingsRepository;

    public GetUserGroupAllUsersQueryHandler(IUserGroupMappingsRepository userGroupMappingsRepository)
    {
        _userGroupMappingsRepository = userGroupMappingsRepository;
    }

    public async Task<IEnumerable<CompanyUserDto>> Handle(GetUserGroupAllUsersQuery request, CancellationToken cancellationToken)
    {
        return await _userGroupMappingsRepository.GetAllUserGroupUsersAsync();
    }
}
