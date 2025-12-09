using Elixir.Application.Common.Constants;
using Elixir.Application.Common.Enums;
using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Features.Notification.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Domain.Entities;
using Elixir.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using static Azure.Core.HttpHeader;

namespace Elixir.Infrastructure.Persistance.Repositories;

public class NotificationsRepository : INotificationsRepository
{
    private readonly ElixirHRDbContext _dbContext;

    public NotificationsRepository(ElixirHRDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<IEnumerable<NotificationDto>> GetAllUserNotificationsAsync(int userId)
    {
        var result = await _dbContext.Notifications
            .Where(unm => unm.Id == userId)
            .Join(
                _dbContext.Notifications,
                unm => unm.Id,
                n => n.Id,
                (unm, n) => n
            )
            .Select(n => new NotificationDto
            {
                Id = n.Id,
                NotificationType = n.NotificationType,
                Title = n.Title,
                Message = n.Message,
                IsRead = n.IsRead ?? false,
                CreatedBy = n.CreatedBy.HasValue ? n.CreatedBy.Value : 0,
            }).ToListAsync();

        return result;
    }
    public async Task<bool> UpdateAllUserMarkAsReadAsync(int userId)
    {
        // Find all unread notifications for the user.
        var notifications = await _dbContext.Notifications
            .Where(n => n.UserId == userId && (n.IsRead == null || n.IsRead == false))
            .ToListAsync();

        if (notifications.Count == 0)
            return false;

        // Mark all as read.
        foreach (var notification in notifications)
        {
            notification.IsRead = true;
        }

        // Save changes and return true if any were updated.
        return await _dbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateUserMarkAsReadAsync(int NotificationId)
    {
        // Find the notification by its primary key (Id) where IsRead is false or null.
        var notification = await _dbContext.Notifications
            .FirstOrDefaultAsync(n => n.Id == NotificationId);

        if (notification == null)
            return false;

        // Mark as read.
        notification.IsRead = true;

        // Save changes and return true if updated.
        return await _dbContext.SaveChangesAsync() > 0;
    }
    
    public async Task<List<CompanyAdminDetailsDto>> GetClientCompanyAdminIDsAsync(int clientId)
    {
        // Pseudocode:
        // 1. Query ClientCompaniesMapping for mappings with the given clientId.
        // 2. Join with Company to get company details.
        // 3. Join with ElixirUser to get users with RoleId == Company Admin.
        // 4. Join with Client to get client name.
        // 5. Project to CompanyAdminDetailsDto.
        // 6. Return as list.

        var companyAdminRoleId = (int)Roles.CompanyAdmin;

        var result = await (from ccm in _dbContext.ClientCompaniesMappings
                            join com in _dbContext.Companies on ccm.CompanyId equals com.Id
                            join user in _dbContext.ElixirUsers on com.Id equals user.CompanyId
                            join client in _dbContext.Clients on ccm.ClientId equals client.Id
                            where ccm.ClientId == clientId && user.RoleId == companyAdminRoleId
                            select new CompanyAdminDetailsDto
                            {
                                UserId = user.UserId,
                                CompanyId = com.Id,
                                CompanyName = com.CompanyName ?? string.Empty,
                                ClientName = client.ClientName ?? string.Empty
                            }).ToListAsync();

        return result;
    }
    public async Task<List<int>> GetMappedCompanyIdsAsync(int clientId)
    {
        // Pseudocode:
        // 1. Query ClientCompaniesMappings for mappings with the given clientId.
        // 2. Select CompanyId from those mappings.
        // 3. Return as list.

        var existingMappings = await _dbContext.ClientCompaniesMappings
            .Where(ccm => ccm.ClientId == clientId)
            .ToListAsync();

        return existingMappings.Select(ccm => ccm.CompanyId).ToList();
    }
    // Pseudocode:
    // 1. Accept company creation request, createdBy, companyId, and optional status change message.
    // 2. For each user group (makers, custom users, checkers), create a notification and related user notification mappings.
    // 3. Use current Notification and UserNotificationsMapping entities and save to the database in a single transaction.
    // 4. Return true if successful, false otherwise.


   
    public async Task<bool> InsertNotificationAsync(NotificationDto notificationDto)
    {
        // Pseudocode:
        // 1. Map NotificationDto to Notification entity.
        // 2. Add Notification entity to Notifications table.
        // 3. Save changes and return true if successful.

        var notification = new Notification
        {
            CreatedAt = notificationDto.CreatedAt,
            UpdatedAt = notificationDto.UpdatedAt,
            Title = notificationDto.Title,
            Message = notificationDto.Message,
            NotificationType = notificationDto.NotificationType,
            IsRead = notificationDto.IsRead,
            IsDeleted = notificationDto.IsDeleted,
            CompanyId = notificationDto.CompanyId,
            IsActive = notificationDto.IsActive,
            UserId = notificationDto.UserId
        };

        await _dbContext.Notifications.AddAsync(notification);
        var result = await _dbContext.SaveChangesAsync();
        return result > 0;
    }
    public async Task<Tuple<List<NotificationDto>, int>> GetFilteredNotificationsAsync(int? userId, bool IsSuperAdmin, int pageNumber, int pageSize, string? searchTerm)
    {
        var query = _dbContext.Notifications.AsQueryable();
        // Count of unread notifications (IsRead == false or null)
        int unreadCount;
        if (IsSuperAdmin)
        {
            unreadCount = await query.CountAsync(n => n.NotificationType == AppConstants.NOTIFICATION_CONDITION_FOR_SUPER_ADMIN && (n.IsRead == false));
        }
        else
        {
            unreadCount = await query.CountAsync(n => n.UserId == userId && (n.IsRead == false));
        }

        if (IsSuperAdmin)
        {
            query = query.Where(n => n.Title == AppConstants.NOTIFICATION_PROFILE_UPDATED && n.NotificationType == AppConstants.NOTIFICATION_CONDITION_FOR_SUPER_ADMIN);
        }
        else if (userId.HasValue)
        {
            query = query.Where(n => n.UserId == userId.Value);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var lowerSearch = searchTerm.ToLower();
            query = query.Where(n =>
                (n.Title != null && n.Title.ToLower().Contains(lowerSearch)) ||
                (n.Message != null && n.Message.ToLower().Contains(lowerSearch))
            );
        }

        var totalCount = await query.CountAsync();

        var pagedNotifications = await query
            .OrderByDescending(n => n.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var notificationDtos = pagedNotifications.Select(n => new NotificationDto
        {
            Id = n.Id,
            CreatedAt = n.CreatedAt,
            CreatedOn = n.CreatedAt,
            UpdatedAt = n.UpdatedAt ?? default,
            CreatedBy = n.CreatedBy ?? 0,
            UpdatedBy = n.UpdatedBy ?? 0,
            Title = n.Title,
            Message = n.Message,
            NotificationType = n.NotificationType,
            IsRead = n.IsRead ?? false,
            IsDeleted = n.IsDeleted,
            CompanyId = n.CompanyId ?? 0,
            IsActive = n.CompanyId.HasValue
                ? (_dbContext.Companies
                    .Where(c => c.Id == n.CompanyId.Value)
                    .Select(c => c.IsEnabled)
                    .FirstOrDefault() == true)
                : false,
            UserId = (int)n.UserId,
            NotificationCount = unreadCount // Include unread count in each notification DTO
        }).ToList();

        return new Tuple<List<NotificationDto>, int>(notificationDtos, totalCount);
    }



}

