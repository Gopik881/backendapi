using Elixir.Application.Common.Models;
using Elixir.Application.Features.User.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.User.Queries.GetPagedUserGroupUserCount
{
    public record GetPagedUserGroupUserCountQuery(bool IsSuperAdmin, string SearchTerm,bool IsDefaultUserGroupType, int PageNumber, int PageSize) : IRequest<PaginatedResponse<UserGroupUserCountDto>>;
    public class GetPagedUserGroupUserCountQueryHandler : IRequestHandler<GetPagedUserGroupUserCountQuery, PaginatedResponse<UserGroupUserCountDto>>
    {
        private readonly IUserGroupMappingsRepository _mappingRepository;

        public GetPagedUserGroupUserCountQueryHandler(IUserGroupMappingsRepository mappingRepository)
        {
            _mappingRepository = mappingRepository;
        }
        public async Task<PaginatedResponse<UserGroupUserCountDto>> Handle(GetPagedUserGroupUserCountQuery request, CancellationToken cancellationToken)
        {
            var result=await _mappingRepository.GetFilteredUserAssociatedGroupAsync(request.IsSuperAdmin, request.SearchTerm, request.IsDefaultUserGroupType, request.PageNumber, request.PageSize);
            // Apply Pagination
            return new PaginatedResponse<UserGroupUserCountDto>(result.Item1, new PaginationMetadata(result.Item2, request.PageSize, request.PageNumber));
        }
    }
}
