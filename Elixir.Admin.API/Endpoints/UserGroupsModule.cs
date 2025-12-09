using Elixir.Application.Common.Constants;
using Elixir.Application.Common.Models;
using Elixir.Application.Features.User.DTOs;
using Elixir.Application.Features.User.Queries.GetUserAssociatedGroup;
using Elixir.Application.Features.User.Queries.GetPagedUserGroupUserCount;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Elixir.Application.Features.UserGroup.Queries.GetUserGroupAllUsers;
using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Features.UserGroup.Queries.GetAllUsersforUserMapping;
using Elixir.Application.Features.UserGroup.DTOs;
using Elixir.Application.Features.UserGroup.Queries.GetUserMappingsGroupNames;
using Elixir.Application.Features.UserGroup.Queries.GetUserMappedGroupNameswithCompanies;
using Elixir.Application.Features.UserGroup.Queries.GetUserMappingUsersByGroupId;
using Elixir.Application.Features.UserGroup.Queries.GetCreatedUserGroupUsersByGroupId;
using Elixir.Application.Features.UserGroup.Queries.GetFilteredUserGroupMappingUsers;
using Elixir.Application.Features.UserGroup.Commands.UserGroupMapping.AddUserGroupMapping;
using Elixir.Application.Features.UserGroup.Commands.UserGroupMapping.RemoveUserGroupMappping;
using Microsoft.AspNetCore.Mvc;
using Elixir.Application.Features.UserGroup.Queries.GetFilteredUserGroupUsers;
using FluentValidation;
using Elixir.Application.Features.UserGroup.Commands.CreateUserGroupRights.CompositeHandler;
using Elixir.Application.Features.UserGroup.Commands.UpdateUserGroupRights.CompositeHandler;
using Elixir.Application.Features.UserGroup.Commands.DeleteUserGroup.DeleteUserGroupComposite;
using Elixir.Application.Features.UserGroup.Queries.GetUserGroupUserRights.GetCompositeCommandHandler;


namespace Elixir.Admin.API.Endpoints;
public static class UserGroupsModule
{
    private static ILogger _logger;

