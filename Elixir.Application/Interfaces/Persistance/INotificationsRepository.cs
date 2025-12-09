using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Features.Notification.DTOs;
namespace Elixir.Application.Interfaces.Persistance;
public interface INotificationsRepository
{

    Task<IEnumerable<NotificationDto>> GetAllUserNotificationsAsync(int userId);
    Task<bool> UpdateAllUserMarkAsReadAsync(int userId);
    Task<bool> UpdateUserMarkAsReadAsync(int NotificationId);    
    Task<List<CompanyAdminDetailsDto>> GetClientCompanyAdminIDsAsync(int clientId);
    Task<List<int>> GetMappedCompanyIdsAsync(int clientId);
    Task<Tuple<List<NotificationDto>, int>> GetFilteredNotificationsAsync(int? userId, bool IsSuperAdmin, int pageNumber, int pageSize, string? searchTerm);
    Task<bool> InsertNotificationAsync(NotificationDto notificationDto);

}