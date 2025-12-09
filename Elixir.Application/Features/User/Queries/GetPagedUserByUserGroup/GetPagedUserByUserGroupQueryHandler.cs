using Elixir.Application.Common.Models;
using Elixir.Application.Features.User.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elixir.Application.Features.User.Queries.GetPagedUserByUserGroup;

public record GetPagedUserByUserGroupQuery(int UserId, int UseGroupId, string SearchTerm, int PageNumber, int PageSize) : IRequest<PaginatedResponse<UsersUserGroupStatusDto>>;
public class GetPagedUserByUserGroupQueryHandler : IRequestHandler<GetPagedUserByUserGroupQuery, PaginatedResponse<UsersUserGroupStatusDto>>
{
    private readonly IUserGroupMappingsRepository _userGroupMappingsRepository;
    private readonly IUserGroupsRepository _userGroupsRepository;
    private readonly IUsersRepository _usersRepository;
    public GetPagedUserByUserGroupQueryHandler(IUserGroupMappingsRepository userGroupMappingsRepository, IUsersRepository usersRepository, IUserGroupsRepository userGroupsRepository)
    {
        _userGroupMappingsRepository = userGroupMappingsRepository;
        _userGroupsRepository = userGroupsRepository;
        _usersRepository = usersRepository;
    }
    public Task<PaginatedResponse<UsersUserGroupStatusDto>> Handle(GetPagedUserByUserGroupQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
