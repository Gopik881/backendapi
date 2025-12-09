using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elixir.Application.Features.Clients.DTOs;

public class ClientContactInfoDto
{
    public int ClientContactId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string CountryCode { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public string Designation { get; set; }
    public string Department { get; set; }
    public string Remarks { get; set; }
    public int? CreatedBy { get; set; }
    public int? UpdatedBy { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int ClientId { get; set; }
}

