namespace Elixir.Domain.Entities;

public partial class UserNotificationsMapping
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public int UserId { get; set; }

    public int NotificationId { get; set; }

    public bool? IsRead { get; set; }

    public bool IsDeleted { get; set; }

}
