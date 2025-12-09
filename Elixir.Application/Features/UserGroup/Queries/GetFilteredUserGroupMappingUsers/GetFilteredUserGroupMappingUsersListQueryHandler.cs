using Elixir.Application.Common.Models;
using Elixir.Application.Features.UserGroup.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.UserGroup.Queries.GetFilteredUserGroupMappingUsers;

public record GetFilteredUserGroupMappingUsersListQuery(bool IsEligibleToBeRemoved,int GroupId,int PageNumber,int PageSize ,string SearchTerm) : IRequest<PaginatedResponse<UserListforUserMappingDto>>;

public class GetFilteredUserGroupMappingUsersListQueryHandler : IRequestHandler<GetFilteredUserGroupMappingUsersListQuery, PaginatedResponse<UserListforUserMappingDto>>
{
    private readonly IUserGroupMappingsRepository _userGroupMappingsRepository;

    public GetFilteredUserGroupMappingUsersListQueryHandler(IUserGroupMappingsRepository userGroupMappingsRepository)
    {
        _userGroupMappingsRepository = userGroupMappingsRepository;
    }

    public Task<PaginatedResponse<UserListforUserMappingDto>> Handle(GetFilteredUserGroupMappingUsersListQuery request, CancellationToken cancellationToken)
    {
        // Ensure SearchTerm is not null to avoid CS8604
        var searchTerm = request.SearchTerm ?? string.Empty;

        var result = _userGroupMappingsRepository.GetFilteredUserGroupMappingUsersListAsync(request.IsEligibleToBeRemoved,request.GroupId,request.PageNumber,request.PageSize,searchTerm);

        return Task.FromResult(
            new PaginatedResponse<UserListforUserMappingDto>(
                result.Item1,
                new PaginationMetadata(result.Item2, request.PageSize, request.PageNumber)
            )
        );
    }
}
