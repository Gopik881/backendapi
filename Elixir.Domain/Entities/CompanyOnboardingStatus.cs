namespace Elixir.Domain.Entities;

public partial class CompanyOnboardingStatus
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public bool IsActive { get; set; }

    public int ClientId { get; set; }

    public int CompanyId { get; set; }

    public string OnboardingStatus { get; set; } = null!;

    public string? RejectedReason { get; set; }

    public bool IsDeleted { get; set; }

}
