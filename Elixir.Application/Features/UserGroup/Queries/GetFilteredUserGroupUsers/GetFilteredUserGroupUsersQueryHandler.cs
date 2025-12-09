using Elixir.Application.Common.Models;
using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.UserGroup.Queries.GetFilteredUserGroupUsers;

public record GetFilteredUserGroupUsersQuery(int UserGroupId, int PageNumber, int PageSize, string SearchTerm) : IRequest<PaginatedResponse<CompanyUserDto>>;

public class GetFilteredUserGroupUsersQueryHandler : IRequestHandler<GetFilteredUserGroupUsersQuery, PaginatedResponse<CompanyUserDto>>
{
    private readonly IUserGroupMappingsRepository _userGroupMappingsRepository;

    public GetFilteredUserGroupUsersQueryHandler(IUserGroupMappingsRepository userGroupMappingsRepository)
    {
        _userGroupMappingsRepository = userGroupMappingsRepository;
    }

    public Task<PaginatedResponse<CompanyUserDto>> Handle(GetFilteredUserGroupUsersQuery request, CancellationToken cancellationToken)
    {
        var searchTerm = request.SearchTerm ?? string.Empty;
        var result = _userGroupMappingsRepository.GetFilteredUserGroupUsers(request.UserGroupId,request.PageNumber,request.PageSize,searchTerm);
        return Task.FromResult(new PaginatedResponse<CompanyUserDto>(result.Item1,new PaginationMetadata(result.Item2, request.PageSize, request.PageNumber)));
    }
}
