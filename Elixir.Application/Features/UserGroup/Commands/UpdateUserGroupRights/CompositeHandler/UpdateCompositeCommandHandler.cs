using Elixir.Application.Common.Constants;
using Elixir.Application.Common.Models;
using Elixir.Application.Features.UserGroup.Commands.UpdateUserGroupRights.UpdateHorizontals;
using Elixir.Application.Features.UserGroup.Commands.UpdateUserGroupRights.UpdateReportAccess;
using Elixir.Application.Features.UserGroup.Commands.UpdateUserGroupRights.UpdateReportingAdmins;
using Elixir.Application.Features.UserGroup.Commands.UpdateUserGroupRights.UpdateUserGroup;
using Elixir.Application.Features.UserGroup.Commands.UpdateUserGroupRights.UpdateUserRights;
using Elixir.Application.Features.UserGroup.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.UserGroup.Commands.UpdateUserGroupRights.CompositeHandler;

/// <summary>
/// Command to update a composite user group with all related rights and access.
/// </summary>
public record UpdateCompositeCommand(int userId, int UserGroupId, CreateUserGroupDto UpdateCompositeDto) : IRequest<object>;

/// <summary>
/// Handles the update of a composite user group, including all related rights and access.
/// </summary>
public class UpdateCompositeCommandHandler : IRequestHandler<UpdateCompositeCommand, object>
{
    private readonly IMediator _mediator;
    private readonly Func<ITransactionRepository> _transactionRepositoryFactory;
    private readonly IUserGroupsRepository _userGroupsRepository;
    private readonly INotificationsRepository _notficationsRepository;
    private readonly IHorizontalsRepository _horizontalsRepository;
    private readonly IReportAccessRepository _reportingAccessRepository;
    private readonly IReportingAdminRepository _reportingAdminRepository;

    public UpdateCompositeCommandHandler(
        IMediator mediator,
        Func<ITransactionRepository> transactionRepositoryFactory,
        IUserGroupsRepository userGroupsRepository, INotificationsRepository notificationsRepository, IHorizontalsRepository horizontalsRepository, IReportAccessRepository reportingAccessRepository, IReportingAdminRepository reportingAdminRepository)
    {
        _mediator = mediator;
        _transactionRepositoryFactory = transactionRepositoryFactory;
        _userGroupsRepository = userGroupsRepository;
        _notficationsRepository = notificationsRepository;
        _horizontalsRepository = horizontalsRepository;
        _reportingAccessRepository = reportingAccessRepository;
        _reportingAdminRepository = reportingAdminRepository;
    }

    public async Task<object> Handle(UpdateCompositeCommand request, CancellationToken cancellationToken)
    {
        var errorMessages = new List<string>();
        var dto = request.UpdateCompositeDto;

        // 1. Check for required fields
        if (dto == null)
        {
            errorMessages.Add("User group data is required.");
            return new ApiResponse<List<string>>(400, "Validation failed.", false, errorMessages);
        }

        if (string.IsNullOrWhiteSpace(dto.UserGroupName))
            //errorMessages.Add("User group name is required.");
            throw new Exception("User group name is required.");

        // 2. Check if group name is available (if changed)
        if (!string.IsNullOrWhiteSpace(dto.UserGroupName))
        {
            // Only check for name availability if the name is being changed
            var existingGroup = await _userGroupsRepository.GetUserGroupByIdAsync(request.UserGroupId);
            if (existingGroup == null || !string.Equals(existingGroup.GroupName, dto.UserGroupName, StringComparison.OrdinalIgnoreCase))
            {
                var isAvailable = await _userGroupsRepository.IsGroupNameAvailableAsync(dto.UserGroupName);
                if (!isAvailable)
                    //errorMessages.Add($"User group name '{dto.UserGroupName}' is already in use.");
                    throw new Exception($"User group name '{dto.UserGroupName}' is already in use.");
            }
        }

        var duplicateGroupsforUpdate = await _userGroupsRepository.CheckForDuplicateGroupConfigurationsAsync(
            dto.UserGroupMenuRights ?? new List<UserGroupMenuRights>(),
            dto.userGroupHorizontals ?? new List<UserGroupHorizontals>(),
            dto.reportingAccessDto ?? new ReportingAccessDto(),
            dto.userGroupReportingAdmins ?? new List<UserGroupReportingAdmin>(),
            request.UserGroupId);
        if (duplicateGroupsforUpdate != null && duplicateGroupsforUpdate.Count > 0)
        {
            var duplicateGroupNames = string.Join(", ", duplicateGroupsforUpdate.Select(g => g.GroupName));
            throw new Exception($"Duplicate user rights found for the provided combination for userGroup(s): {duplicateGroupNames}.");
        }


        using var transactionRepository = _transactionRepositoryFactory();
        await transactionRepository.BeginTransactionAsync();
        try
        {
            var userRightsdto = request.UpdateCompositeDto;
            var userGroupId = request.UserGroupId;

            // 1. Update User Group
            if (userRightsdto != null)
            {
                var updateUserGroupCommand = new UpdateUserGroupCommand(userGroupId, userRightsdto);
                if (!await _mediator.Send(updateUserGroupCommand, cancellationToken))
                    throw new Exception("Failed to update user group.");
            }

            // 2. Update User Rights
            if (userRightsdto?.UserGroupMenuRights != null && userRightsdto.UserGroupMenuRights.Count > 0)
            {
                var updateUserRightsCommand = new UpdateUserRightsCommand(userGroupId, userRightsdto.UserGroupMenuRights);
                if (!await _mediator.Send(updateUserRightsCommand, cancellationToken))
                    throw new Exception("Failed to update user rights.");
            }

            // 3. Update Horizontals
            if (userRightsdto?.userGroupHorizontals != null && userRightsdto.userGroupHorizontals.Count > 0)
            {
                var updateHorizontalsCommand = new UpdateHorizontalsCommand(userGroupId, userRightsdto.userGroupHorizontals);
                if (!await _mediator.Send(updateHorizontalsCommand, cancellationToken))
                    throw new Exception("Failed to update horizontals.");
            }

            // 4. Update Report Access
            if (userRightsdto?.reportingAccessDto != null && userRightsdto?.reportingAccessDto.Reports.Count > 0)
            {
                var updateReportAccessCommand = new UpdateReportAccessCommand(userGroupId, userRightsdto.reportingAccessDto);
                if (!await _mediator.Send(updateReportAccessCommand, cancellationToken)) ;
                //throw new Exception("Failed to update report access.");
            }

            // 5. Update Reporting Admin
            if (userRightsdto?.userGroupReportingAdmins != null && userRightsdto.userGroupReportingAdmins.Count > 0)
            {
                var updateReportingAdminCommand = new UpdateReportingAdminsCommand(userGroupId, userRightsdto.userGroupReportingAdmins);
                if (!await _mediator.Send(updateReportingAdminCommand, cancellationToken))
                    throw new Exception("Failed to update reporting admins.");
            }

            await transactionRepository.CommitAsync();

            return true;
        }
        catch
        {
            await transactionRepository.RollbackAsync();
            //return false;
            throw new Exception(AppConstants.ErrorCodes.UPDATE_COMPANY_5TAB_FAILED);
        }
    }
}
