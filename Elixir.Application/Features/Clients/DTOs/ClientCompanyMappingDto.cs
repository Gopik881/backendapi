using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elixir.Application.Features.Clients.DTOs
{
    public class ClientCompanyMappingDto
    {
        public int? ClientCompanyMappingId { get; set; }
        public int? ClientId { get; set; }
        public int CompanyId { get; set; }
        public string? CompanyName { get; set; }
        public List<object>? Users { get; set; }
    }

}
