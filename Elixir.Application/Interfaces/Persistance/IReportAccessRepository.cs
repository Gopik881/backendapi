using Elixir.Application.Features.UserGroup.DTOs;
using Elixir.Domain.Entities;

namespace Elixir.Application.Interfaces.Persistance;
public interface IReportAccessRepository
{
    Task<bool> AddReportAccessAsync(int groupId, ReportingAccessDto reportAccess);
    Task<bool> UpdateReportAccessAsync(int groupId, ReportingAccessDto reportAccess);
    Task<List<ReportingAccessDto>> GetReportAccessData(int userGroupId);
    Task<List<SelectionItemDto>> GetReportsForRoleAsync(int userGroupId);
    Task<List<SelectionItemDto>> GetCategoriesForReportsAsync(int groupId);
    Task<bool> DeleteReportAccessByUserGroupIdAsync(int groupId);

    Task<List<UserGroup>> CheckForDuplicateReportAccessesAsync(ReportingAccessDto reportingAccessDto, int? userGroupId = 0);
}