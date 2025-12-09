using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elixir.Application.Features.Clients.DTOs;

public class ClientAccountManagersDto
{
    public int ClientAccountManagerId { get; set; }
    public int? ClientId { get; set; }
    public int GroupId { get; set; }
    public int UserId { get; set; }
    public string GroupName { get; set; }
    public List<string> Users { get; set; }
}

