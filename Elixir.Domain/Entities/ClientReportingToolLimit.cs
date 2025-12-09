namespace Elixir.Domain.Entities;

public partial class ClientReportingToolLimit
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public string? ClientReportingAdmins { get; set; }

    public string? ClientCustomerReportCreators { get; set; }

    public string? ClientSavedReportQueriesLibrary { get; set; }

    public string? ClientSavedReportQueriesPerUser { get; set; }

    public string? ClientDashboardLibrary { get; set; }

    public string? ClientDashboardPersonalLibrary { get; set; }

    public int? ClientId { get; set; }

    public bool IsDeleted { get; set; }

}
