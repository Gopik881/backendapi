namespace Elixir.Domain.Entities;

public partial class Company5TabOnboardingHistory
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public int? CompanyId { get; set; }

    public string? Status { get; set; }

    public int? UserId { get; set; }

    public string? Reason { get; set; }

    public bool IsEnabled { get; set; }

    public bool IsDeleted { get; set; }

}
