namespace Elixir.Application.Features.Notification.DTOs;

public class NotificationDto
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int CreatedBy { get; set; }
    public int UpdatedBy { get; set; }
    public string Title { get; set; }
    public string Message { get; set; }
    public string NotificationType { get; set; }
    public bool IsRead { get; set; }
    public bool IsDeleted { get; set; }
    public int UserId { get; set; }
    public int CompanyId { get; set; }
    public bool IsActive { get; set; }
    public int NotificationCount { get; set; } = 0; // Default to 0 if not set
    public DateTime? CreatedOn { get; set; }
}

