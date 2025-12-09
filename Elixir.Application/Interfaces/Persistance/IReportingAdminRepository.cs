using Elixir.Application.Features.UserGroup.DTOs;
using Elixir.Domain.Entities;

namespace Elixir.Application.Interfaces.Persistance;

public interface IReportingAdminRepository
{
    Task<bool> AddReportingAdminsAsync(int groupId, List<UserGroupReportingAdmin> reportingAdmins);
    Task<bool> UpdateReportingAdminsAsync(int groupId, List<UserGroupReportingAdmin> reportingAdmins);
    Task<List<UserGroupReportingAdmin>> GetReportingAdminsForRoleAsync(int groupId);
    Task<bool> DeleteReportingAdminsByUserGroupIdAsync(int groupId);

    Task<List<UserGroup>> CheckForDuplicateReportingAdminsAsync(List<UserGroupReportingAdmin> reportingAdmins, int? userGroupId = 0);
}