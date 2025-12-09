using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elixir.Application.Features.Clients.DTOs;

public class ReportingToolLimitsDto
{
    public int ClientReportingToolId { get; set; }
    public string? ClientReportingAdmins { get; set; }
    public string? ClientCustomerReportCreators { get; set; }
    public string? ClientSavedReportQueriesLibrary { get; set; }
    public string? ClientSavedReportQueriesPerUser { get; set; }
    public string? ClientDashboardLibrary { get; set; }
    public string? ClientDashboardPersonalLibrary { get; set; }
    public int? CreatedBy { get; set; }
    public int? UpdatedBy { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int ClientId { get; set; }
}
