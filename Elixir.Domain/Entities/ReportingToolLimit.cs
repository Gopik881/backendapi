namespace Elixir.Domain.Entities;

public partial class ReportingToolLimit
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public int CompanyId { get; set; }

    public int? NoOfReportingAdmins { get; set; }

    public int? NoOfCustomReportCreators { get; set; }

    public int? SavedReportQueriesInLibrary { get; set; }

    public int? SavedReportQueriesPerUser { get; set; }

    public int? DashboardsInLibrary { get; set; }

    public int? DashboardsInPersonalLibrary { get; set; }

    public int? LetterGenerationAdmins { get; set; }

    public int? TemplatesSaved { get; set; }

    public bool IsDeleted { get; set; }

}
