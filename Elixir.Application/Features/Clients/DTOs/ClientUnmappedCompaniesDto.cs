using Elixir.Application.Features.Company.DTOs;

namespace Elixir.Application.Features.Clients.DTOs;

public class ClientUnmappedCompaniesDto
{
    public int CompanyId { get; set; }
    public string CompanyName { get; set; }
    public List<CompanyUserDto> AccountManagers { get; set; }
}
