using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elixir.Application.Features.Clients.DTOs
{
    public class ClientCompanyDto
    {
        public string ClientName { get; set; }
        public DateTime? CreatedOn { get; set; }
        public List<CompanyDto> Companies { get; set; } = new();
    }

    public class CompanyDto
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string? ZipCode { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
