using Elixir.Application.Features.UserGroup.DTOs;
using Elixir.Domain.Entities;

namespace Elixir.Application.Interfaces.Persistance;
public interface IHorizontalsRepository
{
    Task<bool> AddHorizontalsAsync(int groupId, List<UserGroupHorizontals> horizontals);
    Task<bool> UpdateHorizontalsAsync(int groupId, List<UserGroupHorizontals> horizontals);
    Task<List<UserGroupHorizontals>> GetHorizontalsForRoleAsync(int userGroupId);
    Task<bool> DeleteHorizontalsByUserGroupIdAsync(int groupId);

    Task<List<UserGroup>> CheckForDuplicateHorizontalsAsync(List<UserGroupHorizontals> horizontals, int? userGroupId = 0);
}