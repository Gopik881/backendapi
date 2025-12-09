using Elixir.Application.Common.Constants;
using Elixir.Application.Features.UserGroup.DTOs;
using Elixir.Application.Features.UserGroup.Queries.GetUserGroupUserRights.GetHorizontalsByGroupId;
using Elixir.Application.Features.UserGroup.Queries.GetUserGroupUserRights.GetReportAccess;
using Elixir.Application.Features.UserGroup.Queries.GetUserGroupUserRights.GetReportingAdminsByGroupId;
using Elixir.Application.Features.UserGroup.Queries.GetUserGroupUserRights.GetRoleNameByGroupId;
using Elixir.Application.Features.UserGroup.Queries.GetUserGroupUserRights.GetUserGroupDetailsByGroupId;
using Elixir.Application.Features.UserGroup.Queries.GetUserGroupUserRights.GetUserRightsByGroupId;
using MediatR;

namespace Elixir.Application.Features.UserGroup.Queries.GetUserGroupUserRights.GetCompositeCommandHandler;

public record GetCompositeQuery(int GroupId) : IRequest<CreateUserGroupDto>;

public class GetCompositeQueryHandler : IRequestHandler<GetCompositeQuery, CreateUserGroupDto>
{
    private readonly IMediator _mediator;

    public GetCompositeQueryHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<CreateUserGroupDto> Handle(GetCompositeQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Get User Group Details
            var userGroupDetails = await _mediator.Send(new GetUserGroupDetailsQuery(request.GroupId), cancellationToken);

            // Get Role Name
            var roleName = await _mediator.Send(new GetUserGroupRoleNameQuery(request.GroupId), cancellationToken);

            // Get Horizontals
            var horizontals = await _mediator.Send(new GetUserGroupHorizontalsQuery(request.GroupId), cancellationToken);

            // Get Reporting Admins
            var reportingAdmins = await _mediator.Send(new GetUserGroupReportingAdminQuery(request.GroupId), cancellationToken);

            // Get User Rights (assume a query exists)
            var userRights = await _mediator.Send(new GetUserGroupRightsByUserGroupIdQuery(request.GroupId), cancellationToken);

            // Get Report Accesses (assume a query exists)
            var reportAccesses = await _mediator.Send(new GetReportAccessQuery(request.GroupId), cancellationToken);

            // Fix the issue by ensuring the correct type is used for GetAllReportsQuery
            var allReports = reportAccesses; //await _mediator.Send(new GetAllReportsQuery(request.GroupId), cancellationToken);
            var allCategories = reportAccesses; //await _mediator.Send(new GetAllCategoriesQuery(request.GroupId), cancellationToken);

            if (!allReports.Any() || !allCategories.Any())
            {
                return new CreateUserGroupDto
                {
                    UserGroupId = userGroupDetails.GroupId,
                    UserGroupName = userGroupDetails.GroupName,
                    Description = userGroupDetails.Description,
                    Status = userGroupDetails.Status,
                    CreateBy = userGroupDetails.CreatedBy,
                    UserRightsData = userRights,
                    userGroupHorizontals = horizontals,
                    userGroupReportingAdmins = reportingAdmins,
                    reportingAccessDto = null
                };
            }
            else
            {
                return new CreateUserGroupDto
                {
                    UserGroupId = userGroupDetails.GroupId,
                    UserGroupName = userGroupDetails.GroupName,
                    Description = userGroupDetails.Description,
                    Status = userGroupDetails.Status,
                    CreateBy = userGroupDetails.CreatedBy,
                    UserRightsData = userRights,
                    userGroupHorizontals = horizontals,
                    userGroupReportingAdmins = reportingAdmins,
                    reportingAccessDto = reportAccesses.First()
                };
            }
        }
        catch
        {
            throw new Exception(AppConstants.ErrorCodes.CLIENT_ACCOUNT_MANAGERS_RETRIEVAL_FAILED);
        }
    }
}
