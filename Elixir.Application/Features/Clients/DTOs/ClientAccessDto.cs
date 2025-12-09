using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elixir.Application.Features.Clients.DTOs;

public class ClientAccessDto
{
    public int ClientAccessId { get; set; }
    public bool? EnableWebQuery { get; set; }
    public bool? EnableWebReportAccess { get; set; }
    public int? ClientUserLimit { get; set; }
    public int? CreatedBy { get; set; }
    public int? UpdatedBy { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? ClientId { get; set; }
}
