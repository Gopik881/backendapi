using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Features.User.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Company.Queries.GetCompany5TabCustomUsers
{
    public record GetCompany5TabCustomUserGroupsQuery(int CompanyId, string? ScreenName = "") : IRequest<List<UserGroupDto>>;

    public class GetCompany5TabCustomUserGroupsQueryHandler : IRequestHandler<GetCompany5TabCustomUserGroupsQuery, List<UserGroupDto>>
    {
        private readonly ICompaniesRepository _customUserGroupRepository;

        public GetCompany5TabCustomUserGroupsQueryHandler(ICompaniesRepository customUserGroupRepository)
        {
            _customUserGroupRepository = customUserGroupRepository;
        }

        public async Task<List<UserGroupDto>> Handle(GetCompany5TabCustomUserGroupsQuery request, CancellationToken cancellationToken)
        {
            return await _customUserGroupRepository.GetCompany5TabCustomUserGroups(request.CompanyId, request.ScreenName);
        }
    }
}