    public static void RegisterUserGroupsEndpoints(this IEndpointRouteBuilder endpoints)
    {
        // Get the ILoggerFactory from the service provider
        var loggerFactory = endpoints.ServiceProvider.GetRequiredService<ILoggerFactory>();

        // Create a logger for state
        _logger ??= loggerFactory.CreateLogger("UserGroupsApiRoutes");

        endpoints.MapGet("api/v{version}/user-groups/mapped", [Authorize] async (int version, IMediator mediator, ClaimsPrincipal principal) =>
        {
            var userId = Convert.ToInt32(principal.FindFirst(AppConstants.USER_ID)?.Value);
            _logger.LogInformation("Fetching mapped user groups for userId: {UserId}", userId);
            var query = new GetUserAssociatedGroupQuery(userId);
            var result = await mediator.Send(query);
            if (result == null)
            {
                _logger.LogInformation("No Groups found for the User with userId: {UserId}", userId);
                return Results.Json(new ApiResponse<string>(401, AppConstants.ErrorCodes.USER_GROUP_USERS_USER_NOT_FOUND, false, String.Empty));
            }
            _logger.LogInformation("User Groups found for the userId: {UserId}", userId);
            return Results.Json(new ApiResponse<IEnumerable<UserGroupDto>>(200, AppConstants.ErrorCodes.USER_GROUPS_FETCHED_SUCCESSFULLY, true, result));
        });

        endpoints.MapGet("api/v{version}/user-groups/{IsDefaultUserGroupType}/{pageNumber}/{pageSize}", [Authorize] async (int version, bool IsSuperAdmin, string? searchTerm, bool IsDefaultUserGroupType, int pageNumber, int pageSize, IMediator mediator) =>
        {
            searchTerm = searchTerm?.Trim() ?? string.Empty;
            pageNumber = pageNumber <= 0 ? 1 : pageNumber;
            pageSize = pageSize <= 0 ? 1 : pageSize;
            _logger.LogInformation("Fetching paged user groups with searchTerm: '{SearchTerm}', pageNumber: {PageNumber}, pageSize: {PageSize}", searchTerm, pageNumber, pageSize);
            var query = new GetPagedUserGroupUserCountQuery(IsSuperAdmin, searchTerm, IsDefaultUserGroupType, pageNumber, pageSize);
            var result = await mediator.Send(query);
            if (result == null)
            {
                _logger.LogInformation("User Groups not found for searchTerm: '{SearchTerm}'", searchTerm);
                return Results.Json(new ApiResponse<string>(404, AppConstants.ErrorCodes.USER_GROUP_USERS_USER_NOT_FOUND, false, String.Empty));
            }
            _logger.LogInformation("User Groups fetched successfully for searchTerm: '{SearchTerm}'", searchTerm);
            return Results.Json(new ApiResponse<PaginatedResponse<UserGroupUserCountDto>>(200, AppConstants.ErrorCodes.USER_GROUPS_FETCHED_SUCCESSFULLY, true, result));
        });

        endpoints.MapGet("api/v{version}/user-groups/all-users", [Authorize] async (int version, IMediator mediator) =>
        {
            _logger.LogInformation("Fetching all users for all user groups.");
            var result = await mediator.Send(new GetUserGroupAllUsersQuery());
            if (result == null || !result.Any())
            {
                _logger.LogInformation("No users found for any user group.");
                return Results.Json(new ApiResponse<string>(404, AppConstants.ErrorCodes.USER_GROUP_USERS_USER_NOT_FOUND, false, string.Empty));
            }
            _logger.LogInformation("All users for all user groups fetched successfully.");
            return Results.Json(new ApiResponse<IEnumerable<CompanyUserDto>>(200, AppConstants.ErrorCodes.USER_GROUPS_FETCHED_SUCCESSFULLY, true, result));
        });

        endpoints.MapGet("api/v{version}/user-groups/mapping-eligibility", [Authorize] async (int version, IMediator mediator) =>
        {
            _logger.LogInformation("Fetching all users for user mapping.");
            var result = await mediator.Send(new GetAllUsersforUserMappingQuery());
            if (result == null || !result.Any())
            {
                _logger.LogInformation("No users found for user mapping.");
                return Results.Json(new ApiResponse<string>(404, AppConstants.ErrorCodes.USER_GROUP_USERS_USER_NOT_FOUND, false, string.Empty));
            }
            _logger.LogInformation("All users for user mapping fetched successfully.");
            return Results.Json(new ApiResponse<IEnumerable<UserListforUserMappingDto>>(200, AppConstants.ErrorCodes.USER_GROUPS_FETCHED_SUCCESSFULLY, true, result));
        });

        endpoints.MapGet("api/v{version}/user-groups/types/groups", [Authorize] async (int version, IMediator mediator) =>
        {
            _logger.LogInformation("Fetching user mapping group names by group type.");
            var result = await mediator.Send(new GetUserMappingGroupNamesByGroupTypeQuery());
            if (result == null || !result.Any())
            {
                _logger.LogInformation("No user mapping group names found.");
                return Results.Json(new ApiResponse<string>(404, AppConstants.ErrorCodes.USER_GROUP_USERS_USER_NOT_FOUND, false, string.Empty));
            }
            _logger.LogInformation("User mapping group names fetched successfully.");
            return Results.Json(new ApiResponse<IEnumerable<UserMappingGroupsByGroupTypeDto>>(200, AppConstants.ErrorCodes.USER_GROUPS_FETCHED_SUCCESSFULLY, true, result));
        });

        endpoints.MapGet("api/v{version}/user-groups/mapping/{groupId}/users", [Authorize] async (int version, int groupId, IMediator mediator) =>
        {
            _logger.LogInformation("Fetching users for mapping by groupId: {GroupId}", groupId);
            var query = new GetUserMappingUsersByGroupIdQuery(groupId);
            var result = await mediator.Send(query);
            if (result == null || !result.Any())
            {
                _logger.LogInformation("No users found for the specified groupId: {GroupId}", groupId);
                return Results.Json(new ApiResponse<string>(404, AppConstants.ErrorCodes.USER_GROUP_USERS_USER_NOT_FOUND, false, string.Empty));
            }
            _logger.LogInformation("Users for the specified groupId: {GroupId} fetched successfully.", groupId);
            return Results.Json(new ApiResponse<IEnumerable<CompanyUserDto>>(200, AppConstants.ErrorCodes.USER_GROUPS_FETCHED_SUCCESSFULLY, true, result));
        });

        endpoints.MapGet("api/v{version}/user-groups/{userId}/companies", [Authorize] async (int version, int userId, IMediator mediator, ClaimsPrincipal principal) =>
        {
            _logger.LogInformation("Fetching mapped group names with companies for userId: {UserId}", userId);
            var query = new GetUserMappedGroupNameswithCompaniesQuery(userId);
            var result = await mediator.Send(query);
            if (result == null)
            {
                _logger.LogInformation("No mapped group names with companies found for the userId: {UserId}", userId);
                return Results.Json(new ApiResponse<string>(404, AppConstants.ErrorCodes.USER_GROUP_USERS_USER_NOT_FOUND, false, string.Empty));
            }
            _logger.LogInformation("Mapped group names with companies fetched successfully for userId: {UserId}", userId);
            return Results.Json(new ApiResponse<UserMappedGroupNamesWithCompaniesDto>(200, AppConstants.ErrorCodes.USER_GROUPS_FETCHED_SUCCESSFULLY, true, result));
        });

        endpoints.MapGet("api/v{version}/user-groups/{groupId}/users", [Authorize] async (int version, int groupId, IMediator mediator) =>
        {
            _logger.LogInformation("Fetching created user group users by groupId: {GroupId}", groupId);
            var query = new GetCreatedUserGroupUsersByGroupIdQuery(groupId);
            var result = await mediator.Send(query);
            if (result == null || !result.Any())
            {
                _logger.LogInformation("No created users found for the specified groupId: {GroupId}", groupId);
                return Results.Json(new ApiResponse<string>(404, AppConstants.ErrorCodes.USER_GROUP_USERS_USER_NOT_FOUND, false, string.Empty));
            }
            _logger.LogInformation("Created users for the specified groupId: {GroupId} fetched successfully.", groupId);
            return Results.Json(new ApiResponse<IEnumerable<CompanyUserDto>>(200, AppConstants.ErrorCodes.USER_DETAILS_FETCHED_SUCCESSFULLY, true, result));
        });

        endpoints.MapDelete("api/v{version}/users-groups/mapping/{groupId}", [Authorize] async (int version, int groupId, [FromBody] List<int> userIds, IMediator mediator, ClaimsPrincipal principal) =>
        {
            var userId = Convert.ToInt32(principal.FindFirst(AppConstants.USER_ID)?.Value);
            _logger.LogInformation("Removing user mapping for groupId: {GroupId}", groupId);
            var command = new RemoveUserGroupMappingCommand(userIds,userId, groupId);
            var result = await mediator.Send(command);
            if (!result)
            {
                _logger.LogError("User mapping removal failed for groupId: {GroupId}", groupId);
                return Results.Json(new ApiResponse<string>(400, AppConstants.ErrorCodes.USER_GROUP_USERS_REMOVE_MAPPING_FAILED, false, string.Empty));
            }
            _logger.LogInformation("User mapping removed successfully for groupId: {GroupId}", groupId);
            return Results.Json(new ApiResponse<bool>(200, AppConstants.ErrorCodes.USER_GROUP_USERS_REMOVE_MAPPING_SUCCESS, true, true));
        });

        endpoints.MapPost("api/v{version}/users-groups/mapping/{groupId}", [Authorize] async (int version, int groupId, [FromBody] List<int> userIds, IMediator mediator, ClaimsPrincipal principal) =>
        {
            var userId = Convert.ToInt32(principal.FindFirst(AppConstants.USER_ID)?.Value);
            _logger.LogInformation("Adding user mapping for groupId: {GroupId}", groupId);
            var command = new AddUserGroupMappingCommand(userIds,userId,groupId);
            var result = await mediator.Send(command);
            if (!result)
            {
                _logger.LogError("User mapping failed for groupId: {GroupId}", groupId);
                return Results.Json(new ApiResponse<string>(400, AppConstants.ErrorCodes.USER_GROUP_USERS_ADD_MAPPING_FAILED, false, string.Empty));
            }
            _logger.LogInformation("User mapping added successfully for groupId: {GroupId}", groupId);
            return Results.Json(new ApiResponse<bool>(201, AppConstants.ErrorCodes.USER_GROUP_USERS_ADD_MAPPING_SUCCESS, true, true));
        });

        endpoints.MapGet("api/v{version}/user-groups/eligible/users/{pageNumber}/{pageSize}", [Authorize] async (int version, bool isEligibleToBeRemoved, int groupId, int pageNumber, int pageSize, string? searchTerm, IMediator mediator) =>
        {
            _logger.LogInformation(
                "Fetching filtered user group mapping users. RemoveUserFromMapping: {IsRemoveUserfromMapping}, GroupId: {GroupId}, Page: {PageNumber}, Size: {PageSize}, SearchTerm: {SearchTerm}",
                isEligibleToBeRemoved, groupId, pageNumber, pageSize, searchTerm);

            searchTerm = searchTerm?.Trim() ?? string.Empty;
            pageNumber = pageNumber <= 0 ? 1 : pageNumber;
            pageSize = pageSize <= 0 ? 1 : pageSize;

            var query = new GetFilteredUserGroupMappingUsersListQuery(isEligibleToBeRemoved, groupId, pageNumber, pageSize, searchTerm);

            var result = await mediator.Send(query);

            if (result == null || result.Data == null || !result.Data.Any())
            {
                _logger.LogError("No filtered user group mapping users found for GroupId: {GroupId} and SearchTerm: {SearchTerm}", groupId, searchTerm);
                return Results.Json(new ApiResponse<string>(404, AppConstants.ErrorCodes.USER_GROUP_USERS_USER_NOT_FOUND, false, string.Empty));
            }

            _logger.LogInformation("Filtered user group mapping users fetched successfully for GroupId: {GroupId}", groupId);
            return Results.Json(new ApiResponse<PaginatedResponse<UserListforUserMappingDto>>(200, AppConstants.ErrorCodes.USER_GROUP_USERS_FETCHED_SUCCESSFULLY, true, result));
        });

        endpoints.MapGet("api/v{version}/user-groups/{groupId}/mapped/users/{pageNumber}/{pageSize}", [Authorize] async (int version, int groupId, int pageNumber, int pageSize, string? searchTerm, IMediator mediator) =>
        {
            searchTerm = searchTerm?.Trim() ?? string.Empty;
            pageNumber = pageNumber <= 0 ? 1 : pageNumber;
            pageSize = pageSize <= 0 ? 1 : pageSize;
            _logger.LogInformation("Fetching filtered users for user group. GroupId: {GroupId}, Page: {PageNumber}, Size: {PageSize}, SearchTerm: {SearchTerm}", groupId, pageNumber, pageSize, searchTerm);
            var query = new GetFilteredUserGroupUsersQuery(groupId, pageNumber, pageSize, searchTerm);
            var result = await mediator.Send(query);
            if (result == null || result.Data == null || !result.Data.Any())
            {
                _logger.LogInformation("No filtered users found for user group. GroupId: {GroupId}, SearchTerm: {SearchTerm}", groupId, searchTerm);
                return Results.Json(new ApiResponse<string>(404, AppConstants.ErrorCodes.USER_GROUP_USERS_USER_NOT_FOUND, false, string.Empty));
            }
            _logger.LogInformation("Filtered users for user group fetched successfully. GroupId: {GroupId}", groupId);
            return Results.Json(new ApiResponse<PaginatedResponse<CompanyUserDto>>(200, AppConstants.ErrorCodes.USER_GROUP_USERS_FETCHED_SUCCESSFULLY, true, result));
        });

        endpoints.MapPost("api/v{version}/user-groups", [Authorize] async (int version, [FromBody] CreateUserGroupDto createUserGroupDto, IMediator mediator, IValidator<UserGroupCompositeCommand> validator, ClaimsPrincipal principal) =>
        {
            var userId = Convert.ToInt32(principal.FindFirst(AppConstants.USER_ID)?.Value);
            createUserGroupDto.CreateBy = userId;
            if (createUserGroupDto == null)
              {
                  _logger.LogError("CreateUserGroupDto is null.");
                  return Results.Json(new ApiResponse<string>(400, AppConstants.ErrorCodes.EMAIL_INVALID_REQUEST, false, string.Empty));
              }

              var command = new UserGroupCompositeCommand(createUserGroupDto);

              _logger.LogInformation("Validating UserGroupCompositeCommand for UserGroupId");
              var validationResult = await validator.ValidateAsync(command);
              if (!validationResult.IsValid)
              {
                  _logger.LogError("Validation failed for UserGroupCompositeCommand for UserGroupId: {UserGroupId}");
                  return Results.Json(new ApiResponse<IDictionary<string, string[]>>(400, AppConstants.ErrorCodes.VALIDATION_FAILED, false, validationResult.ToDictionary()));
              }

              _logger.LogInformation("Sending UserGroupCompositeCommand to mediator for UserGroupId: {UserGroupId}");
              var result = await mediator.Send(command);
            if (result is not bool boolResult || !boolResult)
            {                
                _logger.LogError("Failed to create UserGroup composite.");
                return Results.Json(new ApiResponse<object>(200, AppConstants.ErrorCodes.USER_GROUP_USERS_CREATE_FAILED, true, result));
            }
              _logger.LogInformation("UserGroup composite created successfully for UserGroupId: {UserGroupId}");
              return Results.Json(new ApiResponse<string>(200, AppConstants.ErrorCodes.USER_GROUP_CREATED_SUCCESS, true, string.Empty));
          });

        endpoints.MapPut("api/v{version}/user-groups/{userGroupId}", [Authorize] async (int version, int userGroupId, [FromBody] CreateUserGroupDto updateCompositeDto, IMediator mediator, IValidator<UpdateCompositeCommand> validator, ClaimsPrincipal principal) =>
        {
            var userId = Convert.ToInt32(principal.FindFirst(AppConstants.USER_ID)?.Value);
            _logger.LogInformation("Received request to update user group with UserGroupId: {UserGroupId}", userGroupId);
            if (updateCompositeDto == null)
            {
                _logger.LogError("UpdateCompositeDto is null.");
                return Results.Json(new ApiResponse<string>(400, AppConstants.ErrorCodes.EMAIL_INVALID_REQUEST, false, string.Empty));
            }

            var command = new UpdateCompositeCommand(userId, userGroupId, updateCompositeDto);

            _logger.LogInformation("Validating UpdateCompositeCommand for UserGroupId: {UserGroupId}", userGroupId);
            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                _logger.LogError("Validation failed for UpdateCompositeCommand for UserGroupId: {UserGroupId}", userGroupId);
                return Results.Json(new ApiResponse<IDictionary<string, string[]>>(400, AppConstants.ErrorCodes.VALIDATION_FAILED, false, validationResult.ToDictionary()));
            }

            _logger.LogInformation("Sending UpdateCompositeCommand to mediator.");
            var result = await mediator.Send(command);
            if (result is not bool boolResult || !boolResult)
            {
                _logger.LogError("Failed to update user group with UserGroupId: {UserGroupId}", userGroupId);
                return Results.Json(new ApiResponse<object>(200, AppConstants.ErrorCodes.USER_RIGHTS_EDIT_GROUP_FAILED, false, result));
            }

            _logger.LogInformation("User group updated successfully with UserGroupId: {UserGroupId}", userGroupId);
            return Results.Json(new ApiResponse<string>(200, AppConstants.ErrorCodes.USER_GROUP_USERS_ADD_MAPPING_SUCCESS, true, string.Empty));
        });

