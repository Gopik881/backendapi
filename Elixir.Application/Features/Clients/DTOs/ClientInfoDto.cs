using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elixir.Application.Features.Clients.DTOs;

public class ClientInfoDto
{
    public int ClientId { get; set; }
    public string ClientName { get; set; }
    public string ClientInfo { get; set; }
    public bool? Status { get; set; }
    public string ClientCode { get; set; }
    public int CompanyId { get; set; }
    public string Address1 { get; set; }
    public string Address2 { get; set; }
    public int? StateId { get; set; }
    public string? StateName { get; set; }
    public string? CountryName { get; set; }
    public int? CountryId { get; set; }
    public string ZipCode { get; set; }
    public string CountryCode { get; set; }
    public string PhoneNumber { get; set; }
    public DateTime? CreatedAt { get; set; }
    public int? CreatedBy { get; set; }
    public int? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
