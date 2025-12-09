namespace Elixir.Application.Interfaces.Persistance;

public interface IUserGroupMenuMappingRepository
{
    Task<bool> DeleteUserRightsByUserGroupIdAsync(int groupId);
}