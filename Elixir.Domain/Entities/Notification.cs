namespace Elixir.Domain.Entities;

public partial class Notification
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public string Title { get; set; } = null!;

    public string Message { get; set; } = null!;

    public string NotificationType { get; set; } = null!;
    public int? CompanyId { get; set; }
    public int? UserId { get; set; }
    public bool? IsActive { get; set; }

    public bool? IsRead { get; set; }

    public bool IsDeleted { get; set; }

}
