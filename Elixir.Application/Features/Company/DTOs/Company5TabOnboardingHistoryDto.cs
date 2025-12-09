namespace Elixir.Application.Features.Company.DTOs;

public class Company5TabOnboardingHistoryDto
{
    public int companyId { get; set; }
    public int UserId { get; set; }
    public string FirstName { get; set; }
    public string Status { get; set; }
    public bool? isRead { get; set; }
    public string? Reason { get; set; }
    public DateTime? LatestUpdate { get; set; }
    public int? createdBy { get; set; }
}