        endpoints.MapGet("api/v{version}/user-groups/{groupId}", [Authorize] async (int version, int groupId, IMediator mediator) =>
        {
            _logger.LogInformation("Fetching composite user group details for groupId: {GroupId}", groupId);
            var query = new GetCompositeQuery(groupId);
            var result = await mediator.Send(query);
            if (result == null)
            {
                _logger.LogInformation("No composite user group details found for groupId: {GroupId}", groupId);
                return Results.Json(new ApiResponse<string>(404, AppConstants.ErrorCodes.USER_GROUP_USERS_USER_NOT_FOUND, false, string.Empty));
            }
            _logger.LogInformation("Composite user group details fetched successfully for groupId: {GroupId}", groupId);
            return Results.Json(new ApiResponse<CreateUserGroupDto>(200, AppConstants.ErrorCodes.USER_GROUPS_FETCHED_SUCCESSFULLY, true, result));
        });

        endpoints.MapDelete("api/v{version}/user-groups/{userGroupId}", [Authorize] async (int version, int userGroupId, IMediator mediator) =>
        {
            _logger.LogInformation("Received request to delete user group composite with UserGroupId: {UserGroupId}", userGroupId);

            var command = new DeleteCompositeCommand(userGroupId);
            var result = await mediator.Send(command);

            if (!result)
            {
                _logger.LogError("Failed to delete user group composite with UserGroupId: {UserGroupId}", userGroupId);
                return Results.Json(new ApiResponse<string>(409, AppConstants.ErrorCodes.USER_RIGHTS_DELETE_GROUP_FAILED, false, string.Empty));
            }

            _logger.LogInformation("User group composite deleted successfully with UserGroupId: {UserGroupId}", userGroupId);
            return Results.Json(new ApiResponse<string>(200, AppConstants.ErrorCodes.USER_RIGHTS_GROUP_DELETED_SUCCESS, true, string.Empty));
        });


    }
}
