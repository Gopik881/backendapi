namespace Elixir.Domain.Entities;

public partial class AuditLog
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int CompanyId { get; set; }

    public string Action { get; set; } = null!;

    public string EntityName { get; set; } = null!;

    public int EntityId { get; set; }

    public string? Details { get; set; }

    public bool IsDeleted { get; set; }

}
