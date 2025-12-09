using Elixir.Application.Features.UserGroup.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.UserGroup.Queries.GetUserGroupUserRights.GetReportingAdminsByGroupId;

public record GetUserGroupReportingAdminQuery(int userGroupId) : IRequest<List<UserGroupReportingAdmin>>;

public class GetUserGroupReportingAdminQueryHandler : IRequestHandler<GetUserGroupReportingAdminQuery, List<UserGroupReportingAdmin>>
{
    private readonly IReportingAdminRepository _reportingAdminRepository;

    public GetUserGroupReportingAdminQueryHandler(IReportingAdminRepository reportingAdminRepository)
    {
        _reportingAdminRepository = reportingAdminRepository;
    }

    public async Task<List<UserGroupReportingAdmin>> Handle(GetUserGroupReportingAdminQuery request, CancellationToken cancellationToken)
    {
        return await _reportingAdminRepository.GetReportingAdminsForRoleAsync(request.userGroupId);
    }
}
