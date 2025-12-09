public class GetPagedNotificationDto
{
    public int NotificationId { get; set; }
    public DateTime NotificationCreatedAt { get; set; }
    public DateTime? NotificationUpdatedAt { get; set; }
    public int? NotificationCreatedBy { get; set; }
    public int? NotificationUpdatedBy { get; set; }
    public string Title { get; set; }
    public string Message { get; set; }
    public string NotificationType { get; set; }
    public bool? NotificationIsRead { get; set; }
    public bool NotificationIsDeleted { get; set; }

    public int UserMappingNotificationId { get; set; }
    public DateTime UserMappingCreatedAt { get; set; }
    public DateTime? UserMappingUpdatedAt { get; set; }
    public int? UserMappingCreatedBy { get; set; }
    public int? UserMappingUpdatedBy { get; set; }
    public int UserId { get; set; }
    public int UserMappingNotificationNotificationId { get; set; }
    public bool? UserMappingIsRead { get; set; }
    public bool UserMappingIsDeleted { get; set; }

    // Additional fields for screen navigation, company, etc. (if needed)
    public string? ScreenUrl { get; set; }
    public string? ScreenName { get; set; }
    public int? CompanyId { get; set; }
    public int? AssignedToUserId { get; set; }
    public string? ParameterToPass { get; set; }
}

